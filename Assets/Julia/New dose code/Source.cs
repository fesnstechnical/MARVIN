using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Source : MonoBehaviour {

    public RadioIsotopes radioIsotope;
    public float initialActivity;
    public Units unit;
    public bool firstUse = true;
    public string originalDate = "";

    private Transform sourceTransform;

    private Dictionary<string , Dictionary<float , float>> isotopeEmmissionData = new Dictionary<string, Dictionary<float, float>>();
    private Dictionary<string , float> isotopeHalfLifeData = new Dictionary<string , float>();

    private double startTime;

    //Used to contain information
    void Start() {

        this.sourceTransform = GetComponent<Transform>();
        this.startTime = new System.TimeSpan(System.DateTime.Now.Ticks).TotalMilliseconds;

        readCSV();

    }

    void Update() {
        


    }

    public float getDecayedActivity() {

        double time = new System.TimeSpan(System.DateTime.Now.Ticks).TotalMilliseconds - startTime; //Milliseconds

        if ( !firstUse && originalDate.Length > 2 ) {

            time = new System.TimeSpan(System.DateTime.Now.Ticks).TotalMilliseconds - new System.TimeSpan( System.Convert.ToDateTime(originalDate).Ticks ).TotalMilliseconds;
           
        }

        //A = A0 exp( -lambda t )

        float lambda = Mathf.Log( 2 ) / getHalfLife();
        float timef = ( float ) time; //Convert double to float

        float decayedActivity = initialActivity * Mathf.Exp( ( timef / 1000 ) * -lambda );

        return decayedActivity < 0 ? 0 : decayedActivity;

    }

    public Vector3 getPosistion() {

        if ( this.sourceTransform == null ){

            return new Vector3(0 , 0 , 0);

        }
        

        return sourceTransform.position;

    }

    //Returns Bq
    public float getActivity() {

        int setting = ( int ) unit; //mCi,Ci,Bq,TBq
        float activity = getDecayedActivity();

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


        return initialActivity;

    }

    public float getActivity( string unit ) {

        if ( unit == "mCi" ) {

            return initialActivity;

        }
        
        return initialActivity * 37000000; //mCi to Bq

    }

    public float getHalfLife() {
        
        return isotopeHalfLifeData[ getName() ];

    }

    public Dictionary<float , float> getParticleEnergies() {
        
        return isotopeEmmissionData[ getName() ];

    }

    public string getName() {

        return isotopeNames[ ( int ) radioIsotope ];

    }

    private void readCSV() {

        string fileData = System.IO.File.ReadAllText("Assets/Julia/Radionuclide Gamma Energies - Sheet1.csv");
        string[] lines = fileData.Split("\n".ToCharArray());

        if ( lines.Length > 2 ) {

            Dictionary<float , float> subData = new Dictionary<float , float>();

            for ( int i = 1 ; i < lines.Length ; i++ ) {

                

                //Splits the line by comma
                string[] data = lines[ i ].Split(",".ToCharArray());

                
                if ( data[ 0 ] != "" ) {

                    string isotope = data[ 0 ];
                    float gammaEnergyPeak = float.Parse( data[ 1 ] ); //keV
                    float gammaEnergyEmmissionProbability = float.Parse( data[ 2 ] ) / 100f; //0-1

                    if ( gammaEnergyEmmissionProbability > 1 ) {

                        gammaEnergyEmmissionProbability /= 2; //Dont ask

                    }

                    string halfLifeRaw = data[ 3 ]; //Dimless
                    string halfLifeUnits = data[ 4 ];
                       
                    if ( !isotopeHalfLifeData.ContainsKey( isotope ) ) {
                        
                        float halfLifeBase = float.Parse(halfLifeRaw); //Assume seconds

                        if ( halfLifeUnits == "minutes" ) {

                            halfLifeBase /= 60;

                        }
                        else if ( halfLifeUnits == "hours" ) {

                            halfLifeBase /= ( 60 * 60 );

                        }
                        else if ( halfLifeUnits == "days" ) {

                            halfLifeBase /= ( 60 * 60 * 24 );

                        }
                        else if ( halfLifeUnits == "years" ) {

                            halfLifeBase /= ( 60 * 60 * 24 * 365.25f );

                        }

                        isotopeHalfLifeData.Add(isotope , halfLifeBase);
                        
                    }

                    subData.Add(gammaEnergyPeak , gammaEnergyEmmissionProbability);
                    

                }
                else {

                    isotopeEmmissionData.Add(lines[ i - 1 ].Split(",".ToCharArray())[ 0 ] , subData);
                    subData = new Dictionary<float , float>();

                }

                if ( i == lines.Length - 1 ) { //End of file

                    //Add that bad boi, unless its already added
                    if ( !isotopeEmmissionData.ContainsKey(lines[ i  - 1].Split(",".ToCharArray())[ 0 ]) ) {

                        isotopeEmmissionData.Add(lines[ i - 1 ].Split(",".ToCharArray())[ 0 ] , subData);

                    }

                }

            }


        }


    }

    public enum Units {

        mill_Ci,
        Ci,
        Bq,
        terra_Bq

    }

    public enum RadioIsotopes {

        F_18,
        Na_22,
        Sc_46,
        Cr_51,
        Fe_55,
        Co_57,
        Co_58,
        Co_60,
        Ge_67,
        Ge_68,
        Ga_68,
        Se_75,
        Mo_99,
        Tc_99m,
        Cd_109,
        In_111,
        I_123,
        I_124,
        I_125,
        I_131,
        Sb_124,
        Ba_113,
        Cs_137,
        Ir_192,
        Ti_201,
        Am_241,


    }

    private string[] isotopeNames = new string[] {

        "F-18",
        "Na-22",
        "Sc-46",
        "Cr-51",
        "Fe-55",
        "Co-57",
        "Co-58",
        "Co-60",
        "Ge-67",
        "Ge-68",
        "Ga-68",
        "Se-75",
        "Mo-99",
        "Tc-99m",
        "Cd-109",
        "In-111",
        "I-123",
        "I-124",
        "I-125",
        "I-131",
        "Sb-124",
        "Ba-113",
        "Cs-137",
        "Ir-192",
        "Ti-201",
        "Am-241",

    };


}
