// Man, was this a weird step forward...
// I can't believe I actually wrote a collision handling script from scratch

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCollisionHandler : MonoBehaviour {

	public Collider thisCollider;

	// Unfortunately, I only have this up and running for CapsuleColliders and SphereColliders
	public Vector3 capsuleStart; // bottom of the capsule, if we have a capsule.
	public Vector3 capsuleEnd; // top of the capsule, if we have a capsule.
	public Vector3 center; // the center
	public float radius;
	public float height;

	public Vector3 dimensions; // for a box

	public string[] layerNames; // The names of the layers we want to detect collisions on
	int LMask;

	//[HideInInspector]
	public List<Collider> colQueue;

	public List<Collider> enterCols;
	public List<Collider> currentCols;
	public List<Collider> exitCols;

	// Use this for initialization
	void Start () {
		SetMask (layerNames); // You'd only want to check layers you'll be doing something with in this object's code

		thisCollider = GetComponent<Collider>();
		if (thisCollider.GetType () == typeof(CapsuleCollider)) {
			//Debug.Log (this.gameObject.name + "Capsule!");


			CapsuleCollider capCol = GetComponent<CapsuleCollider> ();
			/*if (capCol != null) {
			capsuleStart = capCol.center - new Vector3(0f, (capCol.height/2) - capCol.radius, 0f); // Bottom Center
			capsuleEnd = capCol.center + new Vector3(0f, (capCol.height/2) - capCol.radius, 0f); // Top Center
			radius = capCol.radius;
			}*/

			//capCol = GetComponent<CapsuleCollider>();
			if (capCol != null) {

				center = new Vector3 (capCol.center.x * transform.localScale.x, capCol.center.y * transform.localScale.y, capCol.center.z * transform.localScale.z);
				if (transform.localScale.z > transform.localScale.x)
					radius = capCol.radius * transform.localScale.z;
				else
					radius = capCol.radius * transform.localScale.x;

				if (capCol.radius * 2 <= capCol.height)
					height = capCol.height * transform.localScale.y;
				else
					height = radius * 2 * transform.localScale.y;

				capsuleStart = center - new Vector3 (0f, (height / 2) - radius, 0f); // Bottom Center
				capsuleEnd = center + new Vector3 (0f, (height / 2) - radius, 0f); // Top Center
				//radius = capCol.radius;
			}
		} else if (thisCollider.GetType () == typeof(SphereCollider)) {
			//Debug.Log (this.gameObject.name + "Sphere!");

			SphereCollider sphCol = GetComponent<SphereCollider> ();
			if (sphCol != null) {

				center = new Vector3 (sphCol.center.x * transform.localScale.x, sphCol.center.y * transform.localScale.y, sphCol.center.z * transform.localScale.z);
				if (transform.localScale.z > transform.localScale.x)
					radius = sphCol.radius * transform.localScale.z;
				else
					radius = sphCol.radius * transform.localScale.x;

				height = sphCol.radius * 2; // Not really a thing for spheres, but whatever

				capsuleStart = center; // Bottom Center (not used in a sphere, but whatever)
				capsuleEnd = center; // Top Center (not used in a sphere, but whatever)
			}
		} else if (thisCollider.GetType () == typeof(BoxCollider)) {
			//Debug.Log (this.gameObject.name + "Box!");

			BoxCollider boxCol = GetComponent<BoxCollider> ();
			if (boxCol != null) {

				center = new Vector3 (boxCol.center.x * transform.localScale.x, boxCol.center.y * transform.localScale.y, boxCol.center.z * transform.localScale.z);
				dimensions = new Vector3 (boxCol.size.x * transform.localScale.x, boxCol.size.y * transform.localScale.y, boxCol.size.z * transform.localScale.z);

				radius = (boxCol.size.y / 2) * transform.localScale.y; // Not really a thing for boxes, but whatever
				height = boxCol.size.y; // Not really a thing for boxes, but whatever
				capsuleStart = center; // Bottom Center (not used in a box, but whatever)
				capsuleEnd = center; // Top Center (not used in a box, but whatever)
			}
		}
	}
	
	// Notice that it's a FixedUpdate() block. That's important.
	// Any corresponding interaction with this script will also have to be in a FixedUpdate() block
	// or stuff will probably get weird and annoying. You'd probably miss the enter/exit collisions.
	void FixedUpdate () {
		//Debug.DrawLine ((transform.position + center) - (Vector3.up/2), (transform.position + center) + (Vector3.up/2), Color.red, 3f);
		//Debug.DrawLine ((transform.position + center) - (Vector3.forward/2), (transform.position + center) + (Vector3.forward/2), Color.red, 3f);
		//Debug.DrawLine ((transform.position + center) - (Vector3.right/2), (transform.position + center) + (Vector3.right/2), Color.red, 3f);

		exitCols.Clear ();

		if (currentCols.Count > 0) {
			for (int i = currentCols.Count - 1; i >= 0; i--) {
				if (currentCols [i] == null)
					currentCols.Remove (currentCols [i]); // We don't want null objects hanging around
				else {
					//if (!exitCols.Contains(currentCols [i]))
						exitCols.Add (currentCols [i]);
					currentCols.Remove (currentCols [i]);
				}
			}
		}

		if (enterCols.Count > 0) {
			for (int i = enterCols.Count - 1; i >= 0; i--) {
				if (enterCols [i] == null)
					enterCols.Remove (enterCols [i]); // We don't want null objects hanging around
				else {
					//if (!currentCols.Contains(enterCols[i]))
						currentCols.Add (enterCols [i]);
					enterCols.Remove (enterCols [i]);
				}
			}
		}

		if (thisCollider.GetType () == typeof(CapsuleCollider)) { // If this is a Capsule Collider
			foreach (Collider col in Physics.OverlapCapsule(transform.position + capsuleStart, transform.position + capsuleEnd, radius + 0.1f, LMask)) {
				if (col != thisCollider && col != null) {
					addToQueue (col);
				}
			}
		} else if (thisCollider.GetType () == typeof(SphereCollider)) { // If this is a Sphere Collider
			foreach (Collider col in Physics.OverlapSphere(transform.position + center, radius + 0.1f, LMask)) {
				if (col != thisCollider && col != null) {
					addToQueue (col);
				}
			}
		} else if (thisCollider.GetType () == typeof(BoxCollider)) { // If this is a Box Collider
			foreach (Collider col in Physics.OverlapBox(transform.position + center, dimensions/2, transform.rotation, LMask)) {
				if (col != thisCollider && col != null) {
					addToQueue (col);
				}
			}
		}
			
		for (int i = colQueue.Count - 1; i >= 0; i--) {
			if (colQueue [i] == null) {
				//colQueue.Remove (colQueue [i]); // We don't want null objects hanging around
			} else {
				Collider col = colQueue [i];
				if (exitCols.Contains (col)) {
					if (!currentCols.Contains (col))
						currentCols.Add (col);
					exitCols.Remove (col);
				} else {
					if (!enterCols.Contains (col))
						enterCols.Add (col);
				}

				// If the other object has this script too, make sure it also gets the collision.
				if (col.gameObject.GetComponent<CustomCollisionHandler> () != null) {
					col.gameObject.GetComponent<CustomCollisionHandler> ().addToQueue (gameObject.GetComponent<Collider> ());
				}
			}
		}
		
		/*
		foreach (Collider col in colQueue) {
			//currentCols.Add (col);

			if (exitCols.Contains (col)) {
				if (!currentCols.Contains(col))
					currentCols.Add (col);
				exitCols.Remove (col);
			} else {
				if (!enterCols.Contains(col))
					enterCols.Add (col);
			}

			// If the other object has this script too, make sure it also gets the collision.
			if (col.gameObject.GetComponent<CustomCollisionHandler> () != null) {
				col.gameObject.GetComponent<CustomCollisionHandler> ().addToQueue (gameObject.GetComponent<Collider>());
			}
		}
		*/
		colQueue.Clear ();

	}

	void addToQueue(Collider col) {
		if (!colQueue.Contains (col) && !enterCols.Contains (col) && !currentCols.Contains (col)) {
			colQueue.Add (col);
		}
	}

	void SetMask(string[] layerNames) {
		LMask = LayerMask.GetMask (layerNames);
	}

	/*
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
	*/

	void OnCollisionEnter(Collision col) {
		addToQueue (col.collider);
	}
	void OnCollisionStay(Collision col) {
		addToQueue (col.collider);
	}
}
