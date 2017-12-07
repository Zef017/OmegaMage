using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {

    // Use this for initialization
	void Start ()
    {
		
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.position.y < -100)
        {
            transform.position = new Vector3(0,1,0);
        }
    }
}
