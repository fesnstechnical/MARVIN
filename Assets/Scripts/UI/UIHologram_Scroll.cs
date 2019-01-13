using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHologram_Scroll : MonoBehaviour{
    #region variables
    public Text numberDisplay, measurementDisplay;
    public GameObject extensionPlane;
    public GameObject handle, fill; //  to change scroll image color

    private Color color;
    private string measurement;
    private bool isActive = false;
    private float numForDisplay = 0;
    #endregion

    public void setColor(Color c) { color = c; }

    public void setMeasurementText(string s) { measurement = s; }

    private void Start()
    {
        GetComponentInChildren<Renderer>().material.color = color;  //  set rendering color
        GetComponentInChildren<Material>().color = GetComponentInChildren<Renderer>().material.color;   //  set this material color to render as the rendering color
    }

    // Update is called once per frame
    void Update() {
        if (!isActive) { 
            measurementDisplay.text = measurement;
            isActive = true;
        }
        numberDisplay.text = this.GetComponentInChildren<Slider>().value.ToString();
    }
}
