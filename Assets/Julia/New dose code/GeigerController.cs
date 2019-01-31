using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class GeigerController : DoseBody {

    private bool pickedUp = false;
    private bool active = false;
    private Transform toolTransform;
    private Interactable interactable;
    private Rigidbody toolBody;
    private Transform handTransform;
    private Hand hand;

    public GameObject rightHand;

    private Vector3 priorPosistion;
    private Quaternion priorAngle;

    private GameObject handRenderPrefab;

    private List<DoseReceptor> doseReceptors = new List<DoseReceptor>();

    private bool lastStateTrigger = false;

    //Components
    TextMesh doseTextMesh;
    AudioSource audioSourceGeigerClick;
    AudioSource audioSourceEffects;
    public MeshRenderer meshRendererStatusSphere;

    //Settings
    private string[] modes = { "CPS" , "CPM" , "Sv/hr" };
    private float[] scales = { 0.001f , 1f , 1000f , 10e6f };
    private string[] prefixes = { "m" , "" , "k" , "M" };

    int mode = 0;
    int scale = 0;

    public override void secondaryStart() {

        toolTransform = GetComponent<Transform>();
        interactable = GetComponent<Interactable>();
        toolBody = GetComponent<Rigidbody>();

        handTransform = rightHand.GetComponent<Transform>();
        hand = rightHand.GetComponent<Hand>();

        doseReceptors.Add(new DoseReceptor( 1 , 0.002f , getTransform()));

        //components
        doseTextMesh = GetComponentInChildren<TextMesh>();
        audioSourceGeigerClick = GetComponents<AudioSource>()[ 0 ];
        audioSourceGeigerClick = GetComponents<AudioSource>()[ 1 ];

        doseTextMesh.text = "";
        

    }

    // Update is called once per frame
    void Update() {

        if ( pickedUp ) {

            if ( priorPosistion != null & priorAngle != null ) {

                toolTransform.localPosition = priorPosistion;
                toolTransform.localRotation = priorAngle;

            }

        }

        if ( active ) {

            float value;

            if ( mode == 0 || mode == 1 ) { //Counts

                value = getCountRate();

                if ( mode == 1 ) {

                    value = value / 60; //CPS to CPM

                }

            }
            else { //Dose

                value = getDoseRate();

            }

            value = value / scales[ scale ];

            doseTextMesh.text = value + "\n" + prefixes[ scale ] + modes[ mode ];

        }
        else {

            doseTextMesh.text = "";

        }

    }

    private void checkInputs() {

        //float inputTrigger = SteamVR_Input.__actions_default_in_ControllerTrigger.GetAxis(SteamVR_Input_Sources.RightHand);
        //float inputOther = SteamVR_Input.__

        bool triggerDown = false;//inputTrigger == 1f;

        if ( lastStateTrigger != triggerDown ) {

            lastStateTrigger = triggerDown;

            if ( lastStateTrigger ) {

                //Change modes
                if ( ( mode + 1 ) > modes.Length ) {

                    mode = 0;

                }
                else {

                    mode++;

                }

                //Play sound

            }
         
        }

    }

    private void updateActive() {

        if ( active ) {

            //Play active sound, change color, turn on text mesh
            meshRendererStatusSphere.material.SetColor( "_Color" , Color.green);


        }
        else {

            meshRendererStatusSphere.material.SetColor("_Color" , Color.black);

        }

    }

    private void updateTransform() {

        if ( pickedUp ) {
            
            toolBody.useGravity = false;
            interactable.highlightOnHover = false;
            toolTransform.SetParent(handTransform);

            priorPosistion = toolTransform.localPosition;
            priorAngle = toolTransform.localRotation;

            handRenderPrefab = hand.renderModelPrefab;
            handRenderPrefab.GetComponent<MeshRenderer>().enabled = false;
            hand.SetRenderModel(handRenderPrefab);
            


        }
        else {

            toolTransform.SetParent(null);

            toolBody.useGravity = true;
            interactable.highlightOnHover = true;
            toolTransform.position = handTransform.position;

            hand.SetRenderModel(handRenderPrefab);

        }


    }

    void SendMessage( string message ) {

        if ( message == "Pickup" ) {

            if ( pickedUp == false ) {

                active = true;
                updateActive();

            }
            else {

            }

 
            
        }
        else if ( message == "Drop" ) {

            if ( pickedUp ) {

                active = false;
                updateActive();

            }

            pickedUp = !pickedUp;
            updateTransform();

        }

    }

    public override List<DoseReceptor> getDoseReceptors() {

        return doseReceptors;

    }
}
