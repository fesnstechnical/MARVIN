using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plume : MonoBehaviour {

    private GameObject[] particles;
    private float[] totalTime;
    private Vector3[] acceleration;
    private float[] deltaHeights;
    private bool[] initialMovement;

    public ThermalStratifications thermalStratification;
    public float windDirection = 0f;
    public float verticalWindVelocity = 10f;
    public float windSpeed = 10f;

    private float generationRate = 0.1f;
    private int maxParticles = 1000;
    private float maxDistance = 2.6f; 
    private float radius = 0.05f;
    private float speedFudgingNumber = 0.1f;
    
    private float runningTime = 0f;

    private float chimneySpread = 9; //Radially from center of stack
    private float chimneyDiameter = 3;
    private float chimneyHeight = 40;

    private float scale = 0.02f;

    public float rainSpeed = 0f; //m/s
    
    private float[] lateralIntensities = new float[] { 0.475f , 0.325f , 0.175f , 0.165f , 0.14f };
    private float[] verticalIntensities = new float[] { 0.35f , 0.125f , 0.065f , 0.05f , 0.015f };

    private GameObject baseObject;
    private GameObject field;
    private GameObject stack;

    private GameObject[,] sectorGameObjects;
    private GameObject[,,] sectorAirGameObjects;
    private float[,] relativeContamination;
    private float[,,] airContamination;
    private long[,,] sectorDeltaTimes;


    private float initialSectorSize = 0.1f;
    private float fieldSize = 3;
    private float sectorThickness = 0.05f;

    private float sourceActivityRate = 1000000f; //Bq/s, ie how many dunkaroos are we pumping into the air per second

    private float maxSectorHeight = 2f;
    private int sectorCountVertical = 10;

    private float timeAccelerationFactor = 3600;//1 second to 1 hour

    // Start is called before the first frame update
    void Start() {

        particles = new GameObject[ maxParticles ];
        totalTime = new float[ maxParticles ];
        acceleration = new Vector3[ maxParticles ];
        deltaHeights = new float[ maxParticles ];
        initialMovement = new bool[ maxParticles ];

        for ( int i = 0 ; i < particles.Length ; i++ ) {

            totalTime[ i ] = 0f;

        }

        baseObject = new GameObject( "BaseObject" );
        baseObject.transform.parent = this.transform;
        baseObject.transform.localScale = new Vector3( 1 , 1 , 1 );
        baseObject.transform.localPosition = new Vector3( 0 , 2.55f , 0 );

        field = GameObject.CreatePrimitive( PrimitiveType.Cube );
        field.transform.parent = baseObject.transform;
        field.transform.localScale = new Vector3( fieldSize , 0.1f , fieldSize );
        field.transform.localPosition = new Vector3( 0 , 0 , 0 );
        field.GetComponent<Renderer>().material.color = new Color( 0 , 1 , 0 );

        stack = GameObject.CreatePrimitive( PrimitiveType.Cylinder );
        stack.transform.parent = baseObject.transform;
        stack.transform.localScale = new Vector3( chimneyDiameter * scale , ( chimneyHeight / 2 ) * scale , chimneyDiameter * scale );
        stack.transform.localPosition = new Vector3( 0 , ( ( chimneyHeight / 2 ) * scale ) + ( field.transform.localScale.y / 2 ) , 0 );
        stack.GetComponent<Renderer>().material.color = new Color( 0.3f , 0.3f , 0.3f );

        //Create the sectors
        int sectorCount = ( int ) ( fieldSize / initialSectorSize );
        float sectorSize = fieldSize / sectorCount;

        sectorGameObjects = new GameObject[ sectorCount , sectorCount ];
        relativeContamination = new float[ sectorCount , sectorCount ];
        
        for ( int x = 0 ; x < sectorCount ; x++ ) {

            for ( int z = 0 ; z < sectorCount ; z++ ) {

                GameObject sector = GameObject.CreatePrimitive( PrimitiveType.Cube );
                sector.transform.parent = baseObject.transform;
                sector.transform.localScale = new Vector3( sectorSize , sectorThickness , sectorSize );
                sector.transform.localPosition = new Vector3( 0 , ( 0.1f + sectorThickness ) / 2 , 0 ) - new Vector3( fieldSize / 2 , 0 , fieldSize / 2 ) + new Vector3( x * sectorSize , 0 , z * sectorSize ) + new Vector3( sectorSize / 2 , 0 , sectorSize / 2 );
                sector.GetComponent<Renderer>().material.shader = Shader.Find( "Transparent/Diffuse" );
                sector.SetActive( false );

                sectorGameObjects[ x , z ] = sector; //Sector ZZ9 Plural Z Alpha

            }

        }

        //Calculate air concentration;
        sectorDeltaTimes = new long[ sectorCount , sectorCountVertical , sectorCount ];

        airContamination = new float[ sectorCount , sectorCountVertical , sectorCount ];
        sectorAirGameObjects = new GameObject[ sectorCount , sectorCountVertical , sectorCount ];

        float sectorHeight = maxSectorHeight / sectorCountVertical;
        float totalHeight = ( ( 3 * chimneyDiameter * verticalWindVelocity ) / windSpeed ) + chimneyHeight;

        long currentTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;

        for ( int xI = 0 ; xI < sectorCount ; xI++ ) {

            for ( int z = 0 ; z < sectorCount ; z++ ) {

                for ( int y = 0 ; y < sectorCountVertical ; y++ ) {


                    Vector3 relativePosistion = new Vector3( 0 , ( 0.1f + sectorHeight ) / 2 , 0 ) - new Vector3( fieldSize / 2 , 0 , fieldSize / 2 ) + new Vector3( xI * sectorSize , sectorHeight * y , z * sectorSize ) + new Vector3( sectorSize / 2 , 0 , sectorSize / 2 );

                    GameObject sector = GameObject.CreatePrimitive( PrimitiveType.Cube );
                    sector.transform.parent = baseObject.transform;
                    sector.transform.localScale = new Vector3( sectorSize , sectorHeight , sectorSize );
                    sector.transform.localPosition = relativePosistion;
                    
                    sector.GetComponent<Renderer>().material.shader = Shader.Find( "Transparent/Diffuse" );
                    sector.GetComponent<Renderer>().material.color = new Color( 0 , 0 , 0 , 0 );
                    sectorAirGameObjects[ xI , y , z ] = sector;
                    sector.SetActive( false );


                    sectorDeltaTimes[ xI , y , z ] = currentTime;

                }

            }

        }
        
        int totalSectorCount = ( int ) ( fieldSize / initialSectorSize ) * sectorCount * sectorCount;

        Debug.Log( "Plume model active, rendering " + totalSectorCount + " sectors" );

    }

    int x = -1;
    
    // Update is called once per frame
    void Update() {

        float deltaTime = Time.deltaTime;
        long currentTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;

        //windDirection = GameObject.Find( "WindDirectionBoard" ).GetComponent<ControlInterface>().getValue();
        //windSpeed = GameObject.Find( "WindSpeedBoard" ).GetComponent<ControlInterface>().getValue();
        //verticalWindVelocity = GameObject.Find( "WindVerticalSpeedBoard" ).GetComponent<ControlInterface>().getValue();
        //thermalStratification = ( ThermalStratifications ) ( ( int ) GameObject.Find( "ThermalStratificationBoard" ).GetComponent<ControlInterface>().getValue() );
        //rainSpeed = GameObject.Find( "RainSpeedBoard" ).GetComponent<ControlInterface>().getValue();

        float sectorHeight = maxSectorHeight / sectorCountVertical;
        float totalHeight = ( ( 3 * chimneyDiameter * verticalWindVelocity ) / windSpeed ) + chimneyHeight;
        int sectorCount = ( int ) ( fieldSize / initialSectorSize );
        float sectorSize = fieldSize / sectorCount;


        x++;
        if ( x >= sectorCount ) { x = 0; }

        //for ( int x = 0 ; x < sectorCount ; x++ ) {
        

        for ( int z = 0 ; z < sectorCount ; z++ ) {

            float surfaceContamination = 0f;

            for ( int y = 0 ; y < sectorCountVertical ; y++ ) {

                Vector3 relativePosistion = new Vector3( 0 , ( 0.1f + sectorHeight ) / 2 , 0 ) - new Vector3( fieldSize / 2 , 0 , fieldSize / 2 ) + new Vector3( x * sectorSize , sectorHeight * y , z * sectorSize ) + new Vector3( sectorSize / 2 , 0 , sectorSize / 2 );
                relativePosistion.Scale( new Vector3( 1 / scale , 1 / scale , 1 / scale ) );

                float adjustedX = ( relativePosistion.x * Mathf.Cos( -windDirection * Mathf.Deg2Rad ) ) - ( relativePosistion.z * Mathf.Sin( -windDirection * Mathf.Deg2Rad ) );
                float adjustedZ = ( relativePosistion.z * Mathf.Cos( -windDirection * Mathf.Deg2Rad ) ) + ( relativePosistion.x * Mathf.Sin( -windDirection * Mathf.Deg2Rad ) );

                GameObject sector = sectorAirGameObjects[ x , y , z ];
                 
                float outDistance = adjustedX;
                float concentration, gaussianFunction;

                if ( outDistance < 0 ) {

                    concentration = 0;
                    gaussianFunction = 0;

                }
                else {

                    float sigmaSide = lateralIntensities[ ( int ) thermalStratification ] * outDistance;
                    float sigmaHeight = verticalIntensities[ ( int ) thermalStratification ] * outDistance;

                    float posSide = adjustedZ;
                    float posHeight = relativePosistion.y;

                    float eSide = exp( -( posSide * posSide ) / ( 2 * ( sigmaSide * sigmaSide ) ) );
                    float eHeight = exp( -( ( posHeight - totalHeight ) * ( posHeight - totalHeight ) ) / ( 2 * ( sigmaHeight * sigmaHeight ) ) );

                    gaussianFunction = eSide * eHeight / windSpeed;
                    concentration = ( sourceActivityRate ) * gaussianFunction * ( 1 / ( 2 * Mathf.PI * sigmaSide * sigmaHeight ) );

                    gaussianFunction = gaussianFunction < 0.01f ? 0 : gaussianFunction;

                    if ( gaussianFunction != 0 ) {

                        sector.GetComponent<Renderer>().material.color = new Color( gaussianFunction , 1 - gaussianFunction , 0 , gaussianFunction > 0 ? 0.1f : 0 );

                    }
                    
                    float deposistionRate;
                    if ( rainSpeed > 0 ) { //Wet dposistion rate

                        float volumetricWashoutFactor = 1; //C_rain / C_air

                        deposistionRate = concentration * rainSpeed * volumetricWashoutFactor;

                    }
                    else {

                        deposistionRate = concentration * 0.006f;

                    }

                    surfaceContamination += ( deposistionRate * ( ( currentTime - sectorDeltaTimes[ x , y , z ] ) / ( float ) 1000 ) * timeAccelerationFactor );
                    sectorDeltaTimes[ x , y , z ] = currentTime;
                    
                }

                sector.SetActive( gaussianFunction != 0 );
                airContamination[ x , y , z ] = concentration;

            }

            relativeContamination[ x , z ] = relativeContamination[ x , z ] + surfaceContamination;

            float adjustedContamination = relativeContamination[ x , z ] / ( 3.7E7f ); //Bq to mCi
                
            GameObject surfaceSector = sectorGameObjects[ x , z ];
            surfaceSector.GetComponent<Renderer>().material.color = new Color( adjustedContamination , 1 - adjustedContamination , 0 , adjustedContamination > 0 ? 0.3f : 0 );


        }

        //}

        //Particles

        runningTime += Time.deltaTime;

        if ( runningTime > generationRate ) {

            runningTime = 0f;
            int index = -1;

            for ( int i = 0 ; i < particles.Length ; i++ ) {

                if ( particles[ i ] == null ) {

                    index = i;

                    break;

                }

            }

            if ( index == -1 ) {

                index = particles.Length - 1;

                Destroy( particles[ 0 ] );
                for ( int i = 0 ; i < particles.Length - 1 ; i++ ) {

                    particles[ i ] = particles[ i + 1 ];
                    deltaHeights[ i ] = deltaHeights[ i + 1 ];
                    totalTime[ i ] = totalTime[ i + 1 ];
                    acceleration[ i ] = acceleration[ i + 1 ];
                    initialMovement[ i ] = initialMovement[ i + 1 ];

                }

                


            }

            if ( index != -1 && windSpeed > 0 ) {


                float deltaHeight = ( 3 * chimneyDiameter * verticalWindVelocity * scale ) / windSpeed;
                deltaHeights[ index ] = deltaHeight;
                
                GameObject particle = GameObject.CreatePrimitive( PrimitiveType.Sphere );
                particle.transform.parent = stack.transform;
                particle.transform.localPosition = new Vector3( 0 , 1 , 0 );
                particle.transform.localScale = new Vector3( radius / stack.transform.localScale.x , radius / stack.transform.localScale.y , radius / stack.transform.localScale.z );

                particles[ index ] = particle;

                acceleration[ index ] = new Vector3( Mathf.Cos( windDirection * Mathf.Deg2Rad ) * chimneySpread * scale , deltaHeight , Mathf.Sin( windDirection * Mathf.Deg2Rad ) * chimneySpread * scale );
                initialMovement[ index ] = true;
                totalTime[ index ] = 0;
                
            }

        }

        float speed = windSpeed * speedFudgingNumber;
        float maxLifetime = maxDistance / speed;

        for ( int i = 0 ; i < particles.Length ; i++ ) {
            
            GameObject particle = particles[ i ];

            if ( particle != null ) {

                totalTime[ i ] = totalTime[ i ] + Time.deltaTime;

                if ( totalTime[ i ] > maxLifetime ) {

                    totalTime[ i ] = 0;
                    particles[ i ] = null;

                    Destroy( particle );

                }
                else {


                    if ( initialMovement[ i ] ) {

                        float particleHeight = ( particle.transform.localPosition.y - 1 ) * stack.transform.localScale.y;
                        
                        if ( particleHeight > deltaHeights[ i ] ) {

                            float outDistanceChimney = 1;

                            float standardDeviationX = lateralIntensities[ ( int ) thermalStratification ] * outDistanceChimney;
                            float standardDeviationY = verticalIntensities[ ( int ) thermalStratification ] * outDistanceChimney;

                            float zX = Mathf.Sqrt( -2f * Mathf.Log( Random.Range( 0f , 1f ) ) ) * Mathf.Cos( 2 * Mathf.PI * Random.Range( 0f , 1f ) );
                            float zY = Mathf.Sqrt( -2f * Mathf.Log( Random.Range( 0f , 1f ) ) ) * Mathf.Sin( 2 * Mathf.PI * Random.Range( 0f , 1f ) );

                            float distributionX = zX * standardDeviationX;
                            float distributionY = zY * standardDeviationY;

                            float newRadius = Mathf.Sqrt( ( 1 * 1 ) + ( distributionX * distributionX ) );

                            float deltaTheta = Mathf.Asin( Mathf.Abs( distributionX / newRadius ) ) * ( distributionX < 0 ? -1 : 1 ) * Mathf.Rad2Deg;

                            acceleration[ i ] = new Vector3( Mathf.Cos( ( windDirection + deltaTheta ) * Mathf.Deg2Rad ) * newRadius , distributionY , Mathf.Sin( ( windDirection + deltaTheta ) * Mathf.Deg2Rad ) * newRadius ).normalized;

                            initialMovement[ i ] = false;

                        }

                    }
              
                    particle.transform.localPosition = particle.transform.localPosition + ( new Vector3( acceleration[ i ].x / stack.transform.localScale.x , acceleration[ i ].y / stack.transform.localScale.y , acceleration[ i ].z / stack.transform.localScale.z ) * Time.deltaTime * speed  );
      
                }

            }

        }

        //Calculate concentration map

    }

    public enum ThermalStratifications {

        Extremely_unstable,
        Moderately_unstable,
        Neutral,
        Moderately_stable,
        Extremely_stable

    }

    private float exp( float x ) {

        return Mathf.Exp( x );
        

    }


}
