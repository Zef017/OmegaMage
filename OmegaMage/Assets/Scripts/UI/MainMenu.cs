using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public GameObject transition;

	// Use this for initialization
	void Start () {
		Cursor.visible = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void loadLevel(string sceneName) {
		SceneManager.LoadScene (sceneName);
	}

	public void quitGame () {
		Application.Quit ();
	}
}
