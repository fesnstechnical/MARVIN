using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class source : MonoBehaviour {

    private double airAttenuation = 0.001; //In inverse meters, 1 is made up someone change it

    private ParticleSystem sourceParticles;
    private Light sourceLight;
    
    private bool[] particleTypes = new bool[ 3 ]; //If the particle emits a particle then its index will be true. Order: Alpha, Beta, Gamma
    private double[] particleEnergies = new double[ 3 ]; //In keV

    public string radioNuke = "";
    public Color gammaColour;
    public Color betaColour;

    public double sourceActivity; //in mCi
    private double weight = 50; //Kg and it's a place holder

    private Boolean debug = false;

	private Dictionary<String,Dictionary<float,float>> attenConstants = new Dictionary<String,Dictionary<float,float>>();

    // Use this for initialization
    void Start () {

        readCSV();
       
        sourceParticles = GetComponentInChildren<ParticleSystem>();
        sourceLight = GetComponentInChildren<Light>();

        //Init arrays
        for ( int i = 0 ; i < particleTypes.Length ; i++ ) {

            particleTypes[i] = false;
            particleEnergies[i] = 0;

        }

        switch( radioNuke ) {

            case "Co-60":

                particleTypes[2] = true;
                particleEnergies[2] = 1332.5;

                particleTypes[1] = true;
                particleEnergies[1] = 1491.3;

                break;

            case "Cs-137":

                particleTypes[1] = true;
                particleEnergies[1] = 1175.62;

                particleTypes[2] = true;
                particleEnergies[2] = 661.66;

                break;

        }

        sourceParticles.enableEmission = false;
        sourceLight.enabled = false;

        if ( particleTypes[ 0 ] ) {

            sourceLight.enabled = true;
        
        }



        //Currently there's only one particle system so it can only display gamma or beta, rn gamma takes president
        if ( particleTypes[ 2 ] ) { 

            //Gamma
            //Travels at speed of light
            //Smallest

            sourceParticles.startSize = 0.01f; //Made up
            sourceParticles.startColor = gammaColour;

        }
        else if ( particleTypes[ 1 ] ) {

            //Beta
            //Electron
            //Bigger and faster but doesnt travel at speed of light

            sourceParticles.startSize = 0.05f; //Also made up
            sourceParticles.startColor = betaColour;

        }

        if ( particleTypes[1] || particleTypes[2] ) {

            sourceParticles.enableEmission = true;
            sourceParticles.Play();

        }

    }
	
	// Update is called once per frame
	void Update () {

        Vector3[] doseReceptors = findReceptorLocations();

        double activity = 0;

        //Check to see if array length is 0, we dont want to divide by 0 later on
        if ( doseReceptors.Length != 0 ) {


            float weight = 60f; //Kg
            float height = 1.6f; //Meters

            float surfaceArea = 0.007184f * Mathf.Pow(weight , 0.425f) * Mathf.Pow(height , 0.725f) / doseReceptors.Length; //In m^2


			double doseRate = 0;

            for ( int i = 0 ; i < particleTypes.Length ; i++ ) {

                if ( particleTypes[ i ] ) {

					for ( int k = 0 ; k < doseReceptors.Length ; k++ ) {

						activity += getAttenuatedActivity( sourceActivity * 37000000 , transform.position , doseReceptors[k] , ( float ) particleEnergies[ i ] ); //37000000 is mCi to Bq

						activity = ( ( activity ) / ( 4 * Math.PI * Math.Pow(( doseReceptors[k] - transform.position ).magnitude , 2) ) ) * surfaceArea;

					}

					double averageActivity = activity / doseReceptors.Length; //in units s^-1


                    //averageActivity * particleEnergies[ i ] yields keV/s
                    //keV/s / 6241506479963235 yields j/s, conversion factor
                    //Dividing that by weight yields j/kg*s, and a Sv=j/kg
                    doseRate += ( averageActivity * particleEnergies[ i ] * 1000 * 3600 ) / ( 6241506479963235 * weight ); //Yields mSv/hr
            
                }

            }

            if ( doseRate < 0 ) {

                doseRate = 0;

            }
            
            updateControllerDoseRate(doseRate);

        }

    }

 
    

    //Will return 2 points if line intersects box, will return 1 point if it 'knicks' the box, or return 0 points. Well the array length will always be 2 but they're just filled with zero vectors
    private Vector3[] lineBoxIntersection(Vector3 origin , Vector3 destination , GameObject gameObject ) {
       
        RaycastHit hitA;
        RaycastHit hitB;
        Vector3 startA = Vector3.zero;
        Vector3 startB = Vector3.zero;

  
        Collider collider = gameObject.GetComponent<Collider>();

        //Checks to see if the source hit one face
        if ( collider.Raycast( new Ray( origin , ( ( origin - destination ) * -1f ).normalized ) , out hitA , 1000f ) ) {

            if ( hitA.transform.name == gameObject.name ) {

                startA = hitA.point;

                //Now that we get a hit, we work backwards to get another ray cast, from the target to the source
                if ( collider.Raycast( new Ray( destination , ( ( destination - origin ) * -1f ).normalized ) , out hitB , 1000f ) ) {

                    if ( hitB.transform.name == gameObject.name ) {

                        startB = hitB.point;

                    }

                }

            }

        }


        Vector3[] points = new Vector3[ 2 ];
        
        points[0] = startA;
        points[1] = startB;
        
        
        return points;

    }

    private double attenuate( double initialAcitvity , double attenuationConstant , double distance) {
        
        return initialAcitvity * Mathf.Exp( ( float )( -attenuationConstant * distance ) );


    }

    private GameObject[] sortShields( GameObject[] shields , Vector3 origin) {

        float[] distances = new float[shields.Length];
        for ( int i = 0 ; i < shields.Length ; i++ ) {

            distances[i] = ( shields[i].transform.position - origin ).magnitude;
            
        }

        int n = shields.Length;

        for ( int i = 0 ; i < n - 1 ; i++ ) {

            for ( int j = 0 ; j < n - i - 1 ; j++ ) {

                if ( distances[ j ] > distances[ j + 1 ] ) {

                    GameObject tempObject;
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


	/**
	 * 
	 * particleEnergy is the energy of the radiation packet, in keV
	 * */
	private double getAttenuatedActivity( double initialActivity , Vector3 origin , Vector3 destination , float particleEnergy ) {

        double attenuatedActivity = initialActivity;

        GameObject[] shields = GameObject.FindGameObjectsWithTag( "Shielding" );
        GameObject shield;

        //Sort shields
        shields = sortShields(shields , origin);

        for ( int i = 0 ; i < shields.Length ; i++ ) {

            shield = shields[i];

            Vector3[] points = lineBoxIntersection(origin , destination , shield );
            
            if ( points[0] != Vector3.zero && points[1] != Vector3.zero ) {
                
                //This is our thickness
                float thickness = Vector3.Distance(points[0] , points[1]);

                Vector3 closestPoint = points[0];
                Vector3 furthestPoint = points[1];

                if ( Vector3.Distance( origin , points[ 1 ] ) < Vector3.Distance(origin , points[0]) ) {

                    closestPoint = points[1];
                    furthestPoint = points[0];

                }

                float airAttenuationDistance = Vector3.Distance(origin , closestPoint);

                attenuatedActivity = attenuate(attenuatedActivity , airAttenuation , airAttenuationDistance);

                if ( debug ) {

                    DrawLine(origin , closestPoint , Color.red);
                    DrawLine(closestPoint , furthestPoint , Color.green);

                }
                
                
				string assumed = "Concrete (Ordinary)";
				string renderName = shield.GetComponent<Renderer> ().material.name;
                renderName = renderName.Replace( " (Instance)" , "" );

                if (!attenConstants.ContainsKey (renderName)) {

					assumed = renderName;

				}

                if (attenConstants.ContainsKey (assumed)) {

					Dictionary<float,float> attenData = attenConstants [assumed];

					float materialAttenuationConstant = 10; //Concrete for 1000 keV, uses this if it cant find a useful energy

                    List<float> keyList = new List<float>(attenData.Keys);

					if ( particleEnergy < keyList[ 0 ] ) { //If the particle energy is below the lowest energy

                        //y = mx + b
                        float m = ( attenData[keyList[1]] - attenData[keyList[0]] ) / ( keyList[1] - keyList[0] );
                        float b = attenData[keyList[1]] - ( m * keyList[1] );

                        materialAttenuationConstant = ( m * particleEnergy ) + b;

                    }
                    else if ( particleEnergy > keyList[ attenData.Keys.Count - 1 ] ) { //If the particle energy is larger than the highest particle energy

                        //y = mx + b
                        float m = ( attenData[keyList[attenData.Keys.Count - 2]] - attenData[keyList[attenData.Keys.Count - 1]] ) / ( keyList[attenData.Keys.Count - 2] - keyList[attenData.Keys.Count - 1] );
                        float b = attenData[keyList[attenData.Keys.Count - 1]] - ( m * keyList[attenData.Keys.Count - 1] );

                        materialAttenuationConstant = ( m * particleEnergy ) + b;

                    }
                    else { //Particle energy is somewhere in known particle energy range

                        for ( int k = 1 ; k < attenData.Keys.Count - 2 ; k++ ) {

                            if ( particleEnergy>= keyList[ k ] && particleEnergy <= keyList[ k + 1 ] ) {

                                //y = mx + b
                                float m =  ( attenData[keyList[k + 1] ]- attenData[keyList[k]] ) / ( keyList[k + 1] - keyList[k] );
                                float b = attenData[keyList[k]] - ( m * keyList[k] );

                                materialAttenuationConstant = ( m * particleEnergy ) + b;

                                break;

                            }


                        }

                    }
                    
					attenuatedActivity = attenuate(attenuatedActivity, materialAttenuationConstant , thickness);

				}

                origin = furthestPoint;
            
            }

        }

        if ( debug ) {

            DrawLine(origin , destination , Color.blue);

        }

        attenuatedActivity = attenuate(attenuatedActivity , airAttenuation , Vector3.Distance(origin , destination));
        
        return attenuatedActivity;

    }

    //Get's posistion of game objects tagged with 'DoseReceptor'
    private Vector3[] findReceptorLocations() {

        GameObject[] doseReceptors = GameObject.FindGameObjectsWithTag("DoseReceptor");

        Vector3[] points = new Vector3[doseReceptors.Length];

        for ( int i = 0 ; i < points.Length ; i++ ) {

            points[i] = doseReceptors[i].transform.position;
            
        }

        return points;

    }

    private TextMesh textMesh = null;
    private double lastDoseRate = 0;

    int updateClicks = 40;
    int updateClick = 0;

    private void updateControllerDoseRate( double doseRate ) {


        //Only update if dose has changed
        if ( doseRate != lastDoseRate && updateClick > updateClicks ) {

            //If its null find the text mesh in the game
            if ( textMesh == null ) {

                textMesh = GameObject.Find("Controller text").GetComponent<TextMesh>(); //Name of plane on controller

            }

            if ( textMesh != null ) {

                textMesh.text = Math.Round(doseRate , 2) + "\n" + "mSv/h"; //Rounds to two decimals and updates text

            }

            doseRate = lastDoseRate;
            updateClick = 0;

        }

        updateClick++;

    }

    void DrawLine(Vector3 start , Vector3 end , Color color , float duration = 0.052f) {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color , color);
        lr.SetWidth(0.01f , 0.01f);
        lr.SetPosition(0 , start);
        lr.SetPosition(1 , end);
        GameObject.Destroy(myLine , duration);
    }

    private void readCSV() {

		String fileData = System.IO.File.ReadAllText("Assets/Julia/Attenuation Coefficients - All.csv");
        String[] lines = fileData.Split("\n".ToCharArray());
        
        if ( lines.Length > 0 ) {

			int n = lines [0].Split (",".ToCharArray ()).Length;
			int j = lines.Length;

        	string[,] rawData = new string[ n , j ];

			for (int y = 0; y < lines.Length; y++) {

				for ( int x = 0 ; x < lines[ y ].Split(",".ToCharArray()).Length ; x++ ){

					rawData [x, y] = lines [y].Split (",".ToCharArray ()) [x];

				}

			}

            
            for (int y = 1; y < j; y++) {

                if (rawData [0, y] != "") {

					if (!attenConstants.ContainsKey (rawData [0, y])) {
                        
                        attenConstants.Add( rawData [0, y] , new Dictionary<float,float>() );

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

}
