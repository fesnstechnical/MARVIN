using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class SheildingSpawner : MonoBehaviour {

    public SteamVR_Input_Sources thisHand;
 

    enum mode
    {
        free,
        placingFirstPoint,
        placingSecondPoint
    }

    public enum side
    {
        NONE,
        front,
        back,
        top,
        bottom,
        left,
        right
    }

    enum editingMode
    {
        dragging,
        free
    }

    private mode currentMode;
    private editingMode editingState;
    public GameObject shieldingPrefab;
    private GameObject currentObject = null;
    private Vector3 firstPoint;
    public GameObject cursor;
    private sideDetector gameobjectEditing = null;
    private bool triggerPressedLastFrame = false;
    private bool triggerReleasedThisFrame;
    public bool active = false;

	// Use this for initialization
	void Start () {
        editingState = editingMode.free;
	}

    // Update is called once per frame
    void Update() {

        if(active)
        {
            activeUpdate();
        }

        
	}

    private void activeUpdate()
    {
        triggerReleasedThisFrame = false;
        float inputTrigger = SteamVR_Actions.default_ControllerTrigger.GetAxis(SteamVR_Input_Sources.Any);

        bool triggerDown = inputTrigger == 1f;
        bool triggerPressedThisFrame = false;

        if (triggerDown == false && triggerPressedLastFrame)
        {
            triggerReleasedThisFrame = true;
        }
        else if (triggerDown == true && triggerPressedLastFrame == false)
        {
            triggerPressedThisFrame = true;
        }

        triggerPressedLastFrame = triggerDown;



        if (modeManager.mode == modeManager.handMode.placing)
        {
            if (currentMode == mode.placingSecondPoint)
            {
                currentObject.transform.position = firstPoint + ((cursor.transform.position - firstPoint) / 2);
                //Debug.Log("Controller pos: " + cursor.transform.position);
                // Debug.Log("First point: " + firstPoint);
                //Debug.Log(currentObject.transform.position);
                currentObject.transform.localScale = (cursor.transform.position - firstPoint);
            }

            //if released trigger
            if (triggerReleasedThisFrame)
            {
                // Debug.Log("Released Trigger!");
                if (currentMode == mode.free)
                {
                    currentMode = mode.placingFirstPoint;
                    currentObject = Instantiate(shieldingPrefab, cursor.transform);
                    //Debug.Log("Instantiated cube!");

                }

                else if (currentMode == mode.placingFirstPoint)
                {
                    currentMode = mode.placingSecondPoint;
                    firstPoint = cursor.transform.position;
                    currentObject.transform.parent = null;
                    //Debug.Log("Current pos: " + currentObject.transform.position);
                    currentObject.transform.position = cursor.transform.position;
                    currentObject.transform.rotation = Quaternion.identity;
                    //Debug.Log("Current pos: " + currentObject.transform.position);
                    //Debug.Log("First point placed! First point: " + firstPoint);
                }

                else if (currentMode == mode.placingSecondPoint)
                {
                    currentMode = mode.free;
                    currentObject.AddComponent<Rigidbody>();
                    currentObject.AddComponent<Interactable>();
                    currentObject.AddComponent<VelocityEstimator>();
                    currentObject.AddComponent<Throwable>();
                    currentObject = null;
                    //Debug.Log("Second point placed!");

                }
            }
        }

        else if (modeManager.mode == modeManager.handMode.editing)
        {
            if (triggerPressedThisFrame)
            {
                Debug.Log("Trigger Pulled!");
                //check colliding shielding with cube and delete
                Collider[] colliders = Physics.OverlapSphere(cursor.transform.position, 0.1f);
                //GameObject sphere = Instantiate(sphereDebug, cursor.transform.position, Quaternion.identity);
                //sphere.transform.localScale = new Vector3(cursor.transform.localScale.x, cursor.transform.localScale.x, cursor.transform.localScale.x);
                GameObject shieldingBeingEdited = null;
                side touchedSide = side.NONE;
                Debug.Log("Num collisions: " + colliders.Length);
                //Debug.Log("Sphere size: " + cursor.transform.localScale.x);
  


                foreach (Collider collider in colliders)
                {
                    Debug.Log(collider.gameObject.name);
                    if (collider.tag == "Top")
                    {
                        touchedSide = side.top;
                    }
                    else if (collider.tag == "Bottom")
                    {
                        touchedSide = side.bottom;
                    }
                    else if (collider.tag == "Left")
                    {
                        touchedSide = side.left;
                    }
                    else if (collider.tag == "Right")
                    {
                        touchedSide = side.right;

                    }
                    else if (collider.tag == "Front")
                    {
                        touchedSide = side.front;
                    }
                    else if (collider.tag == "Back")
                    {
                        touchedSide = side.back;
                    }
                    else if (collider.tag == "Shielding")
                    {
                        shieldingBeingEdited = collider.gameObject;
                        gameobjectEditing = collider.gameObject.GetComponent<sideDetector>();
                        gameobjectEditing.pointTouched = cursor.transform.position;
                        gameobjectEditing.originalPosition = shieldingBeingEdited.gameObject.transform.position;
                        gameobjectEditing.originalScale = shieldingBeingEdited.gameObject.transform.localScale;
                    }
                    else
                    {

                    }
                    //Debug.Log("Shielding side touched: " + touchedSide);
                }
                if (shieldingBeingEdited != null && touchedSide != side.NONE)
                {
                    shieldingBeingEdited.GetComponent<sideDetector>().lastSideTouched = touchedSide;
                    editingState = editingMode.dragging;
                    Debug.Log("New side last touched set!");
                }
                else if (gameobjectEditing != null)
                {
                    editingState = editingMode.free;
                    gameobjectEditing.lastSideTouched = side.NONE;
                    gameobjectEditing.originalPosition = Vector3.zero;
                    gameobjectEditing.originalScale = Vector3.one;
                    gameobjectEditing.pointTouched = Vector3.zero;
                    gameobjectEditing = null;
                }
            }

            if (triggerReleasedThisFrame)
            {
                editingState = editingMode.free;
                gameobjectEditing = null;
            }
            else
            {
                if (editingState == editingMode.dragging && gameobjectEditing != null)
                {
                    Debug.Log("Dragging shielding");
                    //float newScaleY, newPosY, newScaleX, newPosX, newScaleZ, newPosZ;
                   // Debug.Log("Original position: " + gameobjectEditing.transform.position);
                    //Debug.Log("Original Scale: " + gameobjectEditing.transform.localScale);
                    switch (gameobjectEditing.lastSideTouched)
                    {
                        case side.top:
                            //delta on y
                            gameobjectEditing.transform.localScale = new Vector3(gameobjectEditing.transform.localScale.x, gameobjectEditing.originalScale.y + (cursor.transform.position - gameobjectEditing.pointTouched).y, gameobjectEditing.transform.localScale.z);
                            gameobjectEditing.transform.position = new Vector3(gameobjectEditing.transform.position.x, gameobjectEditing.originalPosition.y + ((cursor.transform.position - gameobjectEditing.pointTouched).y / 2), gameobjectEditing.transform.position.z);
                            break;

                        case side.bottom:
                            gameobjectEditing.transform.localScale = new Vector3(gameobjectEditing.transform.localScale.x, gameobjectEditing.originalScale.y - (cursor.transform.position - gameobjectEditing.pointTouched).y, gameobjectEditing.transform.localScale.z);
                            gameobjectEditing.transform.position = new Vector3(gameobjectEditing.transform.position.x, gameobjectEditing.originalPosition.y - ((gameobjectEditing.pointTouched - cursor.transform.position).y / 2), gameobjectEditing.transform.position.z);
                            break;

                        case side.right:
                            gameobjectEditing.transform.localScale = new Vector3(gameobjectEditing.originalScale.x + (cursor.transform.position - gameobjectEditing.pointTouched).x, gameobjectEditing.transform.localScale.y, gameobjectEditing.transform.localScale.z);
                            gameobjectEditing.transform.position = new Vector3(gameobjectEditing.originalPosition.x + ((cursor.transform.position - gameobjectEditing.pointTouched).x / 2), gameobjectEditing.transform.position.y, gameobjectEditing.transform.position.z);
                            break;

                        case side.left:
                            gameobjectEditing.transform.localScale = new Vector3(gameobjectEditing.originalScale.x - (cursor.transform.position - gameobjectEditing.pointTouched).x, gameobjectEditing.transform.localScale.y, gameobjectEditing.transform.localScale.z);
                            gameobjectEditing.transform.position = new Vector3(gameobjectEditing.originalPosition.x - ((gameobjectEditing.pointTouched - cursor.transform.position).x / 2), gameobjectEditing.transform.position.y, gameobjectEditing.transform.position.z);

                            break;

                        case side.front:
                            gameobjectEditing.transform.localScale = new Vector3(gameobjectEditing.transform.localScale.x, gameobjectEditing.transform.localScale.y, gameobjectEditing.originalScale.z + (cursor.transform.position - gameobjectEditing.pointTouched).z);
                            gameobjectEditing.transform.position = new Vector3(gameobjectEditing.originalPosition.x, gameobjectEditing.transform.position.y, gameobjectEditing.originalPosition.z + ((cursor.transform.position - gameobjectEditing.pointTouched).z / 2));

                            break;

                        case side.back:
                            gameobjectEditing.transform.localScale = new Vector3(gameobjectEditing.transform.localScale.x, gameobjectEditing.transform.localScale.y, gameobjectEditing.originalScale.z - (cursor.transform.position - gameobjectEditing.pointTouched).z);
                            gameobjectEditing.transform.position = new Vector3(gameobjectEditing.originalPosition.x, gameobjectEditing.transform.position.y, gameobjectEditing.originalPosition.z - ((gameobjectEditing.pointTouched - cursor.transform.position).z / 2));
                            break;

                        default:
                            break;


                    }

                    //Debug.Log("New position: " + gameobjectEditing.transform.position);
                    //Debug.Log("New Scale: " + gameobjectEditing.transform.localScale);
                }
            }
        }

        else if (modeManager.mode == modeManager.handMode.deleting)
        {
            if (triggerReleasedThisFrame)
            {
                //check colliding shielding with cube and delete
                Collider[] colliders = Physics.OverlapSphere(cursor.transform.position, cursor.transform.localScale.x / 2);
                foreach (Collider collider in colliders)
                {
                    if (collider.gameObject.tag == "Shielding")
                    {
                        Destroy(collider.gameObject);
                    }
                }
            }
        }

    }

    public void enable()
    {
        active = true;
    }

    public void disable()
    {
        active = false;
    }
}
