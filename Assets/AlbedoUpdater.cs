using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlbedoUpdater : MonoBehaviour {

    public Reflector reflector;

    private TextMeshPro textMesh;

    // Start is called before the first frame update
    void Start() {

        textMesh = GetComponent<TextMeshPro>();

    }

    // Update is called once per frame
    void Update() {

        if ( reflector != null && textMesh != null ) {
  
            textMesh.text = "=" + Math.Round( ( double ) reflector.getDoseAlbedo() , 5 );

        }

    }
}
