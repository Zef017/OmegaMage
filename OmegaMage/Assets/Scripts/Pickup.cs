using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum pickupType {
	coin,
	health
}

public class Pickup : MonoBehaviour {

	public pickupType pickupType;
	public int amount;
	public GameObject pickupAnim;

	public AudioClip pickupSound;

	CustomCollisionHandler colHandler;

	// Use this for initialization
	void Start () {
		colHandler = GetComponent<CustomCollisionHandler>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate () {
		if (colHandler != null) {
			foreach (Collider col in colHandler.enterCols) {
				if (col.gameObject.tag == "Player") {
					Disappear ();
				}
			}
		}

	}

	void Disappear () {
		GameManager.gm.createSound (pickupSound, transform.position);

		Instantiate (pickupAnim, transform.position, transform.rotation);
		gameObject.SetActive (false);
		Invoke ("DestroyIt", 1f);
	}

	void DestroyIt () {
		Destroy (gameObject);
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.tag == "Player") {

			GameManager.gm.createSound (pickupSound, transform.position);

			Instantiate (pickupAnim, transform.position, transform.rotation);
			Destroy (gameObject);
		}
	}
}
