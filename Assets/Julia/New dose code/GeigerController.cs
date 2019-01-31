using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class GeigerController : MonoBehaviour {

    private bool pickedUp = false;
    private Transform toolTransform;
    private Interactable interactable;
    private Rigidbody toolBody;
    private Transform handTransform;
    private Hand hand;

    public GameObject rightHand;

    private Vector3 priorPosistion;
    private Quaternion priorAngle;

    private GameObject handRenderPrefab;


    // Start is called before the first frame update
    void Start() {

        toolTransform = GetComponent<Transform>();
        interactable = GetComponent<Interactable>();
        toolBody = GetComponent<Rigidbody>();

        handTransform = rightHand.GetComponent<Transform>();
        hand = rightHand.GetComponent<Hand>();
        
    }

    // Update is called once per frame
    void Update() {

        if ( pickedUp ) {

            if ( priorPosistion != null & priorAngle != null ) {

                toolTransform.localPosition = priorPosistion;
                toolTransform.localRotation = priorAngle;

            }

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

                

            }
            else {

            }

 
            
        }
        else if ( message == "Drop" ) {

            pickedUp = !pickedUp;
            updateTransform();

        }

    }

}
