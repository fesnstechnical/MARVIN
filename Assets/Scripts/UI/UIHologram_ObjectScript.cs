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
    public bool spawnUIOnStart;

    private GameObject UIClone;  //  instantiated UI element
    private bool spawned = false;
    #endregion

   /* private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player") && !spawnUIOnStart && !UIClone.GetComponent<UIHologram>().isActive)    //  if player triggers with object, spawn UI and set lines
        {
            spawnUI();
        }
    }*/

    private void Awake()
    {
        if (spawnUIOnStart)
        {
            spawnUI();
        }
    }

    private void spawnUI()
    {
        UIClone = Instantiate(UI_HologramElement, this.transform.position, this.transform.rotation);
        UIClone.transform.Translate(new Vector3(0, displayHeight, 0));  //  move the UI clone upwards
        UIClone.GetComponent<UIHologram>().setPlaneColor(planeColor);
        UIClone.GetComponent<UIHologram>().setTextColor(textColor);
        UIClone.GetComponent<UIHologram>().setMeasurement(extensionMeasurementScale);

        for (int i = 0; i < lines.Count; i++)
        {
            UIClone.GetComponent<UIHologram>().setLines(lines[i], i);
        }
    }

    private void Update()
    {
        float dist = Mathf.Sqrt(Mathf.Pow((this.transform.position.x - GameObject.Find("Player").transform.position.x), 2) + Mathf.Pow((this.transform.position.y - GameObject.Find("Player").transform.position.y), 2) + Mathf.Pow((this.transform.position.z - GameObject.Find("Player").transform.position.z), 2));

        if (dist < 5 && !spawned)
        {
            spawnUI();
            spawned = true;
        }
        else if(dist > 5 && spawned)
        {
            Destroy(UIClone);
            spawned = false;
        }

        if (spawned)
        {
            UIClone.GetComponent<Transform>().LookAt(GameObject.Find("Player").transform.position);  //  billboarding
        }
    }
}
