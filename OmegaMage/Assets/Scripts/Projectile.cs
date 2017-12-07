using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public int damage;
	public float timespan;
	public float speed;

	public GameObject hitAnim;
	public AudioClip fireSound;
	public AudioClip hitSound;

	public GameObject audioInstance;

	AudioSource audioSource;

	float timePassed;
	Vector3 movement;



	// Use this for initialization
	void Start () {
		timePassed = 0f;

		audioSource = gameObject.GetComponent<AudioSource>();

		if (fireSound != null) {
			audioSource.clip = fireSound;
			audioSource.Play();
		}
	}
	

	void FixedUpdate () {

		if (timePassed > timespan)
			destroyProjectile();

		moveProjectile ();

		timePassed += Time.deltaTime * 1f;
	}

	void moveProjectile()
	{
		movement.Set(0f, 0f, speed);
		movement = movement * Time.fixedDeltaTime;
		movement = transform.TransformDirection(movement); // Puttin' it back to local
		GetComponent<Rigidbody>().MovePosition (transform.position + movement);
	}

	void destroyProjectile()
	{
		GameManager.gm.createSound (hitSound, transform.position);

		Instantiate (hitAnim, transform.position, transform.rotation);
		gameObject.SetActive (false);
		Invoke ("DIE", 1f);
	}

	void DIE() {
		Destroy (gameObject);
	}

	void OnCollisionEnter (Collision col)
	{
		destroyProjectile ();
	}
}
