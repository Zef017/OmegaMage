using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedUVs : MonoBehaviour {

	public int frames;
	public float animTime;
	int currentFrame;
	float runtime;
	float offset;

	GameObject graphic;
	Renderer rend;

	// Use this for initialization
	void Start () {
		graphic = gameObject.transform.GetChild(0).gameObject; // Get the textured quad!

		rend = graphic.GetComponent<Renderer> ();
		rend.material.mainTextureScale = new Vector2((1f/frames), 1f);
		rend.material.mainTextureOffset = new Vector2(0f,0f);

		runtime = 0;
		currentFrame = 0;
		offset = 0;

		//renderer.material.TilingX = 
	}
	
	// Update is called once per frame
	void Update () {
		runtime += Time.deltaTime;

		if (runtime > ( (animTime / frames) * (currentFrame + 1))) {
			offset += (1f / frames);
			rend.material.mainTextureOffset = new Vector2(offset,0f);
			currentFrame += 1;
		}

		if (runtime >= animTime)
			Destroy (gameObject);
	}
}
