using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour {

	Transform targ;
	Rigidbody rb;
	SphereCollider sph;

	public GameObject exitPrefab;

	bool held = true;
	Vector3 lastPos;

	[HideInInspector]
	public float powerLevel = 1;
	CustomCollisionHandler colHandler;
	PlayerStats stats;

	elements elementType;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		targ = GameManager.gm.cursorTarget.transform;
		sph = transform.Find("Ball").gameObject.GetComponent<SphereCollider> ();
		colHandler = sph.GetComponent<CustomCollisionHandler>();
		lastPos = transform.position;

		stats = GameManager.gm.player.GetComponent<PlayerStats> ();
		elementType = sph.GetComponent<EnemyDamager> ().elementType;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (colHandler != null) {
			foreach (Collider col in colHandler.enterCols) {
				if (col != null) {
					if (col.gameObject.tag == "PlayerAttack") {
						Debug.Log ("Fire Power UP! " + col.gameObject.name);
						powerLevel += 1;
					}
				}

			}
		}


		if (Input.GetAxisRaw ("Fire1") == 1 && held
			&& (Input.GetAxisRaw ("Fire1") == 1 && stats.elementalPowers[stats.currentElement].energy > 0f)
			&& elementType == stats.elementalPowers[stats.currentElement].element) {
			lastPos = transform.position;

			RaycastHit hinfo;

			if (/*Vector3.Dot(Vector3.up, targ.gameObject.GetComponent<PointerControls>().cursorNormal) <= 0.707f ||*/
				Physics.SphereCast (transform.position, sph.radius, (targ.position - transform.position).normalized, out hinfo,
					(targ.position - transform.position).magnitude, LayerMask.GetMask ("Terrain"), QueryTriggerInteraction.Ignore)
				&& Vector3.Dot (Vector3.up, hinfo.normal) <= 0.707f) { // ---

				//Vector3 newPos = new Vector3 (targ.position.x, transform.position.y, targ.position.z);
				//rb.MovePosition (Vector3.Lerp (transform.position, newPos, 0.25f)); // If the path between the fireball and the target is blocked.

				float h = transform.position.y - targ.position.y;
				float diff = h / (Mathf.Tan ((GameManager.gm.cameraTarget.transform.Find("CameraHeight").localEulerAngles.x) * Mathf.Deg2Rad));
				Debug.Log ("diff: " + diff);
				Vector3 newPos = targ.position + (-1 * (GameManager.gm.cameraTarget.transform.forward).normalized * diff);
				if (targ.position.y > transform.position.y)
					newPos = new Vector3 (newPos.x, transform.position.y, newPos.z);
				Debug.DrawLine (transform.position, newPos, Color.red, 10f);
				rb.MovePosition (Vector3.Lerp (transform.position, newPos, 0.25f)); // If the path between the fireball and the target is blocked.
			} else {
				Debug.DrawLine (transform.position, targ.position, Color.green, 10f);
				rb.MovePosition (Vector3.Lerp (transform.position, targ.position, 0.25f));
				rb.velocity = Vector3.zero;
			}
		} else if (held) {
			//gameObject.layer = LayerMask.NameToLayer("PlayerAttacks");
			//transform.Find("FireBody").gameObject.layer = LayerMask.NameToLayer("PlayerAttacks");

			held = false;
			rb.velocity = Vector3.ClampMagnitude((transform.position - lastPos) / Time.fixedDeltaTime, 32f);
			Invoke ("destroyFireball", 1f);
			//destroyFireball ();

			Debug.Log ("DROP");

			if (stats.elementalPowers [stats.currentElement].energy <= 0f)
				GameManager.gm.cursor.GetComponent<Animator> ().SetTrigger("Empty");
		}

		transform.Find ("Ball").gameObject.transform.localScale = Vector3.one * Mathf.Sqrt(powerLevel);
	}

	void destroyFireball() {
		if (gameObject.activeInHierarchy) {
			gameObject.SetActive (false);
			if (exitPrefab != null)
				Instantiate (exitPrefab, transform.position, Quaternion.identity);
			Invoke ("DIE", 1f);
		}
	}

	void DIE() {
		Destroy (gameObject);
	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
			Debug.Log ("Hit " + col.gameObject.name);
			if (Vector3.Dot (Vector3.up, col.contacts [0].normal) <= 0.707f && col.contacts [0].point.y >= (transform.position.y - 0.1f))
				destroyFireball();
		}

		if (col.gameObject.layer == LayerMask.NameToLayer("PlayerAttacks") && col.gameObject.GetComponent<EnemyDamager>() != null) {
			Debug.Log ("Fire Power UP! " + col.gameObject.name);
			powerLevel += 1;
		}
	}

}
