using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionModeController : MonoBehaviour {
    
    private Dictionary<GameObject , Material> originalMaterials;

    private const float X_RAY_MIN_ALPHA = 0.1f;


    void Start() {

    }
    
    void Update() {

    }

    public void setModeNormal() {

        if ( GameObject.Find( "Gamma gun mega shield" ) != null ) { GameObject.Find( "Gamma gun mega shield" ).GetComponent<Renderer>().enabled = false; }
        
        if ( originalMaterials != null ) {

            foreach ( GameObject gameObject in originalMaterials.Keys ) {

                if ( gameObject.GetComponent<Renderer>() != null ) { //Should never be null, but just in case of an oopsies
                    
                    gameObject.GetComponent<Renderer>().material = originalMaterials[ gameObject ];

                }

            }

        }

    }

    public void setModeXRay() {

        setMode( "X-ray" );

    }

    public void setInformationBlockHighlightState( bool state ) {

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<InformationBlock>() != null ) {

                gameObject.GetComponent<InformationBlock>().setHighlightState( state );

            }

        }

    }

    private void setMode( string name ) {

        setModeNormal();

        if ( name == "X-ray" ) {

            if ( GameObject.Find( "Gamma gun mega shield" ) != null ) { GameObject.Find( "Gamma gun mega shield" ).GetComponent<Renderer>().enabled = true; }

            originalMaterials = new Dictionary<GameObject, Material>();

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

            foreach( GameObject gameObject in allObjects ) {

                if ( gameObject.GetComponent<Renderer>() != null ) {


                    float attenuationConstant = gameObject.GetComponent<Shield>() != null ? gameObject.GetComponent<Shield>().getAverageAttenuationCoefficient() : 0;

                    //Record the material so we can set it back to normal later
                    Material material = gameObject.GetComponent<Renderer>().material;

                    if ( material.HasProperty( "_Color" ) ) {

                        originalMaterials[ gameObject ] = material;
                        Material newMaterial = Resources.Load( "X_Ray_Base" , typeof( Material ) ) as Material;


                        gameObject.GetComponent<Renderer>().material = newMaterial;

                        Color color = newMaterial.color;
                        color.a = attenuationConstant == 0 ? X_RAY_MIN_ALPHA : X_RAY_MIN_ALPHA + ( ( Mathf.Log10( attenuationConstant ) / 2 ) * ( 1 - X_RAY_MIN_ALPHA ) );

                        gameObject.GetComponent<Renderer>().material.color = color;
                        
                    }


                }

            }

        }

    }

}
