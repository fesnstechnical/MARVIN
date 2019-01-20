using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoseBody : MonoBehaviour {

    public abstract List<DoseReceptor> getDoseReceptors();

    private float doseRate;
    private float countRate;
    private Transform transform;


    public Transform getTransform() {

        return transform;

    }

    void Start() {

        transform = GetComponent<Transform>();

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
