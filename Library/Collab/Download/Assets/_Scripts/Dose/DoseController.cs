using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is my portal
//I know it contains every secret nuclear has to offer
//However it has also lead me astray
//I saw the truth that lies within it, and i became convinced I could solve everything with C#
//But I couldnt have possibly been more wrong, that was just arrogance
public class DoseController : MonoBehaviour {

    public bool applyCorrectionCode = true;

    private Dictionary<string , Dictionary<float , float>> knownAttenuationCoefficients = new Dictionary<string , Dictionary<float , float>>();
    private Dictionary<string , Dictionary<float , float>> attenConstants = new Dictionary<string , Dictionary<float , float>>();
    private float airAttenuation = 0.05f;

    private List<Shield> listShields = new List<Shield>();
    private List<DoseBody> listBodies = new List<DoseBody>();
    private List<Source> listSources = new List<Source>();
    
    public bool debug = false;

    public bool useMinhParticleSystem = true;

    private bool enableDoseSystem = true;


    private bool staggerCalculations = true;
    private float staggerTime = 0.05f;

    private int updateTickInterval = 8;
    private int lastGameObjectCount = -1;
    
    //This class is the controller for the dose system
    //Only one of these per scene

    // Use this for initialization
    void Start() {

        readCSV();

        if ( GameObject.Find( "ColumnBase" ) != null ) {

            createColumn();

        }


        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        updateListShields( allObjects );
        updateListBodies( allObjects );
        updateListSources( allObjects );


        //StartCoroutine( DoseCalculator() );

        updateParticleSystemChoice();
        

    }

    int t = 0;

    // Update is called once per frame
    void Update() {
        
        t++;


    }

    //I am a monument to all your sins
    IEnumerator DoseCalculator() {

        while ( true ) {

            yield return new WaitForSecondsRealtime( staggerCalculations ? staggerTime : 0 );

            if ( t > 90 *2 ) {

                yield return new WaitForSecondsRealtime( staggerCalculations ? staggerTime : 0 );

                List<Source> sources = getSources();
                List<Shield> shields = getShields();
                List<DoseBody> doseBodies = getDoseBodies();

                foreach ( DoseBody doseBody in doseBodies ) {

                    yield return new WaitForSecondsRealtime( staggerCalculations ? staggerTime : 0 );

                    if ( doseBody != null ) {

                        if ( doseBody.enabled ) {

                            float countRate = 0;
                            float doseRate = 0;

                            foreach ( DoseReceptor doseReceptor in doseBody.getDoseReceptors() ) {

                                yield return new WaitForSecondsRealtime( staggerCalculations ? staggerTime : 0 );

                                foreach ( Source source in sources ) {

                                    yield return new WaitForSecondsRealtime( staggerCalculations ? staggerTime : 0 );


                                    float totalDistance = ( source.getPosistion() - doseReceptor.getPosistion() ).magnitude;

                                    RaycastHit[] forwardHits = Physics.RaycastAll( doseReceptor.getPosistion() , ( ( doseReceptor.getPosistion() - source.getPosistion() ) * -1f ).normalized , totalDistance );
                                    RaycastHit[] reverseHits = Physics.RaycastAll( source.getPosistion() , ( ( source.getPosistion() - doseReceptor.getPosistion() ) * -1f ).normalized , totalDistance );
                                
                                    Dictionary<string , float> shieldThicknesses = new Dictionary<string , float>();
                                    
                                    for ( int i = 0 ; i < forwardHits.Length ; i++ ) {
                                        
                                        RaycastHit pointEnter = forwardHits[ i ];
                                        Shield shieldEnter = pointEnter.transform.gameObject.GetComponent<Shield>();

                                        if ( shieldEnter != null ) {
                                            
                                            float maxDistance = totalDistance;
                                            bool foundCounter = false;

                                            for ( int j = 0 ; j < reverseHits.Length ; j++ ) {
                                                
                                                RaycastHit pointExit = reverseHits[ j ];
                                                Shield shieldExit = pointExit.transform.gameObject.GetComponent<Shield>();
                                                
                                                if ( shieldExit != null && GameObject.ReferenceEquals( pointEnter.transform.gameObject , pointExit.transform.gameObject ) ) {
                                                    
                                                    float distance = ( pointEnter.point - pointExit.point ).magnitude;
                                                    
                                                    if ( distance < maxDistance && distance != 0 ) { //I dont know how distance would be 0 but here we are
                                                        
                                                        foundCounter = true;
                                                        maxDistance = distance;

                                                    }

                                                }

                                            }

                                            if ( foundCounter ) {
                                                
                                                string shieldMaterial = pointEnter.transform.gameObject.GetComponent<Shield>().getName();

                                                if ( !shieldThicknesses.ContainsKey( shieldMaterial ) ) {

                                                    shieldThicknesses[ shieldMaterial ] = 0f;
                                                    
                                                }
                                                
                                                shieldThicknesses[ shieldMaterial ] = shieldThicknesses[ shieldMaterial ] + maxDistance;
                                                totalDistance -= maxDistance;

                                            }
                                            
                                        
                                        }

                                    }

                                    List<Isotope> decayChain = new List<Isotope>();

                                    foreach ( Isotope isotope in source.getIsotopes() ) {

                                        if ( source.enableComposistionEvolution ) {

                                            recurseDecayChain( ref decayChain , isotope , 0 );

                                        }
                                        else {

                                            decayChain.Add( isotope );

                                        }

                                    }
                                    

                                    //Now we go through each isotope
                                    foreach ( Isotope isotope in decayChain ) {

                                        if ( isotope.getIsotopeName() != null ) {
                                            
                                            float isotopeActivity = source.getActivity( isotope.getIsotopeName() );
                                            float averageParticleEnergy = isotope.getGammaDecayEnergy() + isotope.getBetaDecayEnergy();
                                            
                                            //Attenuate due to distance distance
                                            isotopeActivity = ( ( isotopeActivity ) / ( 4 * Mathf.PI * Mathf.Pow( ( doseReceptor.getPosistion() - source.getPosistion() ).magnitude , 2 ) ) ) * doseReceptor.getSurfaceArea();


                                            if ( isotopeActivity > 0 ) { //No point in doing all of this if it has already been completely attenuated

                                                isotopeActivity = materialAttenuate( isotopeActivity , airAttenuation , totalDistance ); //Attenuation due to air
                                                
                                                //Attenuation due to shielding
                                                foreach ( string materialName in shieldThicknesses.Keys ) {

                                                    float attenuationCoefficient = getMaterialAttenuationCoefficient( materialName , averageParticleEnergy );

                                                    //Debug.Log( isotopeActivity + ">" + materialAttenuate( isotopeActivity , attenuationCoefficient , shieldThicknesses[ materialName ] ) + " " + materialName + "!" + shieldThicknesses[ materialName ] );

                                                    isotopeActivity = materialAttenuate( isotopeActivity , attenuationCoefficient , shieldThicknesses[ materialName ] );
                                                    
                                                    
                                                }
                                                
                                                if ( isotopeActivity > 0 ) {

                                                    countRate += isotopeActivity;

                                                    //Dose rate
                                                    //averageActivity * particleEnergies[ i ] yields keV/s
                                                    //keV/s / 6241506479963235 yields j/s, conversion factor
                                                    //Dividing that by weight yields j/kg*s, and a Sv=j/kg
                                                    doseRate += ( isotopeActivity * averageParticleEnergy * 1000 * 3600 ) / ( 6241506479963235 * doseReceptor.getMass() ); //Yields mSv/hr

                                                }

                                            }

                                        }
                                        else {}


                                    } 
                                    

                                }


                            }

                            doseBody.setCountRate( countRate );
                            doseBody.setDoseRate( doseRate );

                        }

                    }

                }

                t = 0;

            }
            

        }

    }
    

    public float getAverageMaterialAttenuationCoefficient( string materialName ) {

        return getMaterialAttenuationCoefficient(materialName , 1000); //1000 keV

    }

    public float getMaterialAttenuationCoefficient( string materialName , float averageParticleEnergy ) {

        if ( knownAttenuationCoefficients.ContainsKey( materialName ) ) {

            if ( knownAttenuationCoefficients[ materialName ].ContainsKey( averageParticleEnergy ) ) {
                
                return knownAttenuationCoefficients[ materialName ][ averageParticleEnergy ];

            }

        }

        if ( !attenConstants.ContainsKey( materialName ) ) {

            Debug.Log( materialName );

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
                        
                        //collider.gameObject.GetComponent<Renderer>().material.color = Color.green;
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
    private void updateListSources( GameObject[] allObjects ) {
        
        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<Source>() != null ) {

                addSource( gameObject );

            }

        }
        
    }

    //Finds all game objects with a 'Shield' component and returns a list of type Shield
    private void updateListShields( GameObject[] allObjects ) {
        
        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<Shield>() != null ) {

                addShield( gameObject );

            }

        }

    }

    //Finds all game objects with a 'DoseBody' component and returns a list of type DoseBody
    private void updateListBodies( GameObject[] allObjects ) {
        
        foreach ( GameObject gameObject in allObjects ) {
            
            if ( gameObject.GetComponent<DoseBody>() != null ) {

                addDoseBody( gameObject );

            }

        }

        
    }

    public void addSource( GameObject source ) {

        if ( !listSources.Contains( source.GetComponent<Source>() ) ) {

            listSources.Add( source.GetComponent<Source>() );

        }


    }

    public void addDoseBody( GameObject doseBody ) {

        if ( !listBodies.Contains( doseBody.GetComponent<DoseBody>() ) ) {

            listBodies.Add( doseBody.GetComponent<DoseBody>() );

        }

    }

    public void addShield( GameObject shield ) {

        if ( !listShields.Contains( shield.GetComponent<Shield>() ) ) {

            listShields.Add( shield.GetComponent<Shield>() );

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            foreach ( GameObject gameObject in allObjects ) {

                if ( gameObject.GetComponent<PSOnTrigger_Minh>() != null ) {

                    gameObject.GetComponent<PSOnTrigger_Minh>().reloadList();

                }

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

    private void createColumn() {

        Transform transform = GameObject.Find( "ColumnBase" ).transform;

        List<GameObject> generatedObjects = new List<GameObject>();

        float mainThickness = 0.03f;
        float maxRadius = 0.2f;

        for ( int c = 0 ; c < 2 ; c++ ) {

            Vector3 offset = new Vector3( c == 1 ? 0.038f : 0 , 0 , 0 );
        
            float thickness = new float[]{ mainThickness , 0.01f }[ c ];
            float radius = new float[] { maxRadius - ( mainThickness / 2 ) , 0.15f / 2 }[ c ]; 
            float height = new float[] { 0.2435f * 2 , 0.065f }[ c ];

            int n = new int[] { 18 , 18 }[ c ];
            int heightSplit = new int[] { 24 , 1 }[ c ];

            float heightInterval = height / ( float ) heightSplit;

            float theta = 360 / n;
            float length = ( radius + ( thickness / 2 ) ) * Mathf.Tan( Mathf.Deg2Rad * ( theta / 2 ) ) * 2;

            if ( c == 1 ) {

                offset = offset + new Vector3( -0.272f , 0.054f , 0 );

            }

            GameObject parentObject = new GameObject();
            parentObject.transform.parent = transform;
            parentObject.transform.localPosition = new Vector3( 0 , 0 , 0 ) + offset;
            parentObject.transform.localScale = new Vector3( 1 , 1 , 1 );


            MeshCollider parentMeshCollider = parentObject.AddComponent<MeshCollider>();
            MeshFilter parentFilter = parentObject.AddComponent<MeshFilter>();
            MeshRenderer parentRenderer = parentObject.AddComponent<MeshRenderer>();


            List<MeshFilter> filters = new List<MeshFilter>();

            for ( int h = 0 ; h < heightSplit ; h++ ) {

                for ( int i = 0 ; i < n ; i++ ) {

                    bool ignore = ( i == 9 ) && ( h >= 13 && h <= 16 ) && c == 0;

                    if ( !ignore ) {

                        float x = radius * Mathf.Cos( Mathf.Deg2Rad * ( theta * i ) );
                        float y = radius * Mathf.Sin( Mathf.Deg2Rad * ( theta * i ) );

                        GameObject cube = GameObject.CreatePrimitive( PrimitiveType.Cube );
        
                        cube.transform.SetParent(  parentObject.transform , false );
                        
                        cube.transform.localScale = new Vector3( length , thickness , heightInterval );
                        cube.transform.localPosition = new Vector3( x , -( height / 2 ) + ( heightInterval * h ) + ( heightInterval / 2 ) , y );
                        cube.transform.localEulerAngles = new Vector3( 90 , theta * -i + ( 90 ) , 0 );

                        if ( cube.GetComponent<BoxCollider>() !=null ) {

                            Destroy( cube.GetComponent<BoxCollider>() );

                        }
                        
                        filters.Add( cube.GetComponent<MeshFilter>() );


                    }

                }

            }

            if ( c == 1 ) {

                parentObject.transform.rotation *= Quaternion.Euler( 0 , 0 , 90 );

            }

            Transform masterTransform = GameObject.Find( "GammaGun" ).transform;
            
            CombineInstance[] combine = new CombineInstance[ filters.Count ];
            for ( int i = 0 ; i < combine.Length ; i++ ) {

                combine[ i ].mesh = filters[ i ].sharedMesh;
                

                Vector3 local = filters[ i ].transform.localPosition;
                Quaternion localAngle = filters[ i ].transform.localRotation;

                filters[ i ].transform.localPosition = filters[ i ].transform.position;
                filters[ i ].transform.localRotation = filters[ i ].transform.rotation;
                filters[ i ].transform.position = local;
                filters[ i ].transform.rotation = localAngle;
                
                combine[ i ].transform = ( filters[ i ].transform ).localToWorldMatrix;
                filters[ i ].gameObject.SetActive( false );


            }


            parentFilter.mesh = new Mesh();

            parentFilter.mesh.CombineMeshes( combine , true );

            parentFilter.sharedMesh = parentFilter.mesh;
            parentMeshCollider.sharedMesh = parentFilter.mesh;
            //parentMeshCollider.gameObject.GetComponent<Renderer>().enabled = false;

            parentFilter.gameObject.SetActive( true );
            
            generatedObjects.Add( parentObject );
            
        }

        GameObject scaleMatch = GameObject.Find( "Base-cylinder" );

        float rad = scaleMatch.transform.localScale.x;

        for ( int c = 0 ; c < 2 ; c++ ) {

            GameObject endShields = GameObject.CreatePrimitive( PrimitiveType.Cylinder );
            endShields.transform.parent = transform;
            endShields.transform.localPosition = new Vector3( 0 , ( ( scaleMatch.transform.localScale.y / 1 ) - ( mainThickness / 2 ) ) * ( c == 1 ? -1 : 1 ) , 0 );
            endShields.transform.localScale = new Vector3( rad , mainThickness / 2 , rad );

            generatedObjects.Add( endShields );

        }

        CombineInstance[] combineSuper = new CombineInstance[ generatedObjects.Count ];


        for ( int i = 0 ; i < combineSuper.Length ; i++ ) {

            combineSuper[ i ].mesh = generatedObjects[ i ].GetComponent<MeshFilter>().sharedMesh;


            Vector3 local = generatedObjects[ i ].transform.localPosition;
            Quaternion localAngle = generatedObjects[ i ].transform.localRotation;

            generatedObjects[ i ].transform.localPosition = generatedObjects[ i ].transform.position;
            generatedObjects[ i ].transform.localRotation = generatedObjects[ i ].transform.rotation;
            generatedObjects[ i ].transform.position = local;
            generatedObjects[ i ].transform.rotation = localAngle;

            combineSuper[ i ].transform = ( generatedObjects[ i ].transform ).localToWorldMatrix;
            generatedObjects[ i ].gameObject.SetActive( false );
            
        }

        GameObject grandObject = new GameObject();

        grandObject.transform.parent = transform;
        grandObject.transform.localPosition = new Vector3( 0 , 0 , 0 );
        grandObject.transform.localScale = new Vector3( 1 , 1 , 1 );

        MeshCollider grandMeshCollider = grandObject.AddComponent<MeshCollider>();
        MeshFilter grandFilter = grandObject.AddComponent<MeshFilter>();
        MeshRenderer grandRenderer = grandObject.AddComponent<MeshRenderer>();

        grandFilter.mesh = new Mesh();

        grandFilter.mesh.CombineMeshes( combineSuper , true );

        grandFilter.sharedMesh = grandFilter.mesh;
        grandMeshCollider.sharedMesh = grandFilter.mesh;
        grandMeshCollider.gameObject.GetComponent<Renderer>().enabled = false;

        grandFilter.gameObject.SetActive( true );

        generatedObjects.Add( grandObject );

        Shield cubeShield = grandObject.AddComponent<Shield>();
        cubeShield.nom = "Lead";
        //cubeShield.averageThickness = mainThickness;

        grandObject.tag = "Barrier";

        grandObject.name = "Gamma gun mega shield"; //Therefore my people shall know my name: therefore they shall know in that day that I am he that doth speak: behold, it is I.

        addShield( grandObject );


    }


    void OnValidate() {

        updateParticleSystemChoice();

    }

    private void updateParticleSystemChoice() {

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach ( GameObject gameObject in allObjects ) {

            if (gameObject.GetComponent<PSOnTrigger_Minh>() != null ) {

                gameObject.GetComponent<PSOnTrigger_Minh>().enabled = useMinhParticleSystem;

            }

            if ( gameObject.GetComponent<ParticleTrigger>() != null ) {

                gameObject.GetComponent<ParticleTrigger>().enabled = !useMinhParticleSystem;

            }

        }


    }

}
