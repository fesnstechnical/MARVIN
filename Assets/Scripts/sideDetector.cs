using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sideDetector : MonoBehaviour {
   
    public SheildingSpawner.side lastSideTouched;
    public Vector3 pointTouched;
    public Vector3 originalScale;
    public Vector3 originalPosition;
	// Use this for initialization
	void Start () {
        lastSideTouched = SheildingSpawner.side.NONE;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
  
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collision!");
        Vector3 normal = collision.contacts[0].normal;
        //Debug.Log(normal);
    }

}
