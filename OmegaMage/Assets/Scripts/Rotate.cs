using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	public float angle = 0.5f;
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	

	void FixedUpdate () {
		if (rb != null)
			rb.MoveRotation (transform.rotation * Quaternion.Euler (0f, angle, 0f));
		else
			transform.Rotate (0f, angle, 0f);
	}
}
