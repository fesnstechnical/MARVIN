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
    public float UISpawnDistance = 5.0f;    //  distance from this object to player that the holo UI spawns at
    public bool spawnUIOnStart; //  mainly for debugging

    private GameObject UIClone;  //  instantiated UI element
    private bool spawned = false;   //  has the UIClone been spawned?
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
        if (spawnUIOnStart) { spawnUI(); }
    }

    private void spawnUI()
    {
        UIClone = Instantiate(UI_HologramElement, this.transform.position, this.transform.rotation);
        UIClone.transform.Translate(new Vector3(0, displayHeight, 0));  //  move the UI clone upwards
        UIClone.GetComponent<UIHologram>().setPlaneColor(planeColor);
        UIClone.GetComponent<UIHologram>().setTextColor(textColor);
        UIClone.GetComponent<UIHologram>().setMeasurement(extensionMeasurementScale);

        for (int i = 0; i < lines.Count; i++)   //  set lines in instantiated object
        {
            UIClone.GetComponent<UIHologram>().setLines(lines[i], i);
        }
    }

    private void Update()
    {
        float dist = Mathf.Sqrt(Mathf.Pow((this.transform.position.x - GameObject.Find("Player").transform.position.x), 2) + Mathf.Pow((this.transform.position.y - GameObject.Find("Player").transform.position.y), 2) + Mathf.Pow((this.transform.position.z - GameObject.Find("Player").transform.position.z), 2));  //  distance between this object and player position

        if (dist < UISpawnDistance && !spawned) //  spawn
        {
            spawnUI();
            spawned = true;
        }
        else if(dist > UISpawnDistance && spawned)  //  de-spawn
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
