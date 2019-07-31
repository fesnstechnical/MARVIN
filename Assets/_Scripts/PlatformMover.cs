using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour {
    
    private float setPoint = 0.5f;

    private float maxX = 9.231f;
    private float minX = 6.04f;

    private float moveSpeed = 0.01f;

    private Transform sourceTransform;
    private Transform detectorTransform;

    private float changeSpeed = 0.1f;

    // Start is called before the first frame update
    void Start() {

        sourceTransform = GameObject.Find( "SuperObject" ).transform;
        detectorTransform = GameObject.Find( "GeigerTool (GammaGun)" ).transform;

    }

    // Update is called once per frame
    void Update() {

        float actualPoint = ( transform.position.x - minX ) / ( maxX - minX );

        if ( actualPoint != setPoint ) {

            float coverDistance = ( actualPoint - setPoint ) * -1f;
            
            if ( moveSpeed > Mathf.Abs( coverDistance ) ) {

                transform.position = new Vector3( ( setPoint * ( maxX - minX ) ) + minX , transform.position.y , transform.position.z );

            }
            else {

                transform.position = new Vector3( transform.position.x + moveSpeed * ( coverDistance < 0 ? -1 : 1 ) , transform.position.y , transform.position.z );

            }

        }
        
    }

    public float getDistanceBetweenSourceEtDetector() {

        return ( sourceTransform.position - detectorTransform.position ).magnitude; //Magnitude POP POP

    }

    public void incrementPlatform() {

        if ( setPoint + changeSpeed > 1 ) {

            setPoint = 1;

        }
        else {

            setPoint += changeSpeed;

        }

    }

    public void decrementPlatform() {

        if ( setPoint - changeSpeed < 0 ) {

            setPoint = 0;

        }
        else {

            setPoint -= changeSpeed;

        }

    }
    

}
