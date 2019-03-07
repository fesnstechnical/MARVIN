using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class modeManager : MonoBehaviour {

    Color placing = Color.green;
    Color editing = Color.yellow;
    Color deleting = Color.red;
    public SteamVR_Input_Sources thisHand;
    public static handMode mode;
    public Material cursorCubeMaterial;
   
    

    public enum handMode
    {
        placing,
        editing,
        deleting
    }
    // Use this for initialization
    void Start () {
        mode = handMode.placing;
        cursorCubeMaterial.color = placing;
	}

    // Update is called once per frame
    void Update() {

        bool grip = SteamVR_Actions.default_GrabGrip.GetStateUp(SteamVR_Input_Sources.Any);
        bool touchpad = SteamVR_Actions.default_Teleport.GetStateUp(SteamVR_Input_Sources.Any);




        if (touchpad)
        {
            if(GameObject.FindObjectOfType<SheildingSpawner>().active)
            {
                if (mode == handMode.placing)
                {
                    mode = handMode.editing;
                    cursorCubeMaterial.color = editing;
                }
                else if (mode == handMode.editing)
                {
                    mode = handMode.deleting;
                    cursorCubeMaterial.color = deleting;
                }
                else if (mode == handMode.deleting)
                {
                    mode = handMode.placing;
                    cursorCubeMaterial.color = placing;
                }
            }
            
        }

        
	}
}
