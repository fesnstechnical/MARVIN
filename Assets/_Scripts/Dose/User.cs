using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class User : DoseBody {
    
    //Collection of dose receptors
    private List<DoseReceptor> doseReceptors = new List<DoseReceptor>();
    

    public override void secondaryStart() {

        String[] tags = new String[] { "LeftHand" , "RightHand" , "VRCamera" , "BodyCollider" };

        for ( int i = 0 ; i < tags.Length ; i++ ) {

            doseReceptors.Add( new DoseReceptor( 77 / tags.Length , 1.7f / tags.Length , GameObject.Find( tags[ i ] ).transform ) );

        }

        

        //doseReceptors.Add( new DoseReceptor( 77 / 1 , 1.7f / 1 , getTransform() ) );

        setIsPlayer( true );

    }

    public void Update() {



    }

    public override List<DoseReceptor> getDoseReceptors() {

        return doseReceptors;

    }




}
