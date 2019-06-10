using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoseController : MonoBehaviour {

    public bool applyCorrectionCode = true;

    private Dictionary<string , Dictionary<float , float>> knownAttenuationCoefficients = new Dictionary<string , Dictionary<float , float>>();
    private Dictionary<string , Dictionary<float , float>> attenConstants = new Dictionary<string , Dictionary<float , float>>();
    private float airAttenuation = 0.05f;

    private List<Shield> listShields = new List<Shield>();
    private List<DoseBody> listBodies = new List<DoseBody>();
    private List<Source> listSources = new List<Source>();


    public bool debug;

    private bool enableDoseSystem = true;

    private bool staggerCalculations = true;
    private float staggerTime = 0.05f;

    private int updateTickInterval = 8;
    private int lastGameObjectCount = 0;
    
    //This class is the controller for the dose system
    //Only one of these per scene

    // Use this for initialization
    void Start() {

        readCSV();

        StartCoroutine( DoseCalculator() );

    }

    int t = 0;
    int updateTick = 0;

    // Update is called once per frame
    void Update() {
        
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        if ( allObjects.Length != lastGameObjectCount ) {

            lastGameObjectCount = allObjects.Length;

            if ( updateTick == ( 60 * updateTickInterval ) ) {

                updateListShields( allObjects  );

            }
            else if ( updateTick == ( 60 * updateTickInterval ) + 20 ) {

                updateListBodies( allObjects  );

            }
            else if ( updateTick == ( 60 * updateTickInterval ) + 40 ) {

                updateListSources( allObjects );
                updateTick = 0;

            }

        }

        t++;
        updateTick++;

    }

    IEnumerator DoseCalculator() {

        while ( true ) {

            yield return new WaitForSecondsRealtime( 0.1f );

            if ( t > 90 * 2 ) {
                
                List<Source> sources = getSources();
                List<Shield> shields = getShields();
                List<DoseBody> doseBodies = getDoseBodies();

                if ( enableDoseSystem ) {

                    foreach ( DoseBody doseBody in doseBodies ) {

                        yield return new WaitForSecondsRealtime( staggerTime );

                        float countRate = 0;
                        float doseRate = 0;

                        foreach ( DoseReceptor doseReceptor in doseBody.getDoseReceptors() ) {

                            yield return new WaitForSecondsRealtime( staggerTime );

                            foreach ( Source source in sources ) {

                                yield return new WaitForSecondsRealtime( staggerTime );
                                List<Isotope> decayChain = new List<Isotope>();

                                foreach ( Isotope isotope in source.getIsotopes() ) {

                                    recurseDecayChain( ref decayChain , isotope , 0 );

                                }

                                yield return new WaitForSecondsRealtime( staggerTime );

                                foreach ( Isotope isotope in decayChain ) {

                                    float averageParticleEnergy = isotope.getGammaDecayEnergy() + isotope.getBetaDecayEnergy();

                                    float attenuatedActivity = source.getActivity( isotope.getIsotopeName() );

                                    Vector3 origin = doseReceptor.getPosistion();

                                    //Sort shields
                                    shields = sortShields( shields , doseReceptor.getPosistion() );

                                    bool passedThroughShieldAny = false;

                                    yield return new WaitForSecondsRealtime( staggerTime );

                                    for ( int i = 0 ; i < shields.Count ; i++ ) {

                                        bool passedShield = false;

                                        Shield shield = shields[ i ];

                                        Vector3[] points = lineShieldIntersection( origin , source.getPosistion() , shield );

                                        if ( points[ 0 ] != Vector3.zero && points[ 1 ] != Vector3.zero ) {

                                            passedShield = true;
                                            passedThroughShieldAny = true;

                                            //This is our thickness
                                            float thickness = Vector3.Distance( points[ 0 ] , points[ 1 ] );

                                            Vector3 closestPoint = points[ 0 ];
                                            Vector3 furthestPoint = points[ 1 ];

                                            if ( Vector3.Distance( origin , points[ 1 ] ) < Vector3.Distance( origin , points[ 0 ] ) ) {

                                                closestPoint = points[ 1 ];
                                                furthestPoint = points[ 0 ];

                                            }

                                            float airAttenuationDistance = Vector3.Distance( origin , closestPoint );

                                            attenuatedActivity = materialAttenuate( attenuatedActivity , airAttenuation , airAttenuationDistance );

                                            if ( debug ) {

                                                DrawLine( origin , closestPoint , Color.red );
                                                DrawLine( closestPoint , furthestPoint , Color.green );

                                            }


                                            string assumed = "Concrete (Ordinary)";
                                            string renderName = shield.getName();

                                            if ( attenConstants.ContainsKey( renderName ) ) {

                                                assumed = renderName;

                                            }

                                            if ( attenConstants.ContainsKey( assumed ) ) {

                                                attenuatedActivity = materialAttenuate( attenuatedActivity , getMaterialAttenuationCoefficient( assumed , averageParticleEnergy ) , thickness );

                                                if ( passedShield && applyCorrectionCode ) {

                                                    float materialAttenuationCorrection = Mathf.Exp( -( 3.1f + ( ( ( float ) thickness / 100 ) * 0.0513678f ) ) );

                                                    float distanceDoseDetector = ( doseReceptor.getPosistion() - shield.GetComponent<Transform>().position ).magnitude * 100; //cm

                                                    float buildup = 1f; //Fix later

                                                    attenuatedActivity *= doseBody.getEfficiency() * materialAttenuationCorrection;


                                                    if ( distanceDoseDetector < 50 && isotope.getBetaDecayEnergy() != 0 ) {

                                                        //Less than 50cm, & beta so we gotta consider scattering
                                                        attenuatedActivity += ( ( 50 - distanceDoseDetector ) * 0.095f * attenuatedActivity );

                                                    }

                                                }


                                            }

                                            origin = furthestPoint;

                                        }

                                    }



                                    if ( debug ) {

                                        DrawLine( origin , source.getPosistion() , Color.blue );

                                    }

                                    yield return new WaitForSecondsRealtime( staggerTime );

                                    attenuatedActivity = materialAttenuate( attenuatedActivity , airAttenuation , Vector3.Distance( origin , source.getPosistion() ) );

                                    //Attenuate distance
                                    attenuatedActivity = ( ( attenuatedActivity ) / ( 4 * Mathf.PI * Mathf.Pow( ( doseReceptor.getPosistion() - source.getPosistion() ).magnitude , 2 ) ) ) * doseReceptor.getSurfaceArea();


                                    if ( applyCorrectionCode && !passedThroughShieldAny ) {


                                        //Correction

                                        float distanceDoseSource = ( doseReceptor.getPosistion() - source.GetComponent<Transform>().position ).magnitude * 100; //cm

                                        float geometricFactor = 0.37f;

                                        if ( distanceDoseSource < 3 ) { //Less than 3 centimeters

                                            geometricFactor = ( float ) ( distanceDoseSource * ( ( -0.03180557 + ( 19.65851 - -0.03180557 ) / ( 1 + Mathf.Pow( ( distanceDoseSource / 1.494623f ) , 1.501856f ) ) ) / ( ( 0.00113283 + ( 12233.67 - 0.00113283 ) / ( 1 + Mathf.Pow( ( distanceDoseSource / 0.04303171f ) , 2.002215f ) ) ) ) ) );

                                        }

                                        attenuatedActivity *= doseBody.getEfficiency() * geometricFactor;

                                    }



                                    countRate += attenuatedActivity;

                                    //Dose rate
                                    //averageActivity * particleEnergies[ i ] yields keV/s
                                    //keV/s / 6241506479963235 yields j/s, conversion factor
                                    //Dividing that by weight yields j/kg*s, and a Sv=j/kg
                                    doseRate += ( attenuatedActivity * averageParticleEnergy * 1000 * 3600 ) / ( 6241506479963235 * doseReceptor.getMass() ); //Yields mSv/hr


                                }


                            }

                        }


                        doseBody.setCountRate( countRate );
                        doseBody.setDoseRate( doseRate );

                    }

                }

                t = 0;

            }

        }
        

    }

    private float getMaterialAttenuationCoefficient( string materialName , float averageParticleEnergy ) {

        if ( knownAttenuationCoefficients.ContainsKey( materialName ) ) {

            if ( knownAttenuationCoefficients[ materialName ].ContainsKey( averageParticleEnergy) ) {
                
                return knownAttenuationCoefficients[ materialName ][ averageParticleEnergy ];

            }

        }

        Dictionary<float , float> attenData = attenConstants[ materialName ];

        float materialAttenuationCoefficient = 10; //Concrete for 1000 keV, uses this if it cant find a useful energy

        List<float> keyList = new List<float>( attenData.Keys );

        if ( averageParticleEnergy < keyList[ 0 ] ) { //If the particle energy is below the lowest energy

            //y = mx + b
            float m = ( attenData[ keyList[ 1 ] ] - attenData[ keyList[ 0 ] ] ) / ( keyList[ 1 ] - keyList[ 0 ] );
            float b = attenData[ keyList[ 1 ] ] - ( m * keyList[ 1 ] );

            materialAttenuationCoefficient = ( m * averageParticleEnergy ) + b;

        }
        else if ( averageParticleEnergy > keyList[ attenData.Keys.Count - 1 ] ) { //If the particle energy is larger than the highest particle energy

            //y = mx + b
            float m = ( attenData[ keyList[ attenData.Keys.Count - 2 ] ] - attenData[ keyList[ attenData.Keys.Count - 1 ] ] ) / ( keyList[ attenData.Keys.Count - 2 ] - keyList[ attenData.Keys.Count - 1 ] );
            float b = attenData[ keyList[ attenData.Keys.Count - 1 ] ] - ( m * keyList[ attenData.Keys.Count - 1 ] );

            materialAttenuationCoefficient = ( m * averageParticleEnergy ) + b;

        }
        else { //Particle energy is somewhere in known particle energy range

            for ( int k = 1 ; k < attenData.Keys.Count - 2 ; k++ ) {

                if ( averageParticleEnergy >= keyList[ k ] && averageParticleEnergy <= keyList[ k + 1 ] ) {

                    //y = mx + b
                    float m = ( attenData[ keyList[ k + 1 ] ] - attenData[ keyList[ k ] ] ) / ( keyList[ k + 1 ] - keyList[ k ] );
                    float b = attenData[ keyList[ k ] ] - ( m * keyList[ k ] );

                    materialAttenuationCoefficient = ( m * averageParticleEnergy ) + b;

                    break;

                }


            }

        }

        if ( knownAttenuationCoefficients.Count < 50 ) {

            if ( !knownAttenuationCoefficients.ContainsKey( materialName ) ) {

                knownAttenuationCoefficients[ materialName ] = new Dictionary<float , float>();

            }

            knownAttenuationCoefficients[ materialName ][ averageParticleEnergy ] = materialAttenuationCoefficient;

        }

        return materialAttenuationCoefficient;

    }

    private List<Isotope> recurseDecayChain( ref List<Isotope> decayChain , Isotope parent , int round ) {

        if ( round > 10 ) {
            
            Debug.Log( "ITS TIME TO STOP" );
            Debug.Break();

        }
        else {

            decayChain.Add( parent );

            if ( parent.getDecayProducts().Count != 0 ) {

                foreach ( Isotope daughter in parent.getDecayProducts() ) {

                    recurseDecayChain( ref decayChain , daughter , round + 1 );

                }

            }

        }

        return decayChain;

    }

   

    //Will return 2 points if line intersects box, will return 1 point if it 'knicks' the box, or return 0 points. Well the array length will always be 2 but they're just filled with zero vectors
    private Vector3[] lineShieldIntersection(Vector3 origin , Vector3 destination , Shield shield ) {

        RaycastHit hitA;
        RaycastHit hitB;
        Vector3 startA = Vector3.zero;
        Vector3 startB = Vector3.zero;


        Collider collider = shield.getCollider();

        //Checks to see if the source hit one face
        if ( collider.Raycast(new Ray(origin , ( ( origin - destination ) * -1f ).normalized) , out hitA , 1000f) ) {
            
            if ( hitA.transform.name == shield.name ) {

                startA = hitA.point;

                //Now that we get a hit, we work backwards to get another ray cast, from the target to the source
                if ( collider.Raycast(new Ray(destination , ( ( destination - origin ) * -1f ).normalized) , out hitB , 1000f) ) {
                    
                    if ( hitB.transform.name == shield.name ) {

                        startB = hitB.point;

                    }

                }

            }

        }


        Vector3[] points = new Vector3[2];

        points[0] = startA;
        points[1] = startB;


        return points;

    }

    

    private float materialAttenuate(float initialAcitvity , float attenuationConstant , float distance) {

        return initialAcitvity * Mathf.Exp(( float )( -attenuationConstant * distance ));


    }

    private int partition( ref List<Shield> shields , Vector3 origin , int low , int high ) {

        float pivot = ( shields[ high ].transform.position - origin ).magnitude;

        // index of smaller element 
        int i = ( low - 1 );
        for ( int j = low ; j < high ; j++ ) {
            // If current element is smaller  
            // than or equal to pivot 
            if ( ( shields[ j ].transform.position - origin ).magnitude <= pivot ) {
                i++;

                // swap arr[i] and arr[j] 
                Shield tempShield = shields[ i ];
                shields[ i ] = shields[ j ];
                shields[ j ] = tempShield;

            }
        }

        // swap arr[i+1] and arr[high] (or pivot) 
        Shield tempShield2 = shields[ i + 1 ];
        shields[ i + 1 ] = shields[ high ];
        shields[ high ] = tempShield2;
        
        return i + 1;

    }

    
    private void quickSort( ref List< Shield > shields , Vector3 origin , int low , int high ) {

        if ( low < high ) {

            /* pi is partitioning index, arr[pi] is  
            now at right place */
            int pi = partition( ref shields , origin , low , high );

            // Recursively sort elements before 
            // partition and after partition 
            quickSort( ref shields , origin , low , pi - 1 );
            quickSort( ref shields , origin , pi + 1 , high );

        }
        
    }

    public List<Shield> sortShields( List<Shield> shields , Vector3 origin) {
        
        quickSort( ref shields , origin , 0 , shields.Count - 1 );
        

        return shields;
        

    }

    
    public List<Shield> getShields() {
        
        return listShields;

    }
    
    public List<DoseBody> getDoseBodies() {

        return listBodies;

    }
    
    public List<Source> getSources() {

        return listSources;
        

    }

    //Finds all game objects with a 'Source' component and returns a list of type Source
    public void updateListSources( GameObject[] allObjects ) {
        
        List<Source> sources = new List<Source>();

        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<Source>() != null ) {

                sources.Add( gameObject.GetComponent<Source>() );

            }

        }
        

    }

    //Finds all game objects with a 'Shield' component and returns a list of type Shield
    public void updateListShields( GameObject[] allObjects ) {
        
        List<Shield> shields = new List<Shield>();

        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<Shield>() != null ) {

                shields.Add(gameObject.GetComponent<Shield>());

            }

        }
        

    }

    //Finds all game objects with a 'DoseBody' component and returns a list of type DoseBody
    public void updateListBodies( GameObject[] allObjects ) {
        
        List<DoseBody> doseBodies = new List<DoseBody>();

        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<DoseBody>() != null ) {

                doseBodies.Add( gameObject.GetComponent<DoseBody>() );

            }

        }

        
        
    }

    private void readCSV() {

        string fileData = System.IO.File.ReadAllText("Assets/Julia/Attenuation Coefficients - All.csv");
        string[] lines = fileData.Split("\n".ToCharArray());

        if ( lines.Length > 0 ) {

            int n = lines[0].Split(",".ToCharArray()).Length;
            int j = lines.Length;

            string[,] rawData = new string[n , j];

            for ( int y = 0 ; y < lines.Length ; y++ ) {

                for ( int x = 0 ; x < lines[y].Split(",".ToCharArray()).Length ; x++ ) {

                    rawData[x , y] = lines[y].Split(",".ToCharArray())[x];

                }

            }

            

            for ( int y = 1 ; y < j ; y++ ) {

                if ( rawData[0 , y] != "" ) {

                    if ( !attenConstants.ContainsKey(rawData[0 , y]) ) {

                        attenConstants.Add(rawData[0 , y] , new Dictionary<float , float>());

                    }


                    if ( !attenConstants[rawData[0 , y]].ContainsKey(float.Parse(rawData[2 , y])) ) {

                        //2 is for column Energy (kEV)
                        //5 is for column Linear Attenuation Coeffficient(m-1)
                        attenConstants[rawData[0 , y]].Add(float.Parse(rawData[2 , y]) , float.Parse(rawData[5 , y]));
                   

                    }


                }

            }


        }


    }

    private void DrawLine(Vector3 start , Vector3 end , Color color , float duration = 1f) {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color , 1.0f) } ,
            new GradientAlphaKey[] {  new GradientAlphaKey(1 , 1.0f) }
        );

        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.colorGradient = gradient;
        lr.SetPosition(0 , start);
        lr.SetPosition(1 , end);
        GameObject.Destroy(myLine , duration);
    }

}
