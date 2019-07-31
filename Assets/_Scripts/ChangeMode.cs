using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMode : MonoBehaviour
{
    public Transform Geiger;


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Mode Changed");
        Geiger.GetComponent<GeigerControler_Minh>().mode++;
        if (Geiger.GetComponent<GeigerControler_Minh>().mode > 3)
        {
            Geiger.GetComponent<GeigerControler_Minh>().mode = 0;
        }
    }
}
