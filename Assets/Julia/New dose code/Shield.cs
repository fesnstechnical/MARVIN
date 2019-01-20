using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    //Used to contain information
    public string name;

    public string getName() {

        return name;

    }

    public Collider getCollider() {

        return GetComponent<Collider>();

    }

}
