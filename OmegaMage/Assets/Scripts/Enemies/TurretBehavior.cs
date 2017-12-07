using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour {

	public Rigidbody Projectile;
	public float attentionSpan = 25f;
	public float minWait = 2f;
	public float maxWait = 6f;
	float distance;

	float countDown;
	EnemyStats stats;
	AimAtThing aimScript;

	// Use this for initialization
	void Start () {
		stats = GetComponent<EnemyStats>();
		aimScript = GetComponent<AimAtThing> ();
		if (aimScript.target == null) // If there's no target specified, default to the Player.
			aimScript.target = GameObject.Find ("Player").transform;

		countDown = Random.Range (minWait, maxWait);
	}
	
	// Update is called once per frame
	void Update () {
		if (aimScript.target == null) // Same as above, but just in case the target gets destroyed.
			aimScript.target = GameObject.Find ("Player").transform;

		distance = (transform.position - aimScript.target.transform.position).magnitude;
		float heightDiff = (Mathf.Abs(transform.position.y - aimScript.target.transform.position.y));
		if (distance <= attentionSpan && heightDiff <= 3f) {
			countDown -= Time.deltaTime;

			if (countDown <= 0) {
				countDown = Random.Range (minWait, maxWait);
				StartCoroutine ("fireThreeShots");
			}
		}
	}

	IEnumerator fireThreeShots() {
		for (int i = 0; i < 3; i++) {
			fireProjectile ();
			yield return new WaitForSeconds (0.2f);
		}
	}

	void fireProjectile()
	{
		var firePoint = transform.position + transform.forward;
		//Rigidbody projClone = (Rigidbody) Instantiate (Projectile, firePoint, body.transform.rotation);
		Instantiate (Projectile, firePoint, transform.rotation);
	}
}
