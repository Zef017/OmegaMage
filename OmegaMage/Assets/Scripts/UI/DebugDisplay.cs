using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDisplay : MonoBehaviour {

	Text hud;
	GameObject player;
	CustomCharacterController controller;

	GameObject cursor;
	PointerControls cursorControl;

	float fps;

	// Use this for initialization
	void Start ()
	{
		hud = gameObject.GetComponent<Text>();
		player = GameObject.Find ("Player");
		controller = player.GetComponent<CustomCharacterController>();

		cursor = GameObject.Find ("CursorTarget");
		cursorControl = cursor.GetComponent<PointerControls>();
	}

	// Update is called once per frame
	void Update ()
	{
		fps = 1.0f / Time.deltaTime;

		hud.text = "Y Speed: " + controller.ySpeed
		+ "\n\nX Position: " + player.transform.position.x
		+ "\nY Position: " + player.transform.position.y
		+ "\nZ Position: " + player.transform.position.z
			//+ "\n\nController Grounded: " + controller.isGrounded
		+ "\nScript Grounded: " + controller.isGrounded

		+ "\n\nCursor X: " + cursor.transform.position.x
		+ "\nCursor Y: " + cursor.transform.position.y
		+ "\nCursor Z: " + cursor.transform.position.z

		+ "\n\nPlayerHalfHeight (cursor) : " + cursorControl.playerHalfHeight
		+ "\nPlayerCenter (cursor) : " + cursorControl.playerCenter
		+ "\nFloorTouch : " + controller.floortouch
		+ "\nLastGroundPos : " + controller.lastGroundPos
		+ "\nLastValidGroundPos : " + controller.lastValidGroundPos
		+ "\nlastFloorNormal : " + controller.lastFloorNormal
			
			+"\n\nFPS : " + fps
			+"\n\nFPS : " + Mathf.Round(fps);
	}
}
