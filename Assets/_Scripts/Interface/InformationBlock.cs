using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationBlock : MonoBehaviour {

    private MeshRenderer renderer;
    private Collider collider;

    private bool displayed = false;

    [Header( "Details about the object, to be conveyed to the player" )]
    [TextArea( 5 , 10 )]
    public string information;

    [Header( "Button names and their functions for this information block, max is 3" )]
    public List<InformationBlockExecutable> executables;

 

    // Start is called before the first frame update
    void Start() {
        
        GameObject superObject = new GameObject();
        superObject.name = "CLONE";

        Transform cloneTransform = Instantiate( this.transform , this.transform.position + new Vector3( 0 , 0 , 0 ) , Quaternion.identity ).parent = superObject.transform;

        GameObject clone = cloneTransform.gameObject;

        //Remove everything that isnt a mesh
        List<GameObject> children = new List<GameObject>();
        children = findAllChildren( cloneTransform , children );

        foreach ( GameObject child in children ) {
            

            if ( child.GetComponent<TMPro.TextMeshPro>() != null ) {

                Destroy( child ); //Destroy the child, currupt them all

            }
            else {

          
                Component[] components = child.GetComponents( typeof( Component ) );

                foreach ( Component subComponent in components ) {

                    if ( subComponent.GetType() != typeof( MeshRenderer ) && subComponent.GetType() != typeof( MeshFilter ) && subComponent.GetType() != typeof( RectTransform ) && subComponent.GetType() != typeof( Transform ) && subComponent.GetType() != typeof( CanvasRenderer ) ) {
                        
                        Destroy( subComponent );

                    }

                }

                

            }

        }

        List<GameObject> newChildren = new List<GameObject>();
        List<GameObject> generatedObjects = new List<GameObject>();

        findAllChildren( superObject.transform , newChildren );

        foreach ( GameObject child in newChildren ) {

            if ( child.GetComponent<MeshRenderer>() != null ) {

                generatedObjects.Add( child );

            }

        }

        CombineInstance[] combineSuper = new CombineInstance[ generatedObjects.Count ];


        for ( int i = 0 ; i < combineSuper.Length ; i++ ) {

            combineSuper[ i ].mesh = generatedObjects[ i ].GetComponent<MeshFilter>().sharedMesh;


            Vector3 local = generatedObjects[ i ].transform.localPosition;
            Quaternion localAngle = generatedObjects[ i ].transform.localRotation;

            //generatedObjects[ i ].transform.localPosition = generatedObjects[ i ].transform.position;
            //generatedObjects[ i ].transform.localRotation = generatedObjects[ i ].transform.rotation;
            //generatedObjects[ i ].transform.position = local;
            //generatedObjects[ i ].transform.rotation = localAngle;

            combineSuper[ i ].transform = ( generatedObjects[ i ].transform ).localToWorldMatrix;
            generatedObjects[ i ].gameObject.SetActive( false );

        }


        superObject.transform.localScale = new Vector3( 1f , 1f , 1f );
        superObject.transform.localPosition = new Vector3( 0 , 0 , 0 );
        
        MeshFilter grandFilter = superObject.AddComponent<MeshFilter>();
        MeshRenderer grandRenderer = superObject.AddComponent<MeshRenderer>();

        grandFilter.mesh = new Mesh();

        grandFilter.mesh.CombineMeshes( combineSuper , true );

        grandFilter.sharedMesh = grandFilter.mesh;
        
        superObject.transform.parent = this.transform;

        //grandRenderer.material = Resources.Load( "Outline" , typeof( Material ) ) as Material;

        Outline outline = superObject.AddComponent<Outline>();
        outline.m_TriggerMethod = Outline.ETriggerMethod.AlwaysOn;
        outline.m_OutlineWidth = 0.06f;
        outline.m_OutlineOnly = true;

        renderer = grandRenderer;
        collider = superObject.GetComponent<Collider>();

        setHighlightState( false );

    }

    private List<GameObject> findAllChildren( Transform parent , List<GameObject> runningList ) {
        
        foreach ( Transform child in parent ) {

            runningList.Add( child.gameObject );
            findAllChildren( child , runningList );

        }

        

        return runningList;

    }

    // Update is called once per frame
    void Update() {

    }

    public void setHighlightState( bool state ) {

        if ( renderer != null ) {

            renderer.enabled = state;

        }

        if ( collider != null ) {

            collider.enabled = state;

        }


    }
    
    public void toggleDisplay() {

        displayed = !displayed;

        setHighlightState( displayed );

    }

}
