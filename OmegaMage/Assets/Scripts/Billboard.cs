using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

	public Transform cam;

	// Use this for initialization
	void Start () {
		if (cam == null)
			cam = GameManager.gm.cameraTarget.transform.Find ("CameraHeight").Find ("Main Camera").gameObject.GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		//transform.LookAt (cam); // The old way

		transform.forward = -cam.forward;
	}
}
