using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioHandler : MonoBehaviour {



    // Start is called before the first frame update
    void Start() {
    
    
        
    }

    // Update is called once per frame
    void Update() {
    
        
    }

    private void clearScenario() {

        //Enable all transport mats
        GameObject.Find( "Teleportation" ).SetActive( true );

    }

    public void loadScenarioDefault() {

        clearScenario();

        //Set pos
        GameObject.Find( "Teleportation" ).transform.position = new Vector3( 12 , 0 , -3.62f );
        GameObject.Find( "Teleportation" ).transform.rotation = Quaternion.Euler( 0 , 270 , 0 );


    }

    public void loadScenarioShielding() {

        clearScenario();

        GameObject.Find( "Teleportation Hallway 2" ).SetActive( false );
        GameObject.Find( "Teleportation Hallway 1" ).SetActive( false );

        //Set pos
        GameObject.Find( "Teleportation" ).transform.position = new Vector3( 4.38f , 0 , -6.64f );
        GameObject.Find( "Teleportation" ).transform.rotation = Quaternion.Euler( 0 , 180 , 0 );

    }

}
