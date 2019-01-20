using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoseReceptor {

    //Used to store information

    private double mass; //Constant
    private double surfaceArea; //Constant
    private Transform receptorTransform;

    public DoseReceptor( double mass , double surfaceArea , Transform receptorTransform) {

        this.mass = mass;
        this.surfaceArea = surfaceArea;
        this.receptorTransform = receptorTransform;

    }

    public double getMass() {

        return mass;

    }

    public double getSurfaceArea() {

        return surfaceArea;

    }

    public Vector3 getPosistion() {

        return receptorTransform.position;


    }

}
