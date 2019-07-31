using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlInterface : MonoBehaviour {
    // Start is called before the first frame update

    public TextMeshPro textMeshPro;
    public RotatingHandle rotaingHandle;

    public float minValue;
    public float maxValue;

    [TextArea( 3 , 10 )]
    public string text;

    public string[] textArray;

    private float percentActivation = 0;

    void Start() {

    }

    // Update is called once per frame
    void Update() {

        percentActivation = rotaingHandle.getPercentActivation();

        bool useArray = false;
        if ( textArray != null ) {

            if ( textArray.Length > 0 ) {

                useArray = true;

            }

        }

        if ( useArray ) {

            textMeshPro.text = text.Replace( "{x}" , "" + textArray[ ( int ) getValue() ] );

        }
        else { 

            textMeshPro.text = text.Replace( "{x}" , "" + Math.Round( ( double ) getValue() , 3 ) );

        }

    }

    public float getValue() {

        return ( ( maxValue - minValue ) * percentActivation ) + minValue;

    }

}
