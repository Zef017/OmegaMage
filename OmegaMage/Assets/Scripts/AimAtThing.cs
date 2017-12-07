using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAtThing : MonoBehaviour {

	public Transform target;
	public bool LockXZ;
	public bool Lerp = true;
	public float LerpValue = 0.15f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!Lerp)
			transform.LookAt (target.position);
		else {
			Vector3 relPos = target.position - transform.position;
			Quaternion targRot = Quaternion.LookRotation(relPos);
			transform.rotation = Quaternion.Lerp (transform.rotation, targRot, LerpValue);
		}

		if (LockXZ == true)
			transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0); // Keep the thing from looking up and down or rolling side to side.
	}
}
