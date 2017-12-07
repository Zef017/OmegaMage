using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class damageFactor {
	public elements element;
	public float factor = 1.0f;
}

public class EnemyStats : MonoBehaviour {

	public int maxHP = 10;
	public int currentHP = 10;

	public float speed = 0.3f;
	public int contactDamage;

	[SerializeField]
	public damageFactor[] DamageFactors;
	Dictionary<elements, float> damageDict;

	float lastDamageTime;

	public AudioClip DamageSound;

	public GameObject DamageAnim;
	public GameObject DeathAnim;
	public AudioClip DeathSound;

	CustomCharacterController controller;
	CustomCollisionHandler colHandler;

	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		damageDict = new Dictionary<elements, float>();

		foreach(damageFactor d in DamageFactors) {
			damageDict.Add (d.element, d.factor);
		}

		controller = GetComponent<CustomCharacterController> ();
		colHandler = GetComponent<CustomCollisionHandler>();

		audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (colHandler != null) {
			foreach (Collider col in colHandler.enterCols) {
				if (col != null) {
					if (col.gameObject.tag == "PlayerAttack") {
						checkDmg (col);
						lastDamageTime = Time.time;

						if (DamageAnim != null) {
							GameObject Death = Instantiate (DamageAnim, gameObject.transform.position, gameObject.transform.rotation);
							Death.transform.localScale = transform.localScale;
						}
					}
				}
			}

			foreach (Collider col in colHandler.currentCols) {
				if (col != null) {
					if (col.gameObject.tag == "PlayerAttack" && Time.time - lastDamageTime > 0.5f) {
						checkDmg (col);
						lastDamageTime = Time.time;

						if (DamageAnim != null) {
							GameObject Death = Instantiate (DamageAnim, gameObject.transform.position, gameObject.transform.rotation);
							Death.transform.localScale = transform.localScale;
						}
					}
				}
			}

		}

		if (currentHP <= 0)
			destroyEnemy ();
		if (transform.position.y < -100) // Just in case the enemy falls out of the world or something...
			destroyEnemy ();
	}

	void checkDmg(Collider col)
	{
		EnemyDamager enDmgr = col.gameObject.GetComponent<EnemyDamager> ();
		if (enDmgr != null) {

			int damageRecieved;
			if (damageDict.ContainsKey(enDmgr.elementType))
				damageRecieved = (int)(col.gameObject.GetComponent<EnemyDamager> ().damage * damageDict[enDmgr.elementType]); // Calculate damage
			else
				damageRecieved = col.gameObject.GetComponent<EnemyDamager> ().damage; // Calculate damage
			currentHP = currentHP - damageRecieved; // Deal damage

			//if (col.gameObject.GetComponent<EnemyDamager> ().damage > 0)
			GameManager.gm.createDamageDigits (transform.position + Vector3.up, damageRecieved);


			if (controller != null && enDmgr.knockback != 0f) {
				if (enDmgr.attackType == attackTypes.melee) {
					if (Time.time - lastDamageTime > 0.25f)
						controller.impulse = ((transform.position - SuperCollider.ClosestPointOnSurface (GameManager.gm.player.GetComponent<Collider>(), transform.position + controller.center, controller.radius)).normalized / 4) * enDmgr.knockback;
				} else {
					controller.impulse = ((transform.position - SuperCollider.ClosestPointOnSurface (col, transform.position + controller.center, controller.radius)).normalized / 4) * enDmgr.knockback;
				}
					Debug.Log ("Whack!");
			}
			GameManager.gm.createSound (DamageSound, transform.position);
		} else {
			currentHP = currentHP - 2;
		}
	}

	void destroyEnemy()
	{
		GameManager.gm.createSound (DeathSound, transform.position);
		if (DeathAnim != null) {
			GameObject Death = Instantiate (DeathAnim, gameObject.transform.position, gameObject.transform.rotation);
			Death.transform.localScale = transform.localScale;
		}
		Destroy (gameObject);

		PlayerStats stats = GameManager.gm.player.GetComponent<PlayerStats> ();
		float r = Random.Range (0.0f, 1.0f);
		r -= (stats.currentHP / stats.maxHP) * 7;
		if (r > 0.5f)
			GameManager.gm.createHealthPickup (transform.position);
		else if (r < 0.75f)
			GameManager.gm.createCoin (transform.position);
	}

	/*
	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.tag == "PlayerAttack") {
			currentHP = currentHP - 2;
		}
	}
	*/
}
