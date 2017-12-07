using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class elementSlot {
	public elements element;
	public string elementName;
	public Color mainColor;
	public Color secondaryColor;
	public Sprite cursor;
	public float maxEnergy = 100;
	float _energy;

	public elementSlot() {
		energy = maxEnergy;
	}

	public bool active = false;

	public float energy {
		get {
			return _energy;
		}
		set {
			if (value > maxEnergy)
				_energy = maxEnergy;
			else if (value < 0)
				_energy = 0;
			else
				_energy = value;
		}
	}
}

public class PlayerStats : MonoBehaviour {

	public int maxHP = 20;
	int _currentHP = 20;

	public int coins = 0;

	public elementSlot[] elementalPowers;
	int _currentElement; // The index of the current element in the above array;

	public Vector3 spawnPoint;

	bool hurtVincible;
	float hurtVinciTime;
	float MaxInvinciTime = 1;

	CustomCharacterController controller;
	CustomCollisionHandler colHandler;

	public GameObject DeathAnim;
	public AudioClip DeathSound;
	public AudioClip GetHitSound;
	AudioSource audioSource;
	Renderer rend;
	GameObject body;

	Color defaultColor;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CustomCharacterController>();
		colHandler = GetComponent<CustomCollisionHandler>();
		body = gameObject.transform.Find ("PlayerBody").gameObject;
		rend = body.GetComponent<Renderer> ();
		audioSource = gameObject.GetComponent<AudioSource>();

		Color DefaultColor = rend.material.color;
		hurtVincible = false;
		hurtVinciTime = MaxInvinciTime;

		// ---

		//elementalPowers [0] = new elementSlot();
		//elementalPowers [0].element = elements.fire;
		//elementalPowers [0].elementName = "Fire";
		//elementalPowers [0].active = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (colHandler != null) {
			foreach (Collider col in colHandler.enterCols) {
				if (col != null) {
					//Debug.Log ("Started touching " + col.gameObject.name);
					if (col.gameObject.tag == "Enemy" || col.gameObject.tag == "EnemyAttack") {
						checkDmg (col.gameObject);
						controller.impulse = (transform.position - SuperCollider.ClosestPointOnSurface (col, transform.position + controller.center, controller.radius)).normalized / 4;
					}

					if (col.gameObject.tag == "Pickup") {
						Pickup (col.gameObject);
					}

				}
			}

			foreach (Collider col in colHandler.exitCols) {
				if (col != null) {
					//Debug.Log ("Stopped touching " + col.gameObject.name);
				}
			}
		}

		///

		if (currentHP > maxHP) // Make Sure HP doesn't exceed maximum.
			currentHP = maxHP;

		if (currentHP <= 0)
			Respawn ();
		if (transform.position.y < -100) { // Just in case you fall out of the world or something...
			Vector3 spawnCache = spawnPoint;
			spawnPoint = controller.lastValidGroundPos;
			Respawn ();
			spawnPoint = spawnCache;
		}
	}

	// - - - //

	void Respawn() {
		if (DeathSound != null) {
			audioSource.clip = DeathSound;
			audioSource.Play();
		}

		Instantiate (DeathAnim, gameObject.transform.position, gameObject.transform.rotation);

		transform.position = spawnPoint;
		currentHP = maxHP;
		StartCoroutine ("invinciBlink");
	}


	void checkDmg (GameObject obj) {
		if (hurtVincible == false) {
			if (obj.tag == "Enemy" || obj.tag == "EnemyAttack") {

				int damageGet = 0;
				if (obj.GetComponent<EnemyStats> () != null)
					damageGet = obj.GetComponent<EnemyStats> ().contactDamage;
				else if (obj.GetComponent<Projectile> () != null)
					damageGet = obj.GetComponent<Projectile> ().damage;
				currentHP = currentHP - damageGet;

				GameManager.gm.HUDScript.RefreshHUD ();

				StartCoroutine ("invinciBlink");

				if (GetHitSound != null) {
					audioSource.clip = GetHitSound;
					audioSource.Play();
				}

				// This bit was supposed to make the enemy attacks jolt the player away.
				//Vector3 diff = (transform.position - col.transform.position);
				//Vector3 jolt = diff / diff.magnitude;
				//rb.AddForce (jolt * 5, ForceMode.Impulse);
			}
		}
	}

	void Pickup(GameObject obj){
		if (obj.tag == "Pickup" && obj.GetComponent<Pickup>() != null) {
			if (obj.GetComponent<Pickup> ().pickupType == pickupType.health)
				currentHP += obj.GetComponent<Pickup> ().amount;
			else if (obj.GetComponent<Pickup> ().pickupType == pickupType.coin)
				coins++;

			GameManager.gm.HUDScript.RefreshHUD ();
		}
	}

	public void setNextElement() {
		for (int i = currentElement+1; i < elementalPowers.Length; i++) {
			if (elementalPowers [i].active) {
				currentElement = i;
				break;
			}
		}
		GameManager.gm.HUDScript.RefreshHUD ();
	}
	public void setLastElement() {
		for (int i = currentElement-1; i >= 0; i--) {
			if (elementalPowers [i].active) {
				currentElement = i;
				break;
			}
		}
		GameManager.gm.HUDScript.RefreshHUD ();
	}

	// Fields

	public int currentHP {
		get {
			return (_currentHP);
		}
		set {
			if (value < 0)
				_currentHP = 0;
			else if (value > maxHP)
				_currentHP = maxHP;
			else
				_currentHP = value;
		}
	}

	public int currentElement {
		get {
			return (_currentElement);
		}
		set {
			if (value < 0)
				_currentElement = 0;
			else if (value >= elementalPowers.Length)
				_currentElement = elementalPowers.Length - 1;
			else
				_currentElement = value;
		}
	}

	// ---

	/*
	void OnCollisionEnter (Collision col) {
		checkDmg (col.gameObject);

		Pickup (col.gameObject);
	}

	void OnCollisionStay (Collision col) {
		checkDmg (col.gameObject);
	}
	*/

	IEnumerator invinciBlink() { // This here coroutine handles the invincible blinking you get after being hit.
		hurtVincible = true;
		Color blinkColor = new Color (1, 1, 1, 1);

		bool blinkOn = true;

		while (hurtVinciTime > 0) { // as long as the time isn't out...
			hurtVinciTime -= Time.deltaTime; // ... let the time count down

			/*
			if (blinkOn == true)
				//rend.material.color = blinkColor;
				rend.enabled = false;
			else //rend.material.color = defaultColor;
				rend.enabled = true;

			blinkOn = !blinkOn;
			*/

			yield return null;
		}

		//rend.material.SetColor("_Color", defaultColor);

		//rend.enabled = true;
		hurtVincible = false; // once done, we can stop being invincible
		hurtVinciTime = MaxInvinciTime; // reset the timer so it can start over if we get hit again.
	}
}
