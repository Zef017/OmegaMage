using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacterController : MonoBehaviour {

	public float speed;
	public float knockbackWeight = 0.15f;

	float gravity = -1f;
	float slopeLimit = 0.707f;
	[HideInInspector]
	public bool isGrounded;

	[HideInInspector]
	public float ySpeed = 0f;
	float deltaTime;

	[HideInInspector]
	public Vector3 impulse = Vector3.zero; // For stuff like getting bonked around

	Vector3 movement;
	[HideInInspector]
	public float xMove, zMove;
	[HideInInspector]
	public CapsuleCollider capCol;
	//Vector3 capsuleStart;
	//Vector3 capsuleEnd;
	[HideInInspector]
	public Vector3 center;
	[HideInInspector]
	public float radius;
	[HideInInspector]
	public float height;
	Vector3 footOffset;

	[HideInInspector]
	public Vector3 lastGroundPos;
	[HideInInspector]
	public Vector3 lastFloorNormal;
	int lastFloorLayer;

	[HideInInspector]
	public Vector3 lastValidGroundPos;
	[HideInInspector]
	public Vector3 lastValidFloorNormal;

	Vector3 initialPos;

	GameObject camTarg;
	Transform cursor;

	int mask;
	int standMask;

	[HideInInspector]
	public Vector3 floortouch; // Debug info

	// Use this for initialization
	void Start ()
	{
		capCol = GetComponent<CapsuleCollider>();

		center = new Vector3(capCol.center.x * transform.localScale.x, capCol.center.y * transform.localScale.y, capCol.center.z * transform.localScale.z);
		if (transform.localScale.z > transform.localScale.x)
			radius = capCol.radius * transform.localScale.z;
		else
			radius = capCol.radius * transform.localScale.x;

		if (capCol.radius * 2 <= capCol.height)
			height = capCol.height * transform.localScale.y;
		else
			height = radius * 2 * transform.localScale.y;

		//updateCapsuleInfo ();


		camTarg = GameObject.Find ("CameraTarget");
		cursor = GameObject.Find ("CursorTarget").GetComponent<Transform> ();

		mask = LayerMask.GetMask ("Default","Player","Terrain","Enemies","NPCs");
		standMask = LayerMask.GetMask ("Default","Terrain");

		footOffset = new Vector3(0f, center.y - (height/2), 0f);
		/*if (capCol.height >= capCol.radius*2)
			footOffset = new Vector3(0f, center.y - (height/2), 0f);
		else
			footOffset = new Vector3(0f, center.y - (radius), 0f);*/
	}


	// Update is called once per frame
	public void Update ()
	{
		float delta = Time.deltaTime;
		while (delta > Time.fixedDeltaTime) {
			deltaTime = Time.fixedDeltaTime;
			SingleUpdate ();
			delta -= Time.fixedDeltaTime;
		}

		if (delta > 0f) {
			deltaTime = delta;
			SingleUpdate ();
		}

		xMove = 0f;
		zMove = 0f;
	}

	void SingleUpdate () // I'm ripping off code by the Super Mario 64 HD guy, I know...
	{
		initialPos = transform.position;

		moveCharacter ();

		checkGround ();

		pushback(); // Three pushbacks are better than one, since a single pushback could just leave us stuck in a different wall.
		pushback(); // This is especially useful with mesh colliders, which are counted as one collision despite having potentially concave sections.
		pushback(); // I could potentially optimise it to be a single recursive function, but I'll do that later...

		//checkGround ();
		//actGrounded ();

		checkGround ();
		SlopeLimit ();

		checkGround ();
		actGrounded ();

		//DrawDebugPoint (initialPos, Color.red, deltaTime);
		//DrawDebugPoint (transform.position, Color.yellow, deltaTime);

		//pushback();
	}

	void moveCharacter()
	{
		movement.Set (0f,0f,0f); // This is the movement vector that will be added to the character's position at the end of the function.

		// It's kind of complicated, but these are the new X and Z axes.
		Vector3 forwardVector;
		Vector3 rightVector;

		// if grounded, make the object's X and Z axes lie on the floor plane. This keeps surface movement speed consistent across slopes.
		// Without this, we'd speed up steep slopes at lightning speed.
		// Crazily enough, the way this code works out, the X and Z axes won't always be orthogonal to each other in three dimensions.
		// They only appear to be consistently orthogonal when viewed from above, and that's what I want.
		if ( isGrounded == true && !(lastFloorLayer == (lastFloorLayer | 1 << standMask))) {
		//if (isGrounded == true && lastFloorLayer != LayerMask.NameToLayer ("Enemies") && lastFloorLayer != LayerMask.NameToLayer ("NPCs")) {
			// Not if we're standing on a character, though! We want to be able to dismount an enemy quickly if we land on one somehow.
			Plane standPlane = new Plane(lastFloorNormal, transform.position);
			float rayDistance;

			Ray planeRay = new Ray(transform.position + transform.forward, Vector3.up);
			standPlane.Raycast (planeRay, out rayDistance);
			forwardVector = (planeRay.GetPoint (rayDistance) - transform.position).normalized; // Set forward axis (Z)

			planeRay = new Ray(transform.position + transform.right, Vector3.up);
			standPlane.Raycast (planeRay, out rayDistance);
			rightVector = (planeRay.GetPoint (rayDistance) - transform.position).normalized; // Set right axis (X)
		} else { // Obviously, if we're not on the ground, we don't want that.
			forwardVector = transform.forward;
			rightVector = transform.right;
		}

		//Debug.DrawLine (transform.position, (transform.position + (lastFloorNormal * 2f)), Color.green, 3f); // Show up normal
		//Debug.DrawLine (transform.position, (transform.position + (forwardVector * 2f)), Color.red, 3f); // Show forward normal
		//Debug.DrawLine (transform.position, (transform.position + (rightVector * 2f)), Color.red, 3f); // Show right normal
		//Debug.DrawLine (transform.position, (transform.position + (transform.forward * 2f)), Color.yellow, 3f); // Show true forward normal
		//Debug.DrawLine (transform.position, (transform.position + (transform.right * 2f)), Color.yellow, 3f); // Show true right normal

		//transform.eulerAngles = new Vector3(selfRot.x, camRot.y, selfRot.z);
		//

		// Movement

		//movement = transform.TransformDirection(movement);
		// Puttin' it back to local. If we don't do this, our camera alignment efforts will be for nothing.
		// Well, for the player, at least. The enemies obvously don't have to worry about camera-aligned controls.

		movement += (xMove * rightVector) + (zMove * forwardVector);
		movement = movement.normalized * deltaTime * speed;
		transform.position += movement;

		transform.position += impulse;
		impulse = Vector3.Lerp (impulse, Vector3.zero, Mathf.Clamp(knockbackWeight, 0f, 1f));
	}

	void pushback() // Push Player out of walls
	{
		//updateCapsuleInfo ();

		foreach (Collider col in Physics.OverlapCapsule (capsuleStart, capsuleEnd, radius, mask)) { // For each collider the characcter collides with...
			if (col.isTrigger) // Ignore triggers
				continue;
			if (col == capCol) // Obviously, we don't want to collide with ourself...
				continue;

			Vector3 contactPoint = SuperCollider.ClosestPointOnSurface (col, transform.position + center, radius); // Get contact point
			//DrawDebugPoint(contactPoint, Color.green, deltaTime); // Draw collision contact point

			if (contactPoint != Vector3.zero) {

				// The point in between the capsule collider's start and end points that is closest to the contact point
				Vector3 positionPoint = ClosestPointOnLine(capsuleStart, capsuleEnd, contactPoint);
				//DrawDebugPoint(positionPoint, Color.red, deltaTime * 10); // Draw the thing I just described

				Vector3 v = contactPoint - positionPoint;
				if (v != Vector3.zero) {

					// Cache the collider's layer so that we can cast against it
					int layer = col.gameObject.layer;

					int TempLayer = LayerMask.NameToLayer ("Temporary");
					col.gameObject.layer = TempLayer;

					// Check which side of the normal we are on
					bool facingNormal = Physics.SphereCast (new Ray (positionPoint, v.normalized), 0.01f, v.magnitude + 0.01f, 1 << TempLayer);

					col.gameObject.layer = layer; // Change the layer back

					// Orient and scale our vector based on which side of the normal we are situated
					if (facingNormal) {
						if (Vector3.Distance (positionPoint, contactPoint) < radius) {
							v = v.normalized * (radius - v.magnitude) * -1;
						} else {
							// A previously resolved collision has had a side effect that moved us outside this collider
							continue;
						}
					} else {
						v = v.normalized * (radius + v.magnitude);
					}
				}

				transform.position += v;
				//transform.position += Vector3.ClampMagnitude (v, Mathf.Clamp (capCol.radius - v.magnitude, 0, capCol.radius));
			}
		}
	}

	void checkGround() // Are we grounded?
	{
		//updateCapsuleInfo ();

		Color touchColor = Color.green;

		RaycastHit floorHit;
		//if (Physics.Raycast (new Ray (transform.position + Vector3.up, Vector3.down), out floorHit, 2.0f, mask, QueryTriggerInteraction.Ignore)) {
		//if (Physics.SphereCast (new Ray (transform.position + Vector3.up, Vector3.down), capCol.radius - (0.01f), out floorHit, 1.5f, mask, QueryTriggerInteraction.Ignore)) {
		//if (Physics.SphereCast (new Ray (capsuleStart, Vector3.down), capCol.radius - (0.01f), out floorHit, 1.0f, mask, QueryTriggerInteraction.Ignore)) {
		if (Physics.SphereCast (new Ray (capsuleStart + (Vector3.up*radius), Vector3.down), radius - (0.01f), out floorHit, radius + (0.5f * transform.localScale.y), standMask, QueryTriggerInteraction.Ignore)) {
			if (floorHit.point.y <= capsuleStart.y) {
				isGrounded = true;

				//DrawDebugPoint (floorHit.point, Color.cyan, deltaTime); // Draw the point where the character hits the floor

				lastGroundPos = transform.position + (Vector3.up * (floorHit.point - transform.position).y) - footOffset; // Push the character onto the ground;

				// This next section is to get more accurate normals on corners. RaycastHits return interpolated corner normals. We don't want that.
				if (floorHit.normal == Vector3.up)
					lastFloorNormal = floorHit.normal;
				else {
					Vector3 sideTemp = Vector3.Cross(floorHit.normal, Vector3.up).normalized;
					Vector3 nearDir = Vector3.Cross(sideTemp, floorHit.normal).normalized;
					Vector3 nearPoint = floorHit.point + (nearDir * 0.01f);
					Vector3 farPoint = floorHit.point - (nearDir * 0.01f);

					//Debug.DrawLine (nearPoint, (nearPoint + (sideTemp.normalized * 2f)), Color.red, 3f); // Show sideTemp direction
					//Debug.DrawLine (nearPoint, (nearPoint + (nearDir.normalized * 2f)), Color.blue, 3f); // Show nearDir direction

					RaycastHit nearHit, farHit;
					Vector3 highestNorm;

					if (Physics.Raycast (nearPoint + (floorHit.normal*0.5f), (-floorHit.normal), out nearHit, 1.0f, standMask, QueryTriggerInteraction.Ignore)
						&&	Physics.Raycast (farPoint + (floorHit.normal*0.5f), (-floorHit.normal), out farHit, 1.0f, standMask, QueryTriggerInteraction.Ignore)) {
						if (Vector3.Dot (Vector3.up, nearHit.normal) > Vector3.Dot (Vector3.up, farHit.normal))
							highestNorm = nearHit.normal;
						else
							highestNorm = farHit.normal;

						if (Vector3.Dot (Vector3.up, floorHit.normal) > Vector3.Dot (Vector3.up, highestNorm))
							highestNorm = floorHit.normal;

						lastFloorNormal = highestNorm;
					} else {
						lastFloorNormal = floorHit.normal;
					}
				}

				lastFloorLayer = floorHit.collider.gameObject.layer;

				floortouch = floorHit.point;

				if (!SlopeOK (lastFloorNormal))
					isGrounded = true;	// Okay, this looks pointless, but it's a temporary fix, okay?	--- (!!!) --- (was "isGrounded = true;")
				else {
					lastValidGroundPos = transform.position + (Vector3.up * (floorHit.point - transform.position).y) - footOffset; // Push the character onto the ground;
					lastValidFloorNormal = floorHit.normal;
					// I put this over here because we want something that definitely won't be an invalidly sloped floor
				}

			} else {
				isGrounded = false;
				touchColor = Color.red;
			}
		} else {
			isGrounded = false;
		}
		DrawDebugPoint (floorHit.point, touchColor, 0.5f);
	}

	void actGrounded() // Here's what to do if we're on the ground. (and also what to do if we aren't)
	{
		if (isGrounded == true && SlopeOK(lastFloorNormal)) {	// Okay, this looks weird, but it's a temporary fix, okay?	--- (!!!)
			ySpeed = 0.0f;

			DrawDebugPoint((capsuleStart), Color.yellow, deltaTime);
			DrawDebugPoint((capsuleEnd), Color.red, deltaTime);
			DrawDebugPoint(capsuleStart + (Vector3.down * radius), Color.magenta, deltaTime);
			DrawDebugPoint(capsuleEnd + (Vector3.up * radius), Color.magenta, deltaTime);
			//DrawDebugPoint(transform.position + (footOffset), Color.blue, deltaTime);


			transform.position = lastGroundPos;
		} else if (isGrounded == false) {
			ySpeed += gravity * deltaTime; // Gravitational acceleration!
			transform.position += (new Vector3(0f, ySpeed, 0f));
			//if (controller.isGrounded == true)
		}
	}

	bool SlopeOK (Vector3 norm)
	{
		if (Vector3.Dot (Vector3.up, norm) > slopeLimit)
			return true;
		else
			return false;
	}

	bool SlopeLimit()
	{
		if (isGrounded && !SlopeOK(lastFloorNormal) && (lastGroundPos.y > initialPos.y))
		{
			Vector3 absoluteMoveDirection = (transform.position - initialPos) - (Vector3.Dot((transform.position - initialPos), lastFloorNormal) * lastFloorNormal);
			//Debug.DrawLine (initialPos, initialPos + absoluteMoveDirection, Color.red, 3f); // Show absolute move direction

			// Retrieve a vector pointing down the slope
			Vector3 r = Vector3.Cross(lastFloorNormal, Vector3.down);	// r points to the side, perpendicular to v
			Vector3 v = Vector3.Cross(r, lastFloorNormal);				// v points directly down the slope

			//Debug.DrawLine (transform.position, (transform.position + (r.normalized * 2f)), Color.blue, 3f); // Show r direction
			//Debug.DrawLine (transform.position, (transform.position + (v.normalized * 2f)), Color.red, 3f); // Show r direction+

			float angle = Vector3.Angle(absoluteMoveDirection, v);

			if (angle <= 90.0f)
				return false;
			
			// Calculate where to place the controller on the slope, or at the bottom, based on the desired movement distance
			//Vector3 resolvedPosition = initialPos + r * Vector3.Dot((transform.position - initialPos), r);
			//Vector3 direction = (resolvedPosition - transform.position) - (Vector3.Dot((resolvedPosition - transform.position), lastFloorNormal) * lastFloorNormal);
			Vector3 resolvedPosition = ProjectPointOnLine(initialPos, r, lastGroundPos);
			Vector3 direction = ProjectVectorOnPlane(lastFloorNormal, resolvedPosition - lastGroundPos);

			//Debug.DrawLine (transform.position, (transform.position + (direction.normalized * 2f)), Color.red, 3f); // Show direction direction
			Debug.DrawLine (transform.position, (transform.position + direction), Color.red, 3f); // Show direction
			Vector3 bluhPos = transform.position;
			RaycastHit hit;

			//updateCapsuleInfo ();
			// Check if our path to our resolved position is blocked by any colliders
			if (Physics.CapsuleCast(capsuleStart, capsuleEnd, radius - (0.01f), direction.normalized, out hit, direction.magnitude, mask))
			{
				Debug.DrawLine (capsuleEnd, capsuleEnd + Vector3.up, Color.magenta, 3f);
				transform.position += v.normalized * hit.distance;
			}
			else
			{
				Debug.DrawLine (capsuleEnd, capsuleEnd + Vector3.up, Color.green, 3f);
				//transform.position += direction;
			}

			return true;
		}

		return false;
	}

	/*
	void updateCapsuleInfo()
	{
		capsuleStart = transform.position + center - new Vector3(0f, (height/2) - radius, 0f); // Bottom Center
		capsuleEnd = transform.position + center + new Vector3(0f, (height/2) - radius, 0f); // Top Center
	}

	public Vector3 CapsuleStart()
	{
		return transform.position + center - new Vector3(0f, (height/2) - radius, 0f); // Bottom Center
	}

	public Vector3 CapsuleEnd()
	{
		return transform.position + center + new Vector3(0f, (height/2) - radius, 0f); // Top Center
	}
	*/

	public Vector3 capsuleStart {
		get {
			return transform.position + center - new Vector3(0f, (height/2) - radius, 0f); // Bottom Center
		}
	}
	public Vector3 capsuleEnd {
		get {
			return transform.position + center + new Vector3(0f, (height/2) - radius, 0f); // Top Center
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

	Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
	{

		//get vector from point on line to point in space
		Vector3 linePointToPoint = point - linePoint;

		float t = Vector3.Dot(linePointToPoint, lineVec);

		return linePoint + lineVec * t;
	}

	public static Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
	{

		return vector - (Vector3.Dot(vector, planeNormal) * planeNormal);
	}

	void DrawDebugPoint(Vector3 point, Color color, float duration)
	{
		Debug.DrawLine (point + (Vector3.down / 2), point + (Vector3.up / 2), color, duration);
		Debug.DrawLine (point + (Vector3.left / 2), point + (Vector3.right / 2), color, duration);
		Debug.DrawLine (point + (Vector3.back / 2), point + (Vector3.forward / 2), color, duration);
	}

}