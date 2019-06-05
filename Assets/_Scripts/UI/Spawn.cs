using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    public GameObject SpawnObject;
    public GameObject SpawnPoint;
   
    public void spawn() {
        Instantiate( SpawnObject , SpawnPoint.transform.position , Quaternion.identity );
    }
}
