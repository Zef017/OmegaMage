using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorGraphic : MonoBehaviour {
	
	Animator animator;
	PlayerStats stats;
	RectTransform bar;

	// Use this for initialization
	void Start () {
		Cursor.visible = false;

		stats = GameManager.gm.player.GetComponent<PlayerStats>();
		animator = GetComponent<Animator> ();
		bar = gameObject.transform.Find ("ElementBar").gameObject.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Cursor.visible = false;
		transform.position = Input.mousePosition;

		if (Input.GetButtonDown ("Fire1")) {
			animator.SetTrigger ("Click");
		}

		// ---

		// Elemental Energy Bar
		if (bar.sizeDelta.x != (((float)stats.elementalPowers[stats.currentElement].energy / (float)stats.elementalPowers[stats.currentElement].maxEnergy) * 100) )
			bar.sizeDelta = new Vector2(Mathf.Lerp(bar.sizeDelta.x, (((float)stats.elementalPowers[stats.currentElement].energy / (float)stats.elementalPowers[stats.currentElement].maxEnergy) * 100), 0.5f), bar.sizeDelta.y);
	}
}
