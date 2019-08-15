using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(VisionModeController))]
public class VisionModeEditor : Editor {


    public override void OnInspectorGUI() {


        if ( GUILayout.Button( "Normal mode" ) ) {

            Controller.getController().getVisionModeController().setModeNormal();

        }
        else if ( GUILayout.Button( "X-ray mode" ) ) {

            Controller.getController().getVisionModeController().setModeXRay();

        }
        else if ( GUILayout.Button( "Highlight on" ) ) {

            Controller.getController().getVisionModeController().setInformationBlockHighlightState( true );

        }
        else if ( GUILayout.Button( "Highlight off" ) ) {

            Controller.getController().getVisionModeController().setInformationBlockHighlightState( false );

        }


    }

}
#endif
