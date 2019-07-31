using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GammaGunController : MonoBehaviour {

    private GameObject pin;
    private GameObject pinOuter;

    private bool[] isAnimating;
    private bool[] startAnimation;
    private bool[] open;

    private GameObject[] caps;

    private int tick = 0;
    private float rotationRate = 20f; //Deg/s

    private float[] startRoll;
    private float[] endRoll;

    private float endAngle = 120f;

    public AudioClip moveClip;
    public AudioClip stopClip;
    
    public List<GammaShield> gammaShields = new List<GammaShield>();

    private List<float[]> pointData = new List<float[]>();

    private int select = 0;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {

        pin = GameObject.Find( "Pin" );
        pinOuter = GameObject.Find( "Pin-Outer" );

        audioSource = GetComponent<AudioSource>();

        isAnimating = new bool[ gammaShields.Count ];
        startAnimation = new bool[ gammaShields.Count ];
        open = new bool[ gammaShields.Count ];

        startRoll = new float[ gammaShields.Count ];
        endRoll = new float[ gammaShields.Count ];

        caps = new GameObject[ gammaShields.Count ];

        //startAnimation[ 0 ] = true;

        float totalThickness = 0f;

        if ( gammaShields.Count > 0 ) {

            GameObject baseObject = GameObject.Find( "GammaGunColumn" );

            float conversionFactor = 2 / baseObject.transform.localScale.y;

            float xCount = 0f; // ( this.transform.localScale.y ) - ( gammaShields[ 0 ].thickness * 0.5f * conversionFactor )

            for ( int i = 0 ; i < gammaShields.Count ; i++ ) {

                GameObject cylinder = GameObject.CreatePrimitive( PrimitiveType.Cylinder );
                cylinder.name = "Sub-shield " + ( i + 1 );

                float actualHeight = gammaShields[ i ].thickness * conversionFactor;

                cylinder.transform.parent = baseObject.transform;
                cylinder.transform.localScale = new Vector3( 1f , ( actualHeight * 0.5f ) , 1f );
                cylinder.transform.localRotation = Quaternion.Euler( new Vector3( 0 , 0 , 0 ) );

                cylinder.transform.localPosition = new Vector3( 0 , xCount + ( actualHeight * 0.5f ) + 1 , 0 );

                xCount += ( actualHeight * 1f );
                totalThickness += actualHeight * 0.5f;

                cylinder.GetComponent<Renderer>().material = baseObject.GetComponent<Renderer>().material;

                Destroy( cylinder.GetComponent<CapsuleCollider>() );

                cylinder.AddComponent<MeshCollider>();

                Shield shield = cylinder.AddComponent<Shield>();
                shield.shield = gammaShields[ i ].types;

                Controller.getController().getDoseController().addShield( cylinder );

                caps[ i ] = cylinder;

                startRoll[ i ] = caps[ i ].transform.localRotation.eulerAngles.y;
                endRoll[ i ] = endAngle;

            }

        }

        GameObject basePin = GameObject.Find( "Pin-Base" );
        float adjustX = totalThickness + 1;

        pinOuter.transform.localPosition = new Vector3( -adjustX , 0 , 0 );
        pinOuter.transform.localScale = new Vector3( 1 , totalThickness , 1 );

        pin.transform.localPosition = new Vector3( -adjustX - 0.1f , 0 , 0 );
        pin.transform.localScale = new Vector3( 0.8f , totalThickness , 0.8f );
        

    }

    // Update is called once per frame
    void Update() {

        for ( int i = 0 ; i < gammaShields.Count ; i++ ) {

            if ( startAnimation[ i ] ) {

                startAnimation[ i ] = false;
                isAnimating[ i ] = true;
                open[ i ] = !open[ i ];

                audioSource.clip = moveClip;
                audioSource.Play();

            }

            if ( isAnimating[ i ] ) {

                caps[ i ].transform.RotateAround( pin.transform.position , new Vector3( open[ i ] ? 1 : -1 , 0 , 0 ) , rotationRate * Time.deltaTime );

                float roll = 360 - caps[ i ].transform.localRotation.eulerAngles.y;
                
                if ( open[ i ] ) {
                    
                    if ( roll - startRoll[ i ] >= endRoll[ i ] ) {

                        if ( roll - startRoll[ i ] != endRoll[ i ] ) {

                            caps[ i ].transform.RotateAround( pin.transform.position , new Vector3( open[ i ] ? 1 : -1 , 0 , 0 ) , endRoll[ i ] - ( roll - startRoll[ i ] ) );

                        }

                        isAnimating[ i ] = false;
                        audioSource.clip = stopClip;
                        audioSource.Play();
                        //startAnimation = true;

                    }


                }
                else {

                    if ( roll - startRoll[ i ] <= 0 || roll - startRoll[ i ] > ( endRoll[ i ] + rotationRate ) ) {

                        if ( roll - startRoll[ i ] != 0 ) {

                            caps[ i ].transform.RotateAround( pin.transform.position , new Vector3( open[ i ] ? 1 : -1 , 0 , 0 ) , ( roll - startRoll[ i ] ) );

                        }

                        isAnimating[ i ] = false;
                        audioSource.clip = stopClip;
                        audioSource.Play();


                    }

                }

            }

        }
        
    }

    public void recordPoint() {

        //Distance, CPS
        float[] dataPoint = new float[] { Controller.getController().getPlatformMover().getDistanceBetweenSourceEtDetector() , GameObject.Find( "GeigerTool (GammaGun)" ).GetComponent<GeigerController>().getCountRate() };
        pointData.Add( dataPoint );


    }

    public void exportData() {

        string csvData = "Distance (m), Count rate (CPS) \n";

        for ( int i = 0 ; i < pointData.Count ; i++ ) {

            csvData += pointData[ i ][ 0 ] + "," + pointData[ i ][ 1 ] + "\n";

        }


        File.WriteAllText( System.Environment.GetFolderPath( System.Environment.SpecialFolder.Desktop ) + "/data.csv" , csvData );
        

    }

    public List<GammaShield> getGammaShields(){

        return gammaShields;

    }

    public void toggleCap( int index ) {

        startAnimation[ index ] = true;

    }

    public void toggleAll() {

        int opened = 0;

        for ( int i = 0 ; i < gammaShields.Count ; i++ ) {

            if ( open[ i ] ) {

                opened++;

            }

        }

        bool openAll = opened < gammaShields.Count;

        for ( int i = 0 ; i < gammaShields.Count ; i++ ) {

            if ( open[ i ] != openAll ) {

               toggleCap( i );

            }

        }

    }

    public void decrementSelect() {

        if ( select - 1 >= 0 ) {

            select--;

        }

    }

    public void incrementSelect() {

        if ( select + 1 < gammaShields.Count ) {

            select++;

        }

    }

    public int getSelected() {

        return select;

    }

    

}
