using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayer : MonoBehaviour {

	Vector3 playerPos;

	Vector3 movement;
	public float speed = 3f;

	public float fatigueTime = 5f;
	public float chaseTime = 3f;

	float lastModeChangeTime; // Last time we went from chasing to waiting or vice versa

	bool chaseReady = true;	// Ready to chase
	bool chasing = false;	// Actually chasing
	CustomCharacterController controller;
	CustomCollisionHandler colHandler;
	Animator animator;

	//

	public float attentionSpan = 15f;

	float distance; // Distance to player

	public GameObject alertBox;
	public float alertHeight = 2f;



	// Use this for initialization
	void Start () {
		controller = GetComponent<CustomCharacterController> ();
		colHandler = GetComponent<CustomCollisionHandler> ();
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (chaseReady) {
			playerPos = GameManager.gm.player.transform.position;
			distance = (transform.position - playerPos).magnitude;

			if (chasing) {

				Vector3 relPos = playerPos - transform.position;
				Quaternion targRot = Quaternion.LookRotation(relPos);
				transform.rotation = Quaternion.Lerp (transform.rotation, targRot, 0.15f);
				//transform.LookAt (playerPos); // Look at the player!
				transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0); // Keep the thing from looking up and down or rolling side to side.

				if (Time.time - lastModeChangeTime > 0.5f) // Take half a second or so to start chasing
					moveEnemy ();

				if (Time.time - lastModeChangeTime > chaseTime) { // Stop chasing
					lastModeChangeTime = Time.time;
					chaseReady = false;
					chasing = false;
					if (animator != null)
						animator.SetBool ("Chasing", false);
				}
			} else if (distance <= attentionSpan) { // Start chasing!
				makeAlertBox ();
				lastModeChangeTime = Time.time;
				chasing = true;
				if (animator != null)
					animator.SetBool ("Chasing", true);
			} else {
				checkHit ();
			}
		} else {
			if (Time.time - lastModeChangeTime > fatigueTime) {
				lastModeChangeTime = Time.time;
				chaseReady = true;
			} else {
				checkHit ();
			}
		}
	}

	void moveEnemy()
	{
		//movement.Set(0f, 0f, speed);
		//movement = movement * Time.fixedDeltaTime;
		//movement = transform.TransformDirection(movement); // Puttin' it back to local
		//GetComponent<Rigidbody>().MovePosition (transform.position + movement);
		controller.xMove = 0f;
		controller.zMove = 1f;
	}

	void checkHit() {
		if (colHandler != null) {
			foreach (Collider col in colHandler.enterCols) {
				//Debug.Log ("Started touching " + col.gameObject.name);
				if (col.gameObject.tag == "Player" || col.gameObject.tag == "PlayerAttack") {
					makeAlertBox();
					lastModeChangeTime = Time.time;
					chasing = true;
					chaseReady = true;
					if (animator != null)
						animator.SetBool ("Chasing", true);
				}
			}
		}
	}

	void makeAlertBox() {
		if (alertBox != null) {
			GameObject box = Instantiate (alertBox, transform.position + (Vector3.up * 2f), Quaternion.identity);
			box.GetComponent<ParentConstraint> ().parent = gameObject;
			box.GetComponent<ParentConstraint> ().offset = (Vector3.up * alertHeight);
			gameObject.GetComponent<AudioSource> ().Play ();
		}
	}

}
