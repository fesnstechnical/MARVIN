﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHologram : MonoBehaviour {
    #region variables
   
    public GameObject plane;    //  holoUI background
    public GameObject UI_HologramExtension; //  scrollbar extension
    public Canvas thisCanvas;
    public Button closeButton, extendButton;
    public List<string> text = new List<string>();
    public bool isActive = false;   //  is the UI in the world?
    public float extensionDistance = 0.0f; //  how far from the origin of the main Holo UI?
    public float sizeModifier = 1.9f;   //  used to help modify the dynamic size of the holo-plane
    public float sizeModifierReducer = 0.4f; /// could probably do without this?

    private GameObject player;
    private Text displayText;
    private Color planeColor;
    private Color textColor;
    private GameObject extensionClone;
    private float size;
    private string extensionMeaurementScale;
    private bool extensionActive = false;
    #endregion

    public void setLines(string s, int index) { text.Add(s); }

    public void setPlaneColor(Color col) { planeColor = col; }

    public void setTextColor(Color col) { textColor = col; }

    public void setMeasurement(string s) { extensionMeaurementScale = s; }

    public void openExtension()
    {
        switch(extensionActive)
        {
            case false:
                extensionClone = Instantiate(UI_HologramExtension, this.transform);
                extensionClone.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + extensionDistance);
                extensionClone.transform.SetParent(this.transform);
                extensionClone.GetComponent<UIHologram_Scroll>().setColor(planeColor);
                extensionClone.GetComponent<UIHologram_Scroll>().setMeasurementText(extensionMeaurementScale);
                extensionActive = true;
                break;
        }
    }

    public void closeExtension()
    {
        if(extensionClone != null)
        {
            Destroy(extensionClone);
            extensionActive = false;
        }
    }

    public void closeUI()
    {
        Debug.Log("UI Closed!");
        Destroy(this);
    }

    private void Start() {
        player = GameObject.Find("Player");
        size = plane.GetComponent<RectTransform>().localScale.z * sizeModifier;
        displayText = this.GetComponentInChildren<Text>();
        GetComponentInChildren<Renderer>().material.color = planeColor; //  set rendering color
       // GetComponentInChildren<Material>().color = GetComponentInChildren<Renderer>().material.color;   //  set this material color to render as the rendering color
    }

    // Update is called once per frame
    private void Update () {

        switch(isActive)  //  if the UI isn't already displayed, display it
        {
            case false:
                for (int i = 0; i < text.Count; i++)
                {
                    displayText.text += text[i] + "\n";
                    plane.GetComponent<RectTransform>().localScale += new Vector3(0, 0, size);
                    plane.transform.position += new Vector3(0, size * (sizeModifier - sizeModifierReducer), 0);    //  shift the plane into its proper position
                }

                isActive = true;
                displayText.color = textColor;
                break;
        }
    }
}
