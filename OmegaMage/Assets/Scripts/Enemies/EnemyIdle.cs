using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : MonoBehaviour {

	public float maxFatigue;
	public float fatigue;

	public float attentionSpan = 15f;

	GameObject player;
	CustomCharacterController controller;
	float distance; // Distance to player

	GameObject alert;

	// Use this for initialization
	void Start () {
		fatigue = 0f;
		player = GameObject.Find("Player");
		controller = GetComponent<CustomCharacterController> ();

		alert = gameObject.transform.Find("AlertMark").gameObject; // Get the Exclamation Point Box!
		alert.SetActive (false);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		distance = (transform.position - player.transform.position).magnitude;

		fatigue -= Time.fixedDeltaTime; // Fatigue drops while idling;

		if (distance <= attentionSpan && fatigue <= 0f)
			goChase ();
	}

	public void goChase()
	{
		if (alert.activeInHierarchy == false)
			gameObject.GetComponent<AudioSource> ().Play ();

		alert.SetActive (true);

		var chaseScript = GetComponent<ChasePlayer> ();
		chaseScript.enabled = true;

		fatigue = maxFatigue;
		this.enabled = false;
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.tag == "PlayerAttack") // Even if still fatigued, it still chases you if shot.
			goChase (); // If the enemy got hit while idle, it's gonna be mad.
	}
}
