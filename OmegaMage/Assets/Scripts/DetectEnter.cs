using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class DetectEnter : MonoBehaviour {

	public string tag;
	public UnityEvent meth = new UnityEvent ();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == tag)
			meth.Invoke ();
	}

}