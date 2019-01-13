using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class sonic_controller : MonoBehaviour {
    
	private AudioSource audioSource;
	private Renderer render;

	public Material unlitMaterial;
	public Material litMaterial;


	// Use this for initialization
	void Start () {

		audioSource = GetComponent<AudioSource> ();
		render = GetComponent<Renderer> ();

	}

	private bool lastState = false;

	// Update is called once per frame
	void Update () {
	
		float input = SteamVR_Input.__actions_default_in_ControllerTrigger.GetAxis (SteamVR_Input_Sources.Any);

		bool triggerDown = input == 1f;

		audioSource.mute = !triggerDown;

		if (lastState != triggerDown) {

			lastState = triggerDown;

			if (lastState) {

				render.material = litMaterial;

				RaycastHit hit;

				if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit, 500f)) {

					processHit( hit.transform.gameObject );

				}


			} else {

				render.material = unlitMaterial;

			}

		}


	}

	private void processHit( GameObject hit ){

		if (hit.name == "source") {

			InfoWindow infoWindow = hit.GetComponent<InfoWindow> ();

			if ( infoWindow != null ) {

				infoWindow.toggleWindow ();

			}

		}

	}


}
