using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PSOnTrigger : MonoBehaviour {

    private DoseController doseController;
    private Transform ceTransform;
    public ParticleSystem ps;

    private Source source;

    public List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    private bool firstStart = true;

    void Start() {


    }

    int frameCount = 0;

    void Update() {

        if ( firstStart && frameCount > 5 ) {


            doseController = Controller.getController().getDoseController();

            ceTransform = GetComponentInChildren<Transform>();
            ps = GetComponent<ParticleSystem>();
            source = GetComponent<Source>();


            setColliders();

            firstStart = false;
            

        }

        if ( frameCount == 90 * 1.5 ) {

            //setColliders();
            frameCount = 0;

        }
        frameCount++;
   

    }

    //She a thicc
    
    private void setColliders() {
        
        List<Shield> shields = doseController.getShields(); //Gets all shields on the map from the dose controller
        

        shields = doseController.sortShields( shields , ceTransform.position ); //Sorts the shields closest to the source

        int max = 5;
        int count = shields.Count < max ? shields.Count : max; //If the number of shields is less than the max then iterate through all shields, else go up to 6 of the closest
        

        for ( int i = 0 ; i < count ; i++ ) {
            
            ps.trigger.SetCollider( i , shields[ i ].getCollider() );
  
            //Set to these colliders

        }
            

    }

    //Called when a particle collides with a shield
    private void OnParticleTrigger() {

        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

        int enterParticleNumber = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        
        if ( doseController == null ) {

            doseController = Controller.getController().getDoseController();

        }

        List< Shield > shields = doseController.getShields();
        
        //Interates over particles that have collided
        for ( int i = 0 ; i < enterParticleNumber ; i++ ) {

            ParticleSystem.Particle p = enter[ i ]; //Gets the particle

            if ( p.velocity.magnitude > 0 ) {

                Ray ray = new Ray( p.position + ( p.velocity * 10 ) , -p.velocity.normalized );

                RaycastHit getName;

                shields = doseController.sortShields( shields , p.position );
                Shield closestShield = shields[ 0 ];

                closestShield.getCollider().Raycast( ray , out getName , 1000f );

                if ( getName.collider != null ) {

                    //getName.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;

                    float thickness = ( p.position - getName.point ).magnitude;

                    string materialName = closestShield.nom;

                    float averageMaterialCoefficient = 0;
                    int count = 0;

                    foreach ( Isotope isotope in source.getIsotopes() ) {

                        float averageParticleEnergy = isotope.getGammaDecayEnergy() + isotope.getBetaDecayEnergy();
                        averageMaterialCoefficient += doseController.getMaterialAttenuationCoefficient( materialName , averageParticleEnergy );

                        count++;

                    }

                    averageMaterialCoefficient /= count;
                    averageMaterialCoefficient *= 1;

                    float random = Random.Range( 0 , 1000 );
                    float upperBound = 1000 * ( 1 - Mathf.Exp( -averageMaterialCoefficient * thickness ) );

                    if ( random < upperBound ) {

                        p.velocity = new Vector3( 0 , 0 , 0 );
                        p.remainingLifetime = 1;

                    }
                    else {

                        p.startColor = Color.red;

                    }


                }



            }

            enter[ i ] = p;

            
        }

        ps.SetTriggerParticles( ParticleSystemTriggerEventType.Enter , enter );


    }


    

}
