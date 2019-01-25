using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class User : DoseBody {
    
    //Collection of dose receptors
    private List<DoseReceptor> doseReceptors = new List<DoseReceptor>();
    

    public override void secondaryStart() {
        
        doseReceptors.Add( new DoseReceptor(77, 1.7f / 2 , getTransform()) );

    }

    public void Update() {
        
        checkInputs();


    }

    public override List<DoseReceptor> getDoseReceptors() {

        return doseReceptors;

    }


    private bool lastState = false;

    private void checkInputs() { 
    

        float input = SteamVR_Input.__actions_default_in_ControllerTrigger.GetAxis(SteamVR_Input_Sources.Any);

        bool triggerDown = input == 1f;
        
        if ( lastState != triggerDown ){

            lastState = triggerDown;

            if ( lastState ) {




            }
            else {

            }

        }

    }
    


}
