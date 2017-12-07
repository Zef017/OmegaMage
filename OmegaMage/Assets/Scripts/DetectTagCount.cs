using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class DetectTagCount : MonoBehaviour {

	public string tag;
	public int tagCount = 0;
	public UnityEvent meth = new UnityEvent();

	public List<Collider> objectList;


	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

		objectList.RemoveAll (obj => obj == null); // Get rid of null colliders

		if (objectList.Count == tagCount)
			meth.Invoke();
	}

}
