using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum gameStates {
	playing,
	cutscene
}

public enum elements {
	none,
	fire,
	water,
	earth,
	air,
	ice,
	electricity,
	shadow,
	light
}

public class GameManager : MonoBehaviour {

	public static GameManager gm;

	public gameStates gameState;

	public GameObject player;
	public GameObject cameraTarget;
	public GameObject cursorTarget;
	public GameObject mainCanvas;

	[HideInInspector]
	public GameObject cursor;
	[HideInInspector]
	public UIDisplay HUDScript;

	[Header("Some common prefabs")]
	public GameObject coin;
	public GameObject healthPickup;
	public GameObject damageDigits;

	[Header("The universal sound instance")]
	public GameObject soundInstance;

	void Awake() {
		gm = this;

		gameState = gameStates.playing;

		if (player == null)
			player = GameObject.Find ("Player");
		if (cameraTarget == null)
			cameraTarget = GameObject.Find ("CameraTarget");
		if (cursorTarget == null)
			cursorTarget = GameObject.Find ("CursorTarget");
		if (mainCanvas == null)
			mainCanvas = GameObject.Find ("MainCanvas");

		if (mainCanvas != null)
			cursor = mainCanvas.transform.Find ("Cursor").gameObject;
		if (mainCanvas != null)
			HUDScript = mainCanvas.GetComponent<UIDisplay>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void createSound(AudioClip clip, Vector3 position) {
		if (clip != null) {
			GameObject obj = Instantiate (soundInstance, position, Quaternion.identity);
			obj.GetComponent<SoundInstance> ().sound = clip;
		}
	}

	public void createHealthPickup(Vector3 position) {
		if (healthPickup != null) {
			GameObject obj = Instantiate (healthPickup, position, Quaternion.identity);
		}
	}
	public void createCoin(Vector3 position) {
		if (coin != null) {
			GameObject obj = Instantiate (coin, position, Quaternion.identity);
		}
	}

	public void createDamageDigits(Vector3 position, float dmg) {
		if (damageDigits != null) {
			GameObject obj = Instantiate (damageDigits, position, Quaternion.identity);
			obj.transform.Find("NumberText").GetComponent<TextMesh>().text = dmg.ToString();
			obj.transform.Find("NumberText/Shadow").GetComponent<TextMesh>().text = dmg.ToString();

			if (dmg <= 0)
				obj.transform.Find ("NumberText").GetComponent<TextMesh> ().color = Color.white;
		}
	}

	// ---

	public void setGameState(gameStates s) {
		if (gameState != s) {
			gameState = s;
		}
	}

}
