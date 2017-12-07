using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamControl : MonoBehaviour {

	public float minZoom = 1.5f;

	GameObject CameraHeight;

    // Use this for initialization
    void Start ()
    {
		CameraHeight = gameObject.transform.Find ("CameraHeight").gameObject;
    }
	
	// Update is called once per frame
	void Update ()
    {

        // Camera Rotation
        var rot = Input.GetAxis("CamRotate") * Time.deltaTime * 100.0f;
        transform.Rotate(0,rot,0);

        // Camera Height
        var rotH = Input.GetAxis("CamHeight") * Time.deltaTime * 100.0f;
		CameraHeight.transform.Rotate(rotH, 0, 0);

		//Camera Zoom
		var zoom = Input.GetAxis("CamZoom") * Time.deltaTime * 10.0f;
		Camera.main.orthographicSize += zoom;
		if (Camera.main.orthographicSize < minZoom)
			Camera.main.orthographicSize = minZoom;
    }
}
