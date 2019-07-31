using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ReflectorPlatformMover : MonoBehaviour {

    private bool gripped = false;
    private bool lastState = false;
    private bool attached = false;
    private bool freeMode = false;

    private GameObject rightHand;
    private GameObject leftHand;

    private float angleStart = -1f;
    private float lastAngle = -1f;

    private AudioSource audioSource;
    public Rigidbody rigidBody;

    public AudioClip openClip;
    public AudioClip closeClip;
    public AudioClip clickClip;

    // Start is called before the first frame update
    void Start() {

        rightHand = GameObject.Find( "RightHand" );
        leftHand = GameObject.Find( "LeftHand" );
        audioSource = GetComponent<AudioSource>();

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
   
            resultant = resultant > 0 && resultant < 180 ? 0 : resultant;
            resultant = resultant < -90 ? -90 : resultant;
            resultant = resultant < 270 && resultant >= 180 ? 270 : resultant;
            resultant = resultant > 360 ? 0 : resultant;
            
            transform.localEulerAngles = new Vector3( 0 , resultant , 0 );
   

            if ( Mathf.Abs( resultant ) >= 90 && !freeMode ) {

                freeMode = true;
                audioSource.clip = openClip;
                audioSource.Play();

                rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            }
            

            if ( freeMode && resultant == 0 ) {

                freeMode = false;
                audioSource.clip = closeClip;
                audioSource.Play();

                rigidBody.constraints = RigidbodyConstraints.FreezeAll;

            }

        }

        GetComponent<Renderer>().material.color = ( attached || freeMode ) ? Color.green : Color.red;
        

    }

}
