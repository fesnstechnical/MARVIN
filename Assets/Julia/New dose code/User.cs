using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class User : DoseBody {

    private AudioSource audioSource;
    //private Renderer render;

    public Material unlitMaterial;
    public Material litMaterial;
    public Renderer render;

    //Collection of dose receptors
    private List<DoseReceptor> doseReceptors = new List<DoseReceptor>();
    

    public override void secondaryStart() {
        
        doseReceptors.Add( new DoseReceptor(77, 1.7f / 2 , getTransform()) );
        audioSource = GetComponent<AudioSource>();
        //render = GetComponent<Renderer>();

    }

    public void Update() {

        updateControllerRate(getDoseRate() , getCountRate());
        hitScan();


    }

    public override List<DoseReceptor> getDoseReceptors() {

        return doseReceptors;

    }

    private TextMesh textMesh = null;
    private float lastDoseRate = 0;
    private float lastCountRate = 0;

    int updateClicks = 40;
    int updateClick = 0;

 
    private void updateControllerRate ( float doseRate , float countRate ) {

        
        //Only update if dose has changed
        if ( ( doseRate != lastDoseRate || updateClick != lastCountRate ) && countRate > updateClicks ) {
            
            //If its null find the text mesh in the game
            if ( textMesh == null ) {

                textMesh = GameObject.Find("Controller text").GetComponent<TextMesh>(); //Name of plane on controller

            }

            if ( textMesh != null ) {

                textMesh.text = Math.Round(doseRate , 2) + "\n" + "mSv/h" + "\n" + Math.Round(countRate / 60 , 2) + "\n" + "CPM"; //Rounds to two decimals and updates text

            }

            doseRate = lastDoseRate;
            countRate = lastDoseRate;
            updateClick = 0;

        }

        updateClick++;

    }

    private bool lastState = false;

    private void hitScan() { 
    

        float input = SteamVR_Input.__actions_default_in_ControllerTrigger.GetAxis(SteamVR_Input_Sources.Any);

        bool triggerDown = input == 1f;

        audioSource.mute = !triggerDown;

        if ( lastState != triggerDown ){

            lastState = triggerDown;

            if ( lastState ){

                render.material = litMaterial;

                RaycastHit hit;

                if ( Physics.Raycast(transform.position , transform.TransformDirection(Vector3.forward) , out hit , 500f) ){

                    processHit(hit.transform.gameObject);

                }


            }
            else{

                render.material = unlitMaterial;

            }

        }

    }
    
    private void processHit( GameObject hit ){

        if ( hit.GetComponent<Source>() != null ){

            InfoWindow infoWindow = hit.GetComponent<InfoWindow>();

            if ( infoWindow != null ){

                infoWindow.toggleWindow();

            }

        }

    }

}
