using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoWindow : MonoBehaviour {

	private GameObject infoWindow;
	private TextMesh mesh;

	// Use this for initialization
	void Start () {
	
		infoWindow = new GameObject("InfoWindow");

		mesh = infoWindow.AddComponent<TextMesh> ();

		infoWindow.transform.localPosition = transform.localPosition + new Vector3 (0, 0.1f, 0);
		infoWindow.transform.localScale = new Vector3 ( 0.05f , 0.05f , 0.05f );

		mesh.offsetZ = 0;
		mesh.characterSize = 1;
		mesh.lineSpacing = 1;
		mesh.anchor = TextAnchor.UpperCenter;
		mesh.alignment = TextAlignment.Center;
		mesh.tabSize = 4;
		mesh.fontSize = 20;
		mesh.richText = true;

		mesh.color = Color.cyan;

	}

	private bool show = false;
	
	// Update is called once per frame
	void Update () {

		if (show) {

			if (name == "source") {

				source sourceCode = GetComponent<source> ();
				mesh.text = sourceCode.radioNuke + "\n" + sourceCode.sourceActivity + " mCi";

			}

		}
		else {

			mesh.text = "";

		}

	}

	public void showWindow(){

		show = true;

	}

	public void hideWindow(){

		show = false;

	}

	public void toggleWindow(){

		show = !show;

	}


}
