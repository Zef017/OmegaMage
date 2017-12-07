using UnityEngine;
using System.Collections;

public class TimedObjectDestructor : MonoBehaviour {

	public float timeOut = 1.0f;
	public bool detachChildren = false;
	public GameObject exitEffect;

	// Use this for initialization
	void Start () {
		// invote the DestroyNow funtion to run after timeOut seconds
		Invoke ("DestroyNow", timeOut);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void DestroyNow ()
	{
		if (exitEffect != null) {
			Instantiate (exitEffect, transform.position, transform.rotation);
		}

		if (detachChildren) { // detach the children before destroying if specified
			transform.DetachChildren ();
		}
		DestroyObject (gameObject);
	}
}
