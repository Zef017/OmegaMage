using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInstance : MonoBehaviour {

	public AudioClip sound;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();
		if (sound != null) {
			audioSource.clip = sound;
			audioSource.Play();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (audioSource.isPlaying == false)
			Destroy (gameObject);
	}
}
