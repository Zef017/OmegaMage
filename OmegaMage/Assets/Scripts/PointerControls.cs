using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerControls : MonoBehaviour {

	public Plane aimPlane;
	public Transform markerObject;

	GameObject player;
	CapsuleCollider playerCollider;
	[HideInInspector]
	public Vector3 playerHalfHeight;
	[HideInInspector]
	public Vector3 playerCenter;
	[HideInInspector]
	public Vector3 playerSkinHeight;
	Vector3 tinyY;

	[HideInInspector]
	public Vector3 cursorNormal;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
		playerCollider = player.GetComponent<CapsuleCollider> ();
		playerHalfHeight = new Vector3(0f, (playerCollider.height / 2), 0f);
		playerCenter = player.transform.position + playerHalfHeight;
		tinyY = new Vector3(0f, 0.001f, 0f); // To keep the linecast form accidentally intersecting too soon
		playerSkinHeight = new Vector3(0f, 0f, 0f); // That darned skin width interferes with the floor-touching.
		cursorNormal = Vector3.up; // What if we want to know what the normal of the surface we're pointing at is?
	}
	
	// Update is called once per frame
	void LateUpdate () {
		playerCenter = player.transform.position + playerHalfHeight;

		dynamicAim ();

		transform.position = markerObject.position;
	}

	void dynamicAim() { // Targets the ground directly
		RaycastHit floorHit;

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		ray.origin -= playerHalfHeight - playerSkinHeight;

		if (Physics.Raycast (ray, out floorHit, 256, LayerMask.GetMask("Terrain"))) {
			markerObject.position = floorHit.point + playerHalfHeight + playerSkinHeight;
			cursorNormal = floorHit.normal;
			/*if (markerObject.position.y > playerCenter.y) { // If the target is higher than the player
				if (Physics.Linecast (playerCenter, markerObject.position - playerHalfHeight + tinyY, LayerMask.GetMask ("Terrain"))) {
					planeAim (); // If the path between the player and the target is blocked, switch to plane aim mode.
				}
			} else if (markerObject.position.y < playerCenter.y) { // If the target is lower than the player
				if (Physics.Linecast (playerCenter, markerObject.position - playerHalfHeight + tinyY, LayerMask.GetMask ("Terrain"))) {
					planeAim (); // If the path between the player and the target is blocked, switch to plane aim mode.
				}
			} else {
				planeAim ();
			}*/
		} else {
			planeAim ();
			cursorNormal = Vector3.up;
		}
	}

	void planeAim() {
		RaycastHit floorHit;
		Vector3 upNorm;
		if (Physics.Raycast (playerCenter, Vector3.down, out floorHit, 1))
			upNorm = floorHit.normal; // make aimplane parallel to floor
		else
			upNorm = Vector3.up; // If no floor, just go horizontal

		aimPlane = new Plane(upNorm, playerCenter);


		Ray planeRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		float rayDistance;
		if (aimPlane.Raycast (planeRay, out rayDistance))
			markerObject.position = planeRay.GetPoint (rayDistance);
	}

}
