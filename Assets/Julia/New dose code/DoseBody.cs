using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoseBody : MonoBehaviour {

    public abstract List<DoseReceptor> getDoseReceptors();
    public abstract void secondaryStart();

    private float doseRate;
    private float countRate;
    private Transform hiddenTransform;


    public Transform getTransform() {

        return hiddenTransform;

    }

    void Start() {
        
        hiddenTransform = GetComponent<Transform>();
        secondaryStart();

    }



    public void setDoseRate( float doseRate ) {

        this.doseRate = doseRate;

    }


    public void setCountRate( float countRate ) {

        this.countRate = countRate;

    }

    public float getDoseRate() {

        return doseRate;

    }

    public float getCountRate() {

        return countRate;

    }


}
