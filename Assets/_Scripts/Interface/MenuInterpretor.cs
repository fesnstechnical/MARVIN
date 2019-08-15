using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInterpretor : MonoBehaviour {
    
    private ScintillatorScript scintillator;
    private Controller controller;

    // Start is called before the first frame update
    void Start() {

        scintillator = GameObject.Find( "Liquid Scintillator" ).GetComponent<ScintillatorScript>();
        
        controller = Controller.getController();

    }


    //Wtf is a switch method, je ne sais pas

    public void sendMessage( string message ) {
        
        if ( message == "ToggleScintillatorDoor" ) {



        }
        else if ( message == "ToggleParticles" ) {



        }
        else if ( message == "ToggleGammaSelected" ) {

            controller.getGammaGunController().toggleCap( controller.getGammaGunController().getSelected() );

        }
        else if ( message == "IncrementSelected" ) {

            controller.getGammaGunController().incrementSelect();

        }
        else if ( message == "DecrementSelected" ) {

            controller.getGammaGunController().decrementSelect();

        }
        else if ( message == "ToggleGammaAll" ) {

            controller.getGammaGunController().toggleAll();

        }
        else if ( message == "IncrementPlatform" ) {

            controller.getPlatformMover().incrementPlatform();


        }
        else if ( message == "DecrementPlatform" ) {

            controller.getPlatformMover().decrementPlatform();

        }
        else if ( message == "RecordPoint" ) {

            controller.getGammaGunController().recordPoint();

        }
        else if ( message == "ExportData" ) {

            controller.getGammaGunController().exportData();



        }
        else if ( message == "Button_Menu_Reset" ) {




        }
        else if ( message == "Button_Menu_NormalVision" ) {

            Controller.getController().getVisionModeController().setModeNormal();


        }
        else if ( message == "Button_Menu_XRayVision" ) {

            Controller.getController().getVisionModeController().setModeXRay();

        }

    }


    void SendMessage( string message ) {

        sendMessage( message );


    }

}
