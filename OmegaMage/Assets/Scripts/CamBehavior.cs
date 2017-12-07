using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBehavior : MonoBehaviour
{
    public float smoothFactor;
	public Transform player;
	public Transform cursorTarget;
	public int camMode;

	Vector3 targetPosition;

    protected void Start()
    {

    }

    void LateUpdate()
    {
		//followPlayer ();


		if (camMode == 1)
			betweenPlayerAndCursor ();
		else
			followPlayer ();
    }

	void followPlayer()
	{
		// Follow the player
		targetPosition = player.position + (Vector3.up);
		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothFactor);
		//
	}

	void betweenPlayerAndCursor()
	{
		// Go between the player and the cursor
		targetPosition = Vector3.Lerp (player.position, cursorTarget.position, 0.2f);
				// The last variable in the lerp controls how far between points A and B the resulting point is.

		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothFactor);
		//
	}
}