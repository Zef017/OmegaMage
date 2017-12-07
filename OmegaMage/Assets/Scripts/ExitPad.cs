using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPad : MonoBehaviour {

	public bool turnedOn;

	public Material offMaterial;
	public Material onMaterial;
	public string sceneName;

	GameObject padCenter;
	GameObject ring;
	GameObject particles;

	// Use this for initialization
	void Start () {
		padCenter = gameObject.transform.Find ("PadCenter").gameObject;
		ring = gameObject.transform.Find ("PadRing").gameObject;
		particles = gameObject.transform.Find ("Particles").gameObject;

		particles.GetComponent<Renderer> ().material = onMaterial;
		padCenter.GetComponent<Renderer> ().material = onMaterial;

		if (turnedOn == true) {
			turnOn ();
		} else {
			turnOff ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void turnOn() {
		turnedOn = true;
		ring.GetComponent<Renderer> ().material = onMaterial;
		padCenter.SetActive (true);
		particles.SetActive (true);
	}

	public void turnOff() {
		turnedOn = false;
		ring.GetComponent<Renderer> ().material = offMaterial;
		padCenter.SetActive (false);
		particles.SetActive (false);
	}

	void OnTriggerStay (Collider col) {
		if (turnedOn == true && col.gameObject.tag == "Player" && Input.GetButtonDown ("Fire1"))
			SceneManager.LoadScene (sceneName);
	}
}
