using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoseController : MonoBehaviour {

    private Dictionary<string , Dictionary<float , float>> attenConstants = new Dictionary<string , Dictionary<float , float>>();
    private float airAttenuation = 1; //Please change and update later

    public bool debug;


    //This class is the controller for the dose system
    //Only one of these per scene

    // Use this for initialization
    void Start () {

        readCSV();

	}

    int t = 0;

	// Update is called once per frame
	void Update () {

        if ( t == 90 * 1 ) {
       
            calculateDose();
            t = 0;

        }

        t++;

	}



    private void calculateDose() {

        List<Source> sources = getSources();
        List<Shield> shields = getShields();
        List<DoseBody> doseBodies = getDoseBodies();

        foreach ( DoseBody body in doseBodies ) {

            float countRate = calculateCountRateForDoseBody(body , sources , shields);
            float doseRate = calculateDoseRateForDoseBody(body , countRate , sources , shields);
            
            body.setCountRate(countRate);
            body.setDoseRate(doseRate);

        }

    }

    private float calculateCountRateForDoseBody( DoseBody doseBody , List<Source> sources , List<Shield> shields) {

        float countRate = 0;
        
        foreach ( DoseReceptor doseReceptor in doseBody.getDoseReceptors() ) {

            foreach ( Source source in sources ) {

                float attenuatedActivity = source.getActivity();
                
                Vector3 origin = doseReceptor.getPosistion();

                //Sort shields
                shields = sortShields( shields , doseReceptor.getPosistion() );

                for ( int i = 0 ; i < shields.Count ; i++ ) {

                    Shield shield = shields[i];
                    
                    Vector3[] points = lineShieldIntersection( origin , source.getPosistion() , shield );

                    if ( points[0] != Vector3.zero && points[1] != Vector3.zero ) {
                        
                        //This is our thickness
                        float thickness = Vector3.Distance(points[0] , points[1]);

                        Vector3 closestPoint = points[0];
                        Vector3 furthestPoint = points[1];

                        if ( Vector3.Distance(origin , points[1]) < Vector3.Distance(origin  , points[0]) ) {

                            closestPoint = points[ 1 ];
                            furthestPoint = points[0 ];

                        }

                        float airAttenuationDistance = Vector3.Distance( origin , closestPoint);

                        attenuatedActivity = materialAttenuate( attenuatedActivity , airAttenuation , airAttenuationDistance );

                        if ( debug ) {

                            DrawLine( origin , closestPoint , Color.red);
                            DrawLine(closestPoint , furthestPoint , Color.green);

                        }


                        string assumed = "Concrete (Ordinary)";
                        string renderName = shield.getName();
       
                        if ( !attenConstants.ContainsKey(renderName) ) {

                            assumed = renderName;

                        }

                        if ( attenConstants.ContainsKey(assumed) ) {

                            Dictionary<float , float> attenData = attenConstants[assumed];

                            float materialAttenuationConstant = 10; //Concrete for 1000 keV, uses this if it cant find a useful energy

                            List<float> keyList = new List<float>(attenData.Keys);

                            if ( source.getParticleEnergy() < keyList[0] ) { //If the particle energy is below the lowest energy

                                //y = mx + b
                                float m = ( attenData[keyList[1]] - attenData[keyList[0]] ) / ( keyList[1] - keyList[0] );
                                float b = attenData[keyList[1]] - ( m * keyList[1] );

                                materialAttenuationConstant = ( m * source.getParticleEnergy() ) + b;

                            }
                            else if ( source.getParticleEnergy() > keyList[attenData.Keys.Count - 1] ) { //If the particle energy is larger than the highest particle energy

                                //y = mx + b
                                float m = ( attenData[keyList[attenData.Keys.Count - 2]] - attenData[keyList[attenData.Keys.Count - 1]] ) / ( keyList[attenData.Keys.Count - 2] - keyList[attenData.Keys.Count - 1] );
                                float b = attenData[keyList[attenData.Keys.Count - 1]] - ( m * keyList[attenData.Keys.Count - 1] );

                                materialAttenuationConstant = ( m * source.getParticleEnergy() ) + b;

                            }
                            else { //Particle energy is somewhere in known particle energy range

                                for ( int k = 1 ; k < attenData.Keys.Count - 2 ; k++ ) {

                                    if ( source.getParticleEnergy() >= keyList[k] && source.getParticleEnergy() <= keyList[k + 1] ) {

                                        //y = mx + b
                                        float m = ( attenData[keyList[k + 1]] - attenData[keyList[k]] ) / ( keyList[k + 1] - keyList[k] );
                                        float b = attenData[keyList[k]] - ( m * keyList[k] );

                                        materialAttenuationConstant = ( m * source.getParticleEnergy() ) + b;

                                        break;

                                    }


                                }

                            }

                            attenuatedActivity = materialAttenuate(attenuatedActivity , materialAttenuationConstant , thickness);

                        }

                        origin = furthestPoint;

                    }

                }

                if ( debug ) {

                    DrawLine(origin , source.getPosistion() , Color.blue);

                }

                attenuatedActivity = materialAttenuate(attenuatedActivity , airAttenuation , Vector3.Distance(origin , source.getPosistion() ) );

                countRate += attenuatedActivity;

            }

        }

        return countRate;

    }

    private float calculateDoseRateForDoseBody(DoseBody doseBody , double countRate , List<Source> sources , List<Shield> shields) {

        float doseRate = 0;



        return doseRate;

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

    private List<Shield> sortShields( List<Shield> shields , Vector3 origin) {

        float[] distances = new float[ shields.Count ];
        for ( int i = 0 ; i < shields.Count ; i++ ) {

            distances[i] = ( shields[i].transform.position - origin ).magnitude;

        }

        int n = shields.Count;

        for ( int i = 0 ; i < n - 1 ; i++ ) {

            for ( int j = 0 ; j < n - i - 1 ; j++ ) {

                if ( distances[j] > distances[j + 1] ) {

                    Shield tempObject;
                    float tempDistance;

                    tempDistance = distances[j];
                    distances[j] = distances[j + 1];
                    distances[j + 1] = tempDistance;


                    tempObject = shields[j];
                    shields[j] = shields[j + 1];
                    shields[j + 1] = tempObject;

                }

            }

        }

        return shields;

    }

    //Finds all game objects with a 'Source' component and returns a list of type Source
    private List<Source> getSources() {

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        List<Source> sources = new List<Source>();

        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<Source>() != null ) {

                sources.Add(gameObject.GetComponent<Source>());

            }

        }

        return sources;

    }

    //Finds all game objects with a 'Shield' component and returns a list of type Shield
    private List<Shield> getShields() {

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        List<Shield> shields = new List<Shield>();

        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<Shield>() != null ) {

                shields.Add(gameObject.GetComponent<Shield>());

            }

        }

        return shields;

    }

    //Finds all game objects with a 'DoseBody' component and returns a list of type DoseBody
    private List<DoseBody> getDoseBodies() {

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        List<DoseBody> doseBodies = new List<DoseBody>();

        foreach( GameObject gameObject in allObjects ){

            if ( gameObject.GetComponent<User>() != null ) {

                doseBodies.Add( gameObject.GetComponent<DoseBody>() );

            }

        }

        return doseBodies;

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
