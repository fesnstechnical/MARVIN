using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {

    private static Dictionary<string , Dictionary<float , float>> chiltonPrimeValues;
    private static Dictionary<string , Dictionary<float , float>> chiltonIncidentValues;

    public static float getChiltonPrimeValue( string material , float energy ) {
        
        List<float> keyList = new List<float>( chiltonPrimeValues[ material ].Keys );
        Dictionary<float , float> data = chiltonPrimeValues[ material ];

        float value = 0;
        
        if ( energy < keyList[ 0 ] ) { //If the particle energy is below the lowest energy

            //y = mx + b
            float m = ( data[ keyList[ 1 ] ] -data[ keyList[ 0 ] ] ) / ( keyList[ 1 ] - keyList[ 0 ] );
            float b = data[ keyList[ 1 ] ] - ( m * keyList[ 1 ] );

            value = ( m * energy ) + b;

        }
        else if ( energy > keyList[ keyList.Count - 1 ] ) { //If the particle energy is larger than the highest particle energy

            //y = mx + b
            float m = ( data[ keyList[ data.Keys.Count - 2 ] ] - data[ keyList[ data.Keys.Count - 1 ] ] ) / ( keyList[ data.Keys.Count - 2 ] - keyList[ data.Keys.Count - 1 ] );
            float b = data[ keyList[ data.Keys.Count - 1 ] ] - ( m * keyList[ data.Keys.Count - 1 ] );

            value = ( m * energy ) + b;

        }
        else { //Particle energy is somewhere in known particle energy range

            for ( int k = 1 ; k < keyList.Count - 2 ; k++ ) {

                if ( energy >= keyList[ k ] && energy <= keyList[ k + 1 ] ) {

                    //y = mx + b
                    float m = ( data[ keyList[ k + 1 ] ] - data[ keyList[ k ] ] ) / ( keyList[ k + 1 ] - keyList[ k ] );
                    float b = data[ keyList[ k ] ] - ( m * keyList[ k ] );

                    value = ( m * energy ) + b;

                    break;

                }


            }

        }

        return value;

    }

    public static float getChiltonIncidentValue( string material , float energy ) {

        List<float> keyList = new List<float>( chiltonIncidentValues[ material ].Keys );
        Dictionary<float , float> data = chiltonIncidentValues[ material ];

        float value = 0;


        if ( energy < keyList[ 0 ] ) { //If the particle energy is below the lowest energy

            //y = mx + b
            float m = ( data[ keyList[ 1 ] ] - data[ keyList[ 0 ] ] ) / ( keyList[ 1 ] - keyList[ 0 ] );
            float b = data[ keyList[ 1 ] ] - ( m * keyList[ 1 ] );

            value = ( m * energy ) + b;

        }
        else if ( energy > keyList[ keyList.Count - 1 ] ) { //If the particle energy is larger than the highest particle energy

            //y = mx + b
            float m = ( data[ keyList[ data.Keys.Count - 2 ] ] - data[ keyList[ data.Keys.Count - 1 ] ] ) / ( keyList[ data.Keys.Count - 2 ] - keyList[ data.Keys.Count - 1 ] );
            float b = data[ keyList[ data.Keys.Count - 1 ] ] - ( m * keyList[ data.Keys.Count - 1 ] );

            value = ( m * energy ) + b;

        }
        else { //Particle energy is somewhere in known particle energy range
            

            for ( int k = 1 ; k < keyList.Count - 2 ; k++ ) {
                
                if ( energy >= keyList[ k ] && energy <= keyList[ k + 1 ] ) {

                    //y = mx + b
                    float m = ( data[ keyList[ k + 1 ] ] - data[ keyList[ k ] ] ) / ( keyList[ k + 1 ] - keyList[ k ] );

                    float b = data[ keyList[ k ] ] - ( m * keyList[ k ] );

                    value = ( m * energy ) + b;

                    break;

                }


            }

        }

        return value;

    }

    // Start is called before the first frame update
    void Start() {

        //Chilton-Huddleston values
        chiltonPrimeValues = new Dictionary<string , Dictionary<float , float>>();
        chiltonIncidentValues = new Dictionary<string , Dictionary<float , float>>();

        Dictionary<float , float> chiltonPrimeValues_Water = new Dictionary<float , float>();
        Dictionary<float , float> chiltonPrimeValues_Concrete = new Dictionary<float , float>();
        Dictionary<float , float> chiltonPrimeValues_Iron = new Dictionary<float , float>();
        Dictionary<float , float> chiltonPrimeValues_Lead = new Dictionary<float , float>();

        Dictionary<float , float> chiltonIncidentValues_Water = new Dictionary<float , float>();
        Dictionary<float , float> chiltonIncidentValues_Concrete = new Dictionary<float , float>();
        Dictionary<float , float> chiltonIncidentValues_Iron = new Dictionary<float , float>();
        Dictionary<float , float> chiltonIncidentValues_Lead = new Dictionary<float , float>();

        chiltonPrimeValues_Water.Add( 100f , 3839.56f / 10E6f );
        chiltonPrimeValues_Water.Add( 200f , 12893.3f / 10E6f );
        chiltonPrimeValues_Water.Add( 400f , 26925.1f / 10E6f );
        chiltonPrimeValues_Water.Add( 600f , 36269f / 10E6f );
        chiltonPrimeValues_Water.Add( 800f , 44642.8f / 10E6f );
        chiltonPrimeValues_Water.Add( 1000f , 52786.3f / 10E6f );
        chiltonPrimeValues_Water.Add( 1250f , 61972.9f / 10E6f );
        chiltonPrimeValues_Water.Add( 2000f , 86564.2f / 10E6f );
        chiltonPrimeValues_Water.Add( 4000f , 137182f / 10E6f );
        chiltonPrimeValues_Water.Add( 6000f , 172511f / 10E6f );
        chiltonPrimeValues_Water.Add( 8000f , 195014f / 10E6f );
        chiltonPrimeValues_Water.Add( 10000f , 218439f / 10E6f );

        chiltonIncidentValues_Water.Add( 100f , 156682f / 10E6f );
        chiltonIncidentValues_Water.Add( 200f , 95129.4f / 10E6f );
        chiltonIncidentValues_Water.Add( 400f , 49412f / 10E6f );
        chiltonIncidentValues_Water.Add( 600f , 35540.3f / 10E6f );
        chiltonIncidentValues_Water.Add( 800f , 27780.2f / 10E6f );
        chiltonIncidentValues_Water.Add( 1000f , 22560.2f / 10E6f );
        chiltonIncidentValues_Water.Add( 1250f , 18555.1f / 10E6f );
        chiltonIncidentValues_Water.Add( 2000f , 12633.8f / 10E6f );
        chiltonIncidentValues_Water.Add( 4000f , 8639.79f / 10E6f );
        chiltonIncidentValues_Water.Add( 6000f , 7473.89f / 10E6f );
        chiltonIncidentValues_Water.Add( 8000f , 6977.39f / 10E6f );
        chiltonIncidentValues_Water.Add( 10000f , 6587.47f / 10E6f );

        chiltonPrimeValues_Concrete.Add( 100f , 15080.6f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 200f , 19531.7f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 400f , 31051.5f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 600f , 38511.7f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 800f , 46563f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 1000f , 54483f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 1250f , 65366.8f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 2000f , 86521.5f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 4000f , 134941f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 6000f , 162904f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 8000f , 178589f / 10E6f );
        chiltonPrimeValues_Concrete.Add( 10000f , 196888f / 10E6f );

        chiltonIncidentValues_Concrete.Add( 100f , 53570.2f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 200f , 56396.8f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 400f , 34862.3f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 600f , 26535.8f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 800f , 20976.8f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 1000f , 17311.4f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 1250f , 14205.4f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 2000f , 10602.7f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 4000f , 8849.81f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 6000f , 8473.75f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 8000f , 8361.58f / 10E6f );
        chiltonIncidentValues_Concrete.Add( 10000f , 8150.7f / 10E6f );

        chiltonPrimeValues_Iron.Add( 100f , 6019.74f / 10E6f );
        chiltonPrimeValues_Iron.Add( 200f , 22881.8f / 10E6f );
        chiltonPrimeValues_Iron.Add( 400f , 34986.6f / 10E6f );
        chiltonPrimeValues_Iron.Add( 600f , 44766.3f / 10E6f );
        chiltonPrimeValues_Iron.Add( 800f , 52970.4f / 10E6f );
        chiltonPrimeValues_Iron.Add( 1000f , 55709.3f / 10E6f );
        chiltonPrimeValues_Iron.Add( 1250f , 70598.5f / 10E6f );
        chiltonPrimeValues_Iron.Add( 2000f , 91450.5f / 10E6f );
        chiltonPrimeValues_Iron.Add( 4000f , 131920f / 10E6f );
        chiltonPrimeValues_Iron.Add( 6000f , 148934f / 10E6f );
        chiltonPrimeValues_Iron.Add( 8000f , 170405f / 10E6f );
        chiltonPrimeValues_Iron.Add( 10000f , 173252f / 10E6f );

        chiltonIncidentValues_Iron.Add( 100f , 6307.25f / 10E6f );
        chiltonIncidentValues_Iron.Add( 200f , 4524.19f / 10E6f );
        chiltonIncidentValues_Iron.Add( 400f , 10535.5f / 10E6f );
        chiltonIncidentValues_Iron.Add( 600f , 10521.2f / 10E6f );
        chiltonIncidentValues_Iron.Add( 800f , 9632.14f / 10E6f );
        chiltonIncidentValues_Iron.Add( 1000f , 8658.93f / 10E6f );
        chiltonIncidentValues_Iron.Add( 1250f , 7651.2f / 10E6f );
        chiltonIncidentValues_Iron.Add( 2000f , 7817.77f / 10E6f );
        chiltonIncidentValues_Iron.Add( 4000f , 10501.4f / 10E6f );
        chiltonIncidentValues_Iron.Add( 6000f , 11578.4f / 10E6f );
        chiltonIncidentValues_Iron.Add( 8000f , 11914.4f / 10E6f );
        chiltonIncidentValues_Iron.Add( 10000f , 11992.6f / 10E6f );

        chiltonPrimeValues_Lead.Add( 100f , -992.54f / 10E6f );
        chiltonPrimeValues_Lead.Add( 200f , 2541.39f / 10E6f );
        chiltonPrimeValues_Lead.Add( 400f , 12314f / 10E6f );
        chiltonPrimeValues_Lead.Add( 600f , 23025f / 10E6f );
        chiltonPrimeValues_Lead.Add( 800f , 32433.2f / 10E6f );
        chiltonPrimeValues_Lead.Add( 1000f , 41593.7f / 10E6f );
        chiltonPrimeValues_Lead.Add( 1250f , 51294.8f / 10E6f );
        chiltonPrimeValues_Lead.Add( 2000f , 72077.7f / 10E6f );
        chiltonPrimeValues_Lead.Add( 4000f , 93292f / 10E6f );
        chiltonPrimeValues_Lead.Add( 6000f , 107474f / 10E6f );
        chiltonPrimeValues_Lead.Add( 8000f , 125587f / 10E6f );
        chiltonPrimeValues_Lead.Add( 10000f , 139207f / 10E6f );

        chiltonIncidentValues_Lead.Add( 100f , 71426f / 10E6f );
        chiltonIncidentValues_Lead.Add( 200f , 13686.2f / 10E6f );
        chiltonIncidentValues_Lead.Add( 400f , -4163.44f / 10E6f );
        chiltonIncidentValues_Lead.Add( 600f , -6355.61f / 10E6f );
        chiltonIncidentValues_Lead.Add( 800f , -6219.26f / 10E6f );
        chiltonIncidentValues_Lead.Add( 1000f , -5711.07f / 10E6f );
        chiltonIncidentValues_Lead.Add( 1250f , -4898.82f / 10E6f );
        chiltonIncidentValues_Lead.Add( 2000f , 658.92f / 10E6f );
        chiltonIncidentValues_Lead.Add( 4000f , 7477.07f / 10E6f );
        chiltonIncidentValues_Lead.Add( 6000f , 8800.86f / 10E6f );
        chiltonIncidentValues_Lead.Add( 8000f , 9079.66f / 10E6f );
        chiltonIncidentValues_Lead.Add( 10000f , 9038.1f / 10E6f );

        chiltonPrimeValues.Add( "Water" , chiltonPrimeValues_Water );
        chiltonPrimeValues.Add( "Concrete (Ordinary)" , chiltonPrimeValues_Concrete );
        chiltonPrimeValues.Add( "Iron" , chiltonPrimeValues_Iron );
        chiltonPrimeValues.Add( "Lead" , chiltonPrimeValues_Lead );

        chiltonIncidentValues.Add( "Water" , chiltonIncidentValues_Water );
        chiltonIncidentValues.Add( "Concrete (Ordinary)" , chiltonIncidentValues_Concrete );
        chiltonIncidentValues.Add( "Iron" , chiltonIncidentValues_Iron );
        chiltonIncidentValues.Add( "Lead" , chiltonIncidentValues_Lead );

    }

    // Update is called once per frame
    void Update() {

    }



}
