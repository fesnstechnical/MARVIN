using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RotatingHandle: MonoBehaviour {

    private bool gripped = false;
    private bool lastState = false;
    private bool attached = false;
    private bool freeMode = false;

    private GameObject rightHand;
    private GameObject leftHand;

    private float angleStart = -1f;
    private float lastAngle = -1f;

    private float maxAngle = 180;

    public int intervals;

    // Start is called before the first frame update
    void Start() {

        rightHand = GameObject.Find( "RightHand" );
        leftHand = GameObject.Find( "LeftHand" );


    }

    // Update is called once per frame
    void Update() {
        
        bool rightHandGrip = SteamVR_Actions._default.Squeeze.GetAxis( SteamVR_Input_Sources.RightHand ) == 1;
        bool leftHandGrip = false;

        if ( !rightHand ) {

            leftHandGrip = SteamVR_Actions._default.Squeeze.GetAxis( SteamVR_Input_Sources.LeftHand ) == 1;

        }

        gripped = leftHandGrip || rightHandGrip;
        GameObject hand = rightHandGrip ? rightHand : leftHand;
        
        if ( gripped ) {

            if ( !attached ) {
                
                if ( ( hand.transform.position - transform.position ).magnitude < 0.1f ) {

                    attached = true;
                    //hand.SetActive( false );

                }

                

            }

        }
        else {

            if ( attached ) {
                

                //leftHand.SetActive( true );
                //rightHand.SetActive( true );

                attached = false;
                
            }

            angleStart = -1f;
            lastAngle = -1f;

        }

        if ( attached ) {

            float handAngle = hand.transform.localEulerAngles.y;

            if ( lastAngle == -1 ) { lastAngle = transform.localEulerAngles.y; }
            if ( angleStart == -1f ) { angleStart = handAngle; }

            float differential = ( handAngle - angleStart );
            
            float resultant = lastAngle + differential;
            resultant = resultant < 0 ? 360 + resultant : resultant;
            resultant = resultant > 0 && resultant < 90 ? 0 : resultant;
            resultant = resultant < maxAngle && resultant >= 90 ? maxAngle : resultant;
            
            transform.localEulerAngles = new Vector3( 0 , ( int )( resultant / intervals ) * intervals , 0 );
   
            if ( resultant == maxAngle && !freeMode ) {

                freeMode = true;
                
            }
            

            if ( freeMode && resultant == 0 ) {

                freeMode = false;
                
            }

        }

        foreach ( Renderer renderer in GetComponentsInChildren<Renderer>() ){ 

            renderer.material.color = ( attached || freeMode ) ? Color.green : Color.red;

        }

    }

    public float getPercentActivation() {
        
        if ( transform.localEulerAngles.y == 0 ) {

            return 0;

        }
        else {

            return ( 360 - transform.localEulerAngles.y ) / ( float ) maxAngle;

        }

    }

}
