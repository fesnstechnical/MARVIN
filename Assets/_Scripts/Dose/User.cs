using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class User : DoseBody {
    
    //Collection of dose receptors
    private List<DoseReceptor> doseReceptors = new List<DoseReceptor>();
    

    public override void secondaryStart() {

        //doseReceptors.Add( new DoseReceptor( 77 / 3 , 1.7f / 6 , GameObject.Find( "LeftHand" ).transform ) );
        //doseReceptors.Add( new DoseReceptor( 77 / 3 , 1.7f / 6 , GameObject.Find( "RightHand" ).transform  ) );
        //doseReceptors.Add( new DoseReceptor( 77 / 3 , 1.7f / 6 , GameObject.Find( "VR Camera" ).transform ) );

        doseReceptors.Add( new DoseReceptor( 77 / 1 , 1.7f / 1 , getTransform() ) );

        setIsPlayer( true );

    }

    public void Update() {



    }

    public override List<DoseReceptor> getDoseReceptors() {

        return doseReceptors;

    }




}
