using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScintillatorScript : MonoBehaviour
{
    [HideInInspector] public bool isOpen = true;

    private Animator a;
    private bool playerWithinTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        a = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))    ///  INSERT VR INPUT HERE
        {
            switch (playerWithinTrigger) {
                case true:
                     switch (isOpen)
                     {
                         case true:
                             close();
                             break;
                         case false:
                             open();
                             break;
                     }
                     break;
            }
        }
    }

    public void open()
    {
        switch (isOpen)
        {
            case false:
                a.Play("Scintillator_DrawerOpen");
                isOpen = true;
                break;
        }
    }

    public void close()
    {
        switch (isOpen)
        {
            case true:
                a.Play("Scintillator_DrawerClose");
                isOpen = false;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player") { playerWithinTrigger = true; }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.name == "Player") { playerWithinTrigger = false; }
    }
}
