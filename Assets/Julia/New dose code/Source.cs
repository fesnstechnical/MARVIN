using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Source : MonoBehaviour {

   
    public RadioIsotopes radioIsotope = 0;
    public float initialActivity;

    private float[] halfLife = new float[]{ 9.51441e+8F }; //In seconds
    private float[] energies = new float[]{ 661.7F }; //In keV

    
    private Transform sourceTransform;

    //Used to contain information
    void Start() {

        this.sourceTransform = GetComponent<Transform>();

    }

    void Update() {
        


    }

    public Vector3 getPosistion() {

        if ( this.sourceTransform == null ){

            return new Vector3(0 , 0 , 0);

        }
        

        return sourceTransform.position;

    }

    public float getActivity() {

        return initialActivity;

    }

    public float getHalfLife() {

        if ( ( int ) radioIsotope < halfLife.Length ) {

            return halfLife[( int )radioIsotope];

        }

        return 0;

    }

    public float getParticleEnergy() {

        if ( ( int ) radioIsotope < energies.Length ) {

            return energies[ ( int ) radioIsotope ];

        }

        return 0;

    }

    public enum RadioIsotopes {

        Cs_137=0,

    };

}
