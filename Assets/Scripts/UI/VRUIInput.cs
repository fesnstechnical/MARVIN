using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;

//This script raycasts from the controller to wherever and if it hits a buttton it calls it's onlick function
//laser pointer is strictly visual.


    public class VRUIInput : MonoBehaviour
    {
        private void Update()
        {
            RaycastHit hit;
            Ray raycast = new Ray(transform.position, transform.forward);
            if (SteamVR_Actions._default.InteractUI.GetStateUp(SteamVR_Input_Sources.Any))
            {
                Debug.Log("click!");
                if(Physics.Raycast(raycast,out hit, 300.0f))
                {
                    if(hit.transform != null)
                    {
                        if(hit.transform.tag == "Button")
                        {
                            Debug.Log("that's a button");
                            hit.transform.GetComponent<Button>().onClick.Invoke();
                        }
                    }
                }
            }







        }
    }
