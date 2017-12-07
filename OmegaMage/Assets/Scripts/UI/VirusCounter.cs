using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirusCounter : MonoBehaviour {

	int originalEnemyCount;
	GameObject[] enemies;

	Text hud;

	GameObject VCounter;
	RectTransform VBar;

	// Use this for initialization
	void Start () {
		VCounter = gameObject.transform.Find ("VCounter").gameObject;
		VBar = gameObject.transform.Find ("VBar").gameObject.GetComponent<RectTransform>();

		hud = VCounter.GetComponent<Text>();

		enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		originalEnemyCount = enemies.Length;
	}
	
	// Update is called once per frame
	void Update () {

		enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		if (enemies.Length > 0)
			hud.text = "Viruses: " + enemies.Length;
		else
			hud.text = "WELL DONE!";

		// Instant change in bar
		//VBar.sizeDelta = new Vector2(((float)enemies.Length / (float)originalEnemyCount) * 100, 100);

		// Gradual smooth change in bar
		if (VBar.sizeDelta.x != (((float)enemies.Length / (float)originalEnemyCount) * 100) )
			VBar.sizeDelta = new Vector2(Mathf.Lerp(VBar.sizeDelta.x, (((float)enemies.Length / (float)originalEnemyCount) * 100), 0.5f), VBar.sizeDelta.y);
	}
}
