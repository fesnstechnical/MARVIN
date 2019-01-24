using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : DoseBody {

    //Collection of dose receptors

    private List<DoseReceptor> doseReceptors = new List<DoseReceptor>();
    

    public override void secondaryStart() {
        
        doseReceptors.Add(new DoseReceptor(77, 1.7, getTransform()));
   
    }

    public void Update() {

        updateControllerRate(getDoseRate() , getCountRate());

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

                textMesh.text = Math.Round(doseRate , 2) + "\n" + "mSv/h" + "\n" + Math.Round(countRate , 2) + "\n" + "CPS"; //Rounds to two decimals and updates text

            }

            doseRate = lastDoseRate;
            countRate = lastDoseRate;
            updateClick = 0;

        }

        updateClick++;

    }

}
