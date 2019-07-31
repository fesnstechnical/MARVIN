using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParticleTrigger : MonoBehaviour {

    private DoseController doseController;
    private Transform ceTransform;
    public ParticleSystem ps;

    private Source source;

    public List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    private bool firstStart = true;
    Color particlePostPenetrationColorMultiplier = new Color(0.5f, 0.5f, 0.5f, 0.9f);


    [Tooltip("Toggle changing particle emission properties based on Activity")]
    public bool adjustCountRate = true;

    [Header("Particle Emission Activity Throttling (shared)")]
    public static Vector2 radiationAmountRange = new Vector2(1f, 100000.0f); 
    public static Vector2 emissionRateRange = new Vector2(1, 100000);
    public static Vector2 ParticleScaleRange = new Vector2(0.2f, 5.0f);
    float startSize = 0.05f;

    [Header("Use accurate particle model" ) ]
    public bool useAccurateParticleModel = false;

    [Header( "Use taylor series for exp" )]
    public bool taylorApproximation = true;

    void UpdateEmissionRate()
    {
        float activity = source.totalActivityLevelBq;
        var emissionProps = ps.emission;
        var systemProps = ps.main;

        float normalizedRadiation = Mathf.InverseLerp(radiationAmountRange.x, radiationAmountRange.y, activity);

        float rate = Mathf.Lerp(emissionRateRange.x, emissionRateRange.y, normalizedRadiation);
        emissionProps.rateOverTime = rate; //Mathf.Clamp(rate, emissionRateRange.x, emissionRateRange.y);

        float startSizeScale = Mathf.Lerp(ParticleScaleRange.x, ParticleScaleRange.y, normalizedRadiation);
        systemProps.startSizeMultiplier = startSize * startSizeScale;
    }

    //#if UNITY_EDITOR
    //    private void OnValidate()
    //    {
    //        UpdateEmissionRate();
    //    }
    //#endif

    private bool started = false;

    void Start() {

        doseController = Controller.getController().getDoseController();

        ceTransform = GetComponentInChildren<Transform>();
        ps = GetComponent<ParticleSystem>();
        startSize = ps.main.startSizeMultiplier;
        
        source = GetComponent<Source>();

        if ( source == null ) {

            source = GetComponentInParent<Source>();

        }

        setColliders();

        if ( adjustCountRate ){

            InvokeRepeating( "UpdateEmissionRate" , 0.0f , 1.0f );

        }


        
    }

    int frameCount = 0;

    void Update() {

        if ( !started ) {

            doseController = Controller.getController().getDoseController();

            ceTransform = GetComponentInChildren<Transform>();
            ps = GetComponent<ParticleSystem>();
            startSize = ps.main.startSizeMultiplier;

            source = GetComponent<Source>();

            if ( source == null ) {

                source = GetComponentInParent<Source>();

            }

            setColliders();

            if ( adjustCountRate ) {

                InvokeRepeating("UpdateEmissionRate" , 0.0f , 1.0f);

            }

            started = true;

        }

        if ( firstStart && frameCount > 5 ) {

            firstStart = false;
        }

        if ( frameCount == 90 * 1.5 ) {

            setColliders();
            frameCount = 0;

        }
        frameCount++;
    }

    //She a thicc
    
    private void setColliders() {
        
        List<Shield> shields = doseController.getShields(); //Gets all shields on the map from the dose controller
        

        shields = doseController.sortShields( shields , ceTransform.position ); //Sorts the shields closest to the source

        int max = 6;
        int count = shields.Count < max ? shields.Count : max; //If the number of shields is less than the max then iterate through all shields, else go up to 6 of the closest
        
        for ( int i = 0 ; i < count ; i++ ) {
            
            ps.trigger.SetCollider( i , shields[ i ].getCollider() );
  
            //Set to these colliders

        }
        
    }
    

    //Called when a particle collides with a shield
    void OnParticleTrigger() {
        

        enter = new List<ParticleSystem.Particle>();

        int enterParticleNumber = ps.GetTriggerParticles( ParticleSystemTriggerEventType.Enter , enter );
        
        if ( source != null ) {
            
            if ( doseController == null ) {

                doseController = Controller.getController().getDoseController();

            }

            List<Shield> shields = doseController.getShields(); 
            List<Isotope> isotopes = source.getIsotopes();

            Isotope selected = isotopes[ 0 ];
            float setPoint;

            if ( !useAccurateParticleModel ) { 

                float randomIsotope = Random.Range( 0 , 100 ) / 100f;
                
                setPoint = 0f;

                for ( int k = 0 ; k < isotopes.Count ; k++ ) {

                    if ( randomIsotope > setPoint && randomIsotope <= setPoint + isotopes[ k ].concentration ) {

                        selected = isotopes[ k ];
                        break;

                    }
                    else {

                        setPoint += isotopes[ k ].concentration;

                    }

                }

            }

            //Interates over particles that have collided
            for ( int i = 0 ; i < enterParticleNumber ; i++ ) {

                ParticleSystem.Particle p = enter[ i ]; //Gets the particle

                int hitID = 0;
                
                RaycastHit getName;

                if ( Physics.Raycast( new Ray( p.position , p.velocity.normalized ) , out getName , 0.1F ) ) {

                    hitID = getName.collider.GetInstanceID();
                 
                }

                if ( p.velocity.magnitude > 0 ) {

                    if ( useAccurateParticleModel ) {

                        float randomIsotope = Random.Range( 0 , 100 ) / 100f;

                        selected = isotopes[ 0 ];

                        setPoint = 0f;

                        for ( int k = 0 ; k < isotopes.Count ; k++ ) {

                            if ( randomIsotope > setPoint && randomIsotope <= setPoint + isotopes[ k ].concentration ) {

                                selected = isotopes[ k ];
                                break;

                            }
                            else {

                                setPoint += isotopes[ k ].concentration;

                            }

                        }

                    }

                    float thickness = 0;
                    
                    Shield collidedShield = shields[ 0 ];
                    
                    float castDistance = 100.0f;

                    RaycastHit[] hits = Physics.RaycastAll(p.position + ( p.velocity.normalized * castDistance ) , -p.velocity.normalized , castDistance);

                    for ( int k = 0 ; k < hits.Length ; k++ ) {
                        
                        if ( hits[ k ].collider.gameObject.GetInstanceID() == hitID ) {

                            float ceThickness = 0;

                            GameObject hitObject = hits[ k ].collider.gameObject;

                            Shield shield = hitObject.GetComponent<Shield>();

                            if ( shield != null ) {

                                if ( shields.Contains( shield ) ) {

                                    ceThickness = ( p.position - hits[ k ].point ).magnitude;

                                }

                            }

                            if ( ( ceThickness < thickness || thickness == 0 ) && ceThickness != 0 ) {

                                thickness = ceThickness;
                                collidedShield = shield;
                                

                            }

                            break;

                        }

                        

                    }
                    
                    
                    
                    if ( thickness != 0 ) {

                        string materialName = collidedShield.getName();
                        
                        float averageMaterialCoefficient = ( doseController.getMaterialAttenuationCoefficient(materialName , selected.getBetaDecayEnergy() + selected.getGammaDecayEnergy()) );
                        averageMaterialCoefficient *= 1f;

                        float random = Random.Range(0 , 1000);
                        float upperBound;

                        float tout = -averageMaterialCoefficient * thickness;

                        if ( taylorApproximation && -tout < 3 ) {

                            float taylor = 1 + tout + ( ( tout * tout ) / 2 ) + ( ( tout * tout * tout ) / 6 ) + ( ( tout * tout * tout * tout ) / 24 );

                            upperBound = 1000 * ( 1 - taylor );

                        }
                        else {

                            upperBound = 1000 * ( 1 - Mathf.Exp(tout) );

                        }
                        

                        // if particle penetrates
                        if ( random < upperBound ) {

                            //p.position = getName.point;
                            p.velocity = new Vector3(0 , 0 , 0);
                            p.remainingLifetime = 0f;
                 
                            
                        }
                        else {

                            //p.startColor = Color.blue;
                   
                        }
                        
                    }

                    
                }


                enter[ i ] = p;


            }

        }

        ps.SetTriggerParticles( ParticleSystemTriggerEventType.Enter , enter );


    }

    private void DrawLine(Vector3 start , Vector3 end , Color color , float duration = 1f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color , 1.0f) } ,
            new GradientAlphaKey[] { new GradientAlphaKey(1 , 1.0f) }
        );

        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.colorGradient = gradient;
        lr.SetPosition(0 , start);
        lr.SetPosition(1 , end);
        GameObject.Destroy(myLine , duration);
    }




}
