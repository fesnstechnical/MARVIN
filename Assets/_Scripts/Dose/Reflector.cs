using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflector : MonoBehaviour {

    private float doseAlbedo = 0f;
    private float doseAlbedoTemp = 0f;

    public Types reflector;

    public static string[] names = new string[] { "Water" , "Concrete (Ordinary)" , "Iron" , "Lead" };

    public string getName() {

        return names[ ( int ) reflector ];

    }

    public float getSurfaceArea() {

        return 0.01f;

    }

    public void setDoseAlbedo( float doseAlbedo ) {

        this.doseAlbedo = doseAlbedo;

    }

    public void resetDoseAlbedo() {

        doseAlbedoTemp = 0f;

    }

    public void finalizeDoseAlbedo() {

        doseAlbedo = doseAlbedoTemp;

    }

    public void addDoseAlbedo( float doseAlbedo ) {
    
        this.doseAlbedoTemp += doseAlbedo;

    }

    public float getDoseAlbedo() {

        return this.doseAlbedo;
        
    }

    public Collider getCollider() {

        return GetComponent<Collider>();

    }

    public Vector3 getPosistion() {

        return transform.position;

    }


    public enum Types {

        Water,
        Concrete,
        Iron,
        Lead

    }
}
