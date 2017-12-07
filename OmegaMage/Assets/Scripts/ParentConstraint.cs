using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentConstraint : MonoBehaviour {

	public GameObject parent;
	public bool matchRotation = true;
	public Vector3 offset;
	public bool destroyWithoutParent = false;

	// Use this for initialization
	void Start () {
		constrain ();
	}
	
	// Update is called once per frame
	void Update () {
		constrain ();
	}

	void constrain() {
		if (parent != null) {
			transform.position = parent.transform.position + offset;

			if (matchRotation) {
				transform.rotation = parent.transform.rotation;
			}
		} else if (destroyWithoutParent) {
			Destroy (gameObject);
		}
	}
}
