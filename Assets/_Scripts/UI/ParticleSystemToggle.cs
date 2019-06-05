using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemToggle : MonoBehaviour
{
    
    public ParticleSystem ps;
    // Start is called before the first frame update
    public void ToggleParticles() {
        if(ps.GetComponent<Renderer>().enabled == true) {
            ps.GetComponent<Renderer>().enabled = false;
        } else {
            ps.GetComponent<Renderer>().enabled = true;
        }


    }
}
