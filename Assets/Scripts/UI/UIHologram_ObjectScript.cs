using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHologram_ObjectScript : MonoBehaviour {
    #region variables
    public List<string> lines = new List<string>();
    public GameObject UI_HologramElement;   //  the UI hologram prefab
    public string extensionMeasurementScale;   //  ie. mCi
    public Color planeColor;
    public Color textColor;
    public float displayHeight; //  how high from the spawn point the plane should be

    private GameObject UIClone;  //  instantiated UI element
    #endregion

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))    //  if player triggers with object, spawn UI and set lines
        {
            spawnUI();
        }
    }

    private void Awake()
    {
       // spawnUI();
    }

    private void spawnUI()
    {
        UIClone = Instantiate(UI_HologramElement, this.transform);
        UIClone.transform.Translate(new Vector3(0, displayHeight, 0));  //  move the UI clone upwards
        UIClone.transform.SetParent(this.transform);    //  set this object as parent to the instantiated UI element
        UIClone.GetComponent<UIHologram>().setPlaneColor(planeColor);
        UIClone.GetComponent<UIHologram>().setTextColor(textColor);
        UIClone.GetComponent<UIHologram>().setMeasurement(extensionMeasurementScale);

        for (int i = 0; i < lines.Count; i++)
        {
            UIClone.GetComponent<UIHologram>().setLines(lines[i], i);
        }
    }
}
