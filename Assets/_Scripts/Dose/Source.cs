using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Source : MonoBehaviour {


    public float initialActivity;
    public Units unit;
    public bool firstUse = true;
    public string originalDate = "";
    public bool enableComposistionEvolution = true;

    public List<Isotope> isotopes = new List<Isotope>();

    private Dictionary<string, float> activities = new Dictionary<string, float>();

    private Transform sourceTransform;

    private double startTime;

    private BoxCollider boxCollider;
    private Rigidbody rigidbody;

    private List<Isotope> decayChain;

    // in Bq
    public float totalActivityLevelBq { get; private set; }

    //Used to contain information
    void Start() {

        this.sourceTransform = GetComponent<Transform>();
        this.startTime = new System.TimeSpan(System.DateTime.Now.Ticks).TotalMilliseconds;

        boxCollider = GetComponent<BoxCollider>();
        rigidbody = GetComponent<Rigidbody>();

        getFullDecayChain();

        float randomTime = Random.Range( 0 , 150 ) / 10;

        updateActivityLevel();

        //InvokeRepeating("updateActivityLevel", randomTime , 10.0f);



    }


    public Dictionary<string , float> getNewComposistion() {

        Dictionary<string , float> newComposistion = new Dictionary<string , float>();

        float totalActivity = 0f;

        foreach ( string nom in activities.Keys ) {

            totalActivity += activities[ nom ];
        }

        foreach ( string nom in activities.Keys ) {

            newComposistion[ nom ] = activities[ nom ] / totalActivity;
        }

        return newComposistion;
    }

    //This thing is my portal of truth, so I get the make the disision on how this is used
    //Its come to that, and you're sure about this?
    private void updateActivityLevel() {
    
        totalActivityLevelBq = 0;
        
        double time = new System.TimeSpan( System.DateTime.Now.Ticks ).TotalMilliseconds - startTime; //Milliseconds

        if ( !firstUse && originalDate.Length > 2 ) {

            time = new System.TimeSpan( System.DateTime.Now.Ticks ).TotalMilliseconds - new System.TimeSpan( System.Convert.ToDateTime( originalDate ).Ticks ).TotalMilliseconds;

        }

        float timef = ( float ) time;

        if ( enableComposistionEvolution ) {

            foreach ( Isotope mother in isotopes ) {

                recurseActivity( null , mother , timef , initialActivity , true );

            }

        }
        else {

            foreach ( Isotope mother in isotopes ) {

                float lambda = mother.getDecayConstant();

                activities[ mother.getIsotopeName() ] = initialActivity * Mathf.Exp( ( timef / 1000 ) * -lambda ) * mother.getConcentration();

            }


        }

        foreach ( Isotope isotope in getFullDecayChain() ) {

            totalActivityLevelBq += getDecayedActivity( isotope.getIsotopeName() );
            
        }

    }
    

    private List<Isotope> getFullDecayChain() {

        if ( decayChain == null ) {

            decayChain = new List<Isotope>();

            if ( enableComposistionEvolution ) {

                foreach ( Isotope isotope in getIsotopes() ) {

                    recurseDecayChain( ref decayChain , isotope , 0 );

                }

            }
            else {

                foreach ( Isotope isotope in getIsotopes() ) {

                    decayChain.Add( isotope );

                }

            }

        }

        return decayChain;

    }

    private List<Isotope> recurseDecayChain( ref List<Isotope> decayChain , Isotope parent , int round ) {

        if ( round > 4 ) {

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

    //I am the truth of your despair, the inescapable price of your bostfulness
    //And now I will bestow upon you the dispair you deserve
    private void recurseActivity( Isotope mother , Isotope daughter , float time , float initialAcitivity , bool first ) {

        //First calculate own activity
        float decayedActivity = 0;
        
        if ( first ) {

            float lambda = Mathf.Log( 2 ) / daughter.getHalfLife();
            decayedActivity = initialActivity * Mathf.Exp( ( time / 1000 ) * -lambda ) * daughter.getConcentration();
            
            activities[ daughter.getIsotopeName() ] = decayedActivity;

        }
        else {

            //Daughter parent
  
            if ( !daughter.isStable() ) {

                float lambdaM = mother.getDecayConstant();
                float lambdaD = daughter.getDecayConstant();

                decayedActivity = initialAcitivity * ( lambdaD / ( lambdaD - lambdaM ) ) * ( Mathf.Exp( -lambdaM * time ) - Mathf.Exp( -lambdaD * time ) ) * daughter.getConcentration();

                activities[ daughter.getIsotopeName() ] = decayedActivity;
                
            }
            else {
                
                activities[ daughter.getIsotopeName() ] = 0;

            }

        }

        foreach ( Isotope grandDaughter in daughter.getDecayProducts() ) {

            recurseActivity( daughter , grandDaughter , time , decayedActivity , false );

        }

    }

    private float getDecayedActivity( string isotopeName ) {

        if ( isotopeName == null ) {

            return 0;

        }
        
        return activities[ isotopeName ];

    }

    public Vector3 getPosistion() {
        
        return sourceTransform.position;

    }

    //Returns Bq
    public float getActivity( string isotopeName ) {

        int setting = ( int ) unit; //mCi,Ci,Bq,TBq
        float activity = getDecayedActivity( isotopeName );

        if ( setting == 0 ) { //mCi

            return activity * 37000000;

        }
        else if ( setting == 1 ) { //Ci

            return activity * 37000000000;

        }
        else if ( setting == 2 ) { //Bq

            return activity;

        }
        else if ( setting == 3 ) { //TBq

            return activity * 1000000000000;

        }


        return activity;

    }

    public float getActivity( string unit , string isotopeName ) {

        float activity = getDecayedActivity( isotopeName );

        if ( unit == "mCi" ) {

            return activity;

        }
        
        return activity * 37000000; //mCi to Bq

    }

    public float getHalfLife( string isotopeName ) {

        foreach ( Isotope isotope in isotopes ) {

            if ( isotope.getIsotopeName() == isotopeName ) {

                return isotope.getHalfLife();

            }

        }

        return 0;

    }

    public float getGammaEnergy( string isotopeName ) {

        foreach ( Isotope isotope in isotopes ) {

            if ( isotope.getIsotopeName() == isotopeName ) {

                return isotope.getGammaDecayEnergy();

            }

        }

        return 0;

    }

    public float getBetaEnergy( string isotopeName ) {

        foreach ( Isotope isotope in isotopes ) {

            if ( isotope.getIsotopeName() == isotopeName ) {

                return isotope.getBetaDecayEnergy();

            }

        }

        return 0;

    }

    public List<Isotope> getIsotopes() {

        return this.isotopes;

    }
    
    public enum Units {

        mill_Ci,
        Ci,
        Bq,
        terra_Bq

    }

    public void freeze() {

        boxCollider.enabled = false;
        rigidbody.useGravity = false;

    }

    public void unfreeze() {

        boxCollider.enabled = true;
        rigidbody.useGravity = true;

    }

}
