using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalUpdater : MonoBehaviour {

    private UnityEngine.UI.Text terminalCountRateText;
    private UnityEngine.UI.Text terminalDistanceText;
    private UnityEngine.UI.Text terminalShieldPosistionText;

    private GeigerController geigerController;


    // Start is called before the first frame update
    void Start() {

        terminalCountRateText = GameObject.Find( "Terminal count rate text" ).GetComponent<UnityEngine.UI.Text>();
        terminalDistanceText = GameObject.Find( "Terminal distance text" ).GetComponent<UnityEngine.UI.Text>();
        terminalShieldPosistionText = GameObject.Find( "Terminal shield posistion text" ).GetComponent<UnityEngine.UI.Text>();


        geigerController = GameObject.Find( "GeigerTool (GammaGun)" ).GetComponent<GeigerController>();

    }

    // Update is called once per frame
    void Update() {
        terminalCountRateText.text = ( int ) ( geigerController.getCountRate() / 1 ) + " CPS";

        if ( Controller.getController().getPlatformMover() != null ) {

            terminalDistanceText.text = System.Math.Round( Controller.getController().getPlatformMover().getDistanceBetweenSourceEtDetector() , 2 ) + " m";

        }

        if ( Controller.getController().getGammaGunController() != null ) {

            terminalShieldPosistionText.text = "" + Controller.getController().getGammaGunController().getSelected();

        }


    }
}
