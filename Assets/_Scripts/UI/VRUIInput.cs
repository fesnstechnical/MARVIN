using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;
//This script raycasts from the controller to wherever and if it hits a buttton it calls it's onlick function
//laser pointer is strictly visual.


public class VRUIInput : MonoBehaviour
    {
    public bool menuMode = false;
    public Canvas menu;
    private void Update()
        {
        if (SteamVR_Actions._default.Menu.GetStateUp(SteamVR_Input_Sources.Any))
        {
            if (menuMode)
            {
                Debug.Log("menu off!!!");
                menuMode = false;
            } else
            {
                Debug.Log("menu on!!!");
                menuMode = true;
            }
        }

        if (menuMode)
        {
            //menu.enabled = true;
            //menu.GetComponentInChildren<BoxCollider>().enabled = true;
            GetComponent<SteamVR_LaserPointer>().thickness = 0.002f;

            RaycastHit hit;
            Ray raycast = new Ray( transform.position , transform.forward );
            if ( SteamVR_Actions._default.InteractUI.GetStateUp( SteamVR_Input_Sources.Any ) ) {
                Debug.Log( "click!" );
                if ( Physics.Raycast( raycast , out hit , 1000.0f ) ) {
                    Debug.Log( "doink" );
                    if ( hit.transform != null ) {
                        Debug.Log( hit.transform.tag );

                        if ( hit.transform.tag == "Button" ) {
                            Debug.Log( "that's a button" );
                            hit.transform.GetComponent<Button>().onClick.Invoke();
                        }
                    }
                }
            }

        }
        else
        {
            //menu.enabled = false;
            //menu.GetComponentInChildren<BoxCollider>().enabled = false;
            GetComponent<SteamVR_LaserPointer>().thickness = 0.0f;
        }


           






        }
    }
