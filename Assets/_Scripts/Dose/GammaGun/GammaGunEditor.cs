using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Controller))]
public class GammaGunEditor : Editor {


    public override void OnInspectorGUI() {
        

        for ( int i = 0 ; i < Controller.getController().getGammaGunController().getGammaShields().Count ; i++ ) {

            if ( GUILayout.Button( "Toggle cap " + ( i + 1 ) ) ) {

                Controller.getController().getGammaGunController().toggleCap( i );

            }

        }

    }

}
