using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour {
	
	public float flashTime = 2f;
	public float elapsedTime;
	int bin = 1;
	GameObject body;

	public Color flashColor = new Color (1,0,0,1);
	Color normalColor;
	Vector3 normalScale;

	// Use this for initialization
	void Start () {
		body = gameObject.transform.Find("ChaserBody").gameObject;
		elapsedTime = flashTime;

		normalScale = body.transform.localScale;
		//normalColor = ;
		InvokeRepeating("Flash", 0f, 0.1f);
	}
	
	// Update is called once per frame
	void Update () {
		if (elapsedTime <= 0) {
			elapsedTime = flashTime;
			this.enabled = false;
		}

		elapsedTime -= Time.deltaTime;
	}

	void Flash() {
		if (bin == 1) {
			body.transform.localScale = new Vector3 (normalScale.x, normalScale.y * 0.8f, normalScale.z);
			bin = 0;
		}
		else {
			body.transform.localScale = new Vector3(normalScale.x, normalScale.y, normalScale.z);
			bin = 1;
		}
	}
}
