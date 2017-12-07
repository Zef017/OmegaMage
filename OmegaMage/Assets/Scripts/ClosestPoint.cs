using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestPoint : MonoBehaviour {

	public Transform point1;
	public Transform point2;
	public Transform pointMiddle;
	public bool lerp;
	public float lerpValue = 0.1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (lerp == false)
			transform.position = ClosestPointOnLine (point1.position, point2.position, pointMiddle.position);
		else {
			transform.position = Vector3.Lerp (transform.position, ClosestPointOnLine (point1.position, point2.position, pointMiddle.position), lerpValue);
		}
	}

	Vector3 ClosestPointOnLine(Vector3 V1, Vector3 V2, Vector3 point)
	{
		var Vec1 = point - V1;
		var Vec2 = (V2 - V1).normalized;

		var dist = Vector3.Distance(V1, V2);
		var dot = Vector3.Dot(Vec2, Vec1);

		if (dot <= 0)
			return V1;

		if (dot >= dist)
			return V2;

		var Vec3 = Vec2 * dot;

		var ClosestPoint = V1 + Vec3;

		return ClosestPoint;
	}
}
