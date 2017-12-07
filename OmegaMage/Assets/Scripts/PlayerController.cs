using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public GameObject meleeBox;
	public GameObject Fireball;
	public GameObject Waterball;

	float lastMeleeTime = 0f;

	float xAxis, zAxis;
	CustomCharacterController controller;
	CapsuleCollider capCol;
	PlayerStats stats;

	[HideInInspector]
	public Vector3 lastGroundPos;
	[HideInInspector]
	public Vector3 lastFloorNormal;
	int lastFloorLayer;

	Vector3 initialPos;

	GameObject body;
	GameObject camTarg;
	Transform cursor;

	//int mask;

	[HideInInspector]
	public Vector3 floortouch; // Debug info

	// Use this for initialization
	void Start ()
    {
		controller = GetComponent<CustomCharacterController>();
		capCol = GetComponent<CapsuleCollider>();
		stats = GetComponent<PlayerStats> ();

		body = gameObject.transform.Find ("PlayerBody").gameObject;
		camTarg = GameObject.Find ("CameraTarget");
		cursor = GameObject.Find ("CursorTarget").GetComponent<Transform> ();

		//mask = LayerMask.GetMask ("Default","Terrain","Enemies","NPCs");
	}


	// Update is called once per frame
	void Update ()
	{
		if (Input.GetButtonDown ("ChangeElement")) {
			if (Input.GetAxisRaw ("ChangeElement") == 1)
				stats.setNextElement ();
			else if (Input.GetAxisRaw ("ChangeElement") == -1)
				stats.setLastElement();
		}
		
		if (Input.GetButtonDown ("Fire1")) // It's a bad idea to put GetButtonDown in a FixedUpdate, so here it is in the regular Update.
			fireProjectile ();
		if (Input.GetAxisRaw ("Fire1") == 1)
			stats.elementalPowers [stats.currentElement].energy -= 0.5f;
		else
			stats.elementalPowers [stats.currentElement].energy += 0.5f;

		foreach (elementSlot e in stats.elementalPowers) {
			if (e != stats.elementalPowers [stats.currentElement])
				e.energy += 0.5f;
		}

		if (Input.GetButtonDown ("Fire2")) // It's a bad idea to put GetButtonDown in a FixedUpdate, so here it is in the regular Update.
			meleeAttack ();

		if (Input.GetButtonDown ("Jump")) // JORMP?
			controller.impulse = Vector3.up;
		
		// Keep controls aligned with camera
		var camRot = camTarg.transform.eulerAngles; // Camera rotation
		//var selfRot = transform.eulerAngles; // Cached rotation that we'll set the thing back to

		//transform.eulerAngles = new Vector3(selfRot.x, camRot.y, selfRot.z);

		// Get the input!
		// Directionally even, but no smoothing
		xAxis = Input.GetAxisRaw ("Horizontal");
		zAxis = Input.GetAxisRaw ("Vertical");

		Vector3 dir = Quaternion.Euler(0f, camRot.y, 0f) * new Vector3 (xAxis, 0f, zAxis) ;

		if (Time.time - lastMeleeTime > 0.25f) {
			// Gotta tell the controller what our movements are...
			controller.xMove = dir.x;
			controller.zMove = dir.z;
		} else {
			controller.xMove = 0f;
			controller.zMove = 0f;
		}

		// Rotate back to normal
		//transform.eulerAngles = new Vector3(selfRot.x, selfRot.y, selfRot.z);

		faceCursor ();


		// EXTRA JUNK

		/*
		if (controller.isGrounded == true) {
				body.transform.localScale = new Vector3 (1f, 1f, 1f);
		} else if (controller.isGrounded == false) {
				body.transform.localScale = new Vector3 (0.9f, 1.2f, 0.9f);
		}
		*/

		//TimeScaleToggle (0.2f);
	}

	void faceCursor()
	{
		// Face Cursor
		Vector3 relPos = cursor.position - body.transform.position;
		Quaternion targRot = Quaternion.LookRotation(relPos);
		body.transform.rotation = Quaternion.Lerp(body.transform.rotation, targRot, 0.15f);

		//body.transform.LookAt (cursor);
		body.transform.eulerAngles = new Vector3 (0, body.transform.eulerAngles.y, 0); // Keep the thing from looking up and down or rolling side to side.
	}

	void fireProjectile()
	{
		if (stats.elementalPowers [stats.currentElement].energy > 20) {
			Vector3 playerHalfHeight = new Vector3 (0f, (capCol.height / 2), 0f);
			Vector3 playerCenter = (transform.position + playerHalfHeight);

			var firePoint = playerCenter + (cursor.transform.position - playerCenter).normalized;
			Quaternion fireDir = Quaternion.LookRotation ((cursor.transform.position - playerCenter).normalized);
			//Rigidbody projClone = (Rigidbody) Instantiate (Projectile, firePoint, body.transform.rotation);
			if (stats.elementalPowers [stats.currentElement].element == elements.fire)
				Instantiate (Fireball, firePoint, fireDir);
			else if (stats.elementalPowers [stats.currentElement].element == elements.water)
				Instantiate (Waterball, firePoint, fireDir);
			else
				Instantiate (Fireball, firePoint, fireDir);

			stats.elementalPowers [stats.currentElement].energy -= 20;
		}
	}

	void meleeAttack()
	{
		body.transform.LookAt (cursor);
		body.transform.eulerAngles = new Vector3 (0, body.transform.eulerAngles.y, 0); // Keep the thing from looking up and down or rolling side to side.

		GameObject box = Instantiate (meleeBox, body.transform.position, Quaternion.identity) as GameObject;
		box.GetComponent<ParentConstraint> ().parent = body;

		lastMeleeTime = Time.time;
	}

	/*void TimeScaleToggle(float t)
	{
		if (Input.GetButtonDown ("Fire2")) {
			if (Time.timeScale == 1.0f)
				Time.timeScale = t;
			else
				Time.timeScale = 1.0f;
		}
	}*/

	void DrawDebugPoint(Vector3 point, Color color, float duration)
	{
		Debug.DrawLine (point + Vector3.down, point + Vector3.up, color, duration);
		Debug.DrawLine (point + Vector3.left, point + Vector3.right, color, duration);
		Debug.DrawLine (point + Vector3.back, point + Vector3.forward, color, duration);
	}

}