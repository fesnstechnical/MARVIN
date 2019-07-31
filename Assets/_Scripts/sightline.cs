using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sightline : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.transform.gameObject.GetComponent<Outline>())
        {
            other.transform.gameObject.GetComponent<Outline>().isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.gameObject.GetComponent<Outline>())
        {
            other.transform.gameObject.GetComponent<Outline>().isColliding = false;
        }
    }
}
