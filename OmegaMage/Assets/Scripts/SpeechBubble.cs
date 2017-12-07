using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SpeechBubble : MonoBehaviour {

	[TextArea]
	public string Words = "<INSERT WORDS HERE>";

	//public Vector2 bubbleDimensions = new Vector2 (380, 180);
	public float bubbleWidth = 380;

	public Transform talkSpot;
	public float TalkRange;

	public float TalkHole = 0; // allows the talk range to function like a donut

	public Color bubbleColor = new Color(1,1,1,1);
	public Color textColor = new Color(0,0,0,1);

	Transform player;
	GameObject canvas;
	GameObject textBox;
	GameObject bubble;
	GameObject tail;
	GameObject bubbleShad;
	GameObject tailShad;
	bool boxOpen;
	Vector3 originalScale;

	Animator animator;
	int textIndex = 0;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player").transform;
		canvas = gameObject.transform.Find ("Canvas").gameObject;
		textBox = gameObject.transform.Find ("Canvas/Text").gameObject;
		bubble = gameObject.transform.Find ("Canvas/Bubble").gameObject;
		tail = gameObject.transform.Find ("Canvas/Bubble/Tail").gameObject;
		bubbleShad = gameObject.transform.Find ("Canvas/BubbleShadow").gameObject;
		tailShad = gameObject.transform.Find ("Canvas/BubbleShadow/TailShadow").gameObject;
		boxOpen = false;
		originalScale = canvas.transform.localScale;
		applyParameters ();

		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (talkSpot == null) {
			//StopAllCoroutines ();
			//StartCoroutine("CloseAndDestroy");
			animator.SetBool("Open", false);
			Invoke ("destroy", 1f);
		} else {

			var distance = (talkSpot.position - player.position).magnitude;
			if (distance <= TalkRange && distance > TalkHole && boxOpen == false) {
				//StartCoroutine ("Open");
				textIndex = 0;
				boxOpen = true;
				animator.SetBool ("Open", true);
				//GameManager.gm.setGameState (gameStates.cutscene);
			} else if ((distance > TalkRange || distance <= TalkHole) && boxOpen == true) {
				//StartCoroutine ("Close");
				boxOpen = false;
				animator.SetBool ("Open", false);
				//GameManager.gm.setGameState (gameStates.playing);
			}
		}

		if (boxOpen && textIndex <= Words.Length) {
			textBox.GetComponent<Text>().text = Words.Substring (0, textIndex) + "<color=#00000000>" + Words.Substring (textIndex, Words.Length-textIndex) + "</color>";
			textIndex += 1;
			if (textIndex < Words.Length) textIndex += 1;
		}
	}

	void fixEscapes() {
		Words = Words.Replace ("\\n", "\n");
	}

	void applyParameters() {
		fixEscapes ();
		textBox.GetComponent<UnityEngine.UI.Text>().text = Words; // Text String
		textBox.GetComponent<UnityEngine.UI.Text>().color = textColor; // Text Color

		/*
		bubble.GetComponent<RectTransform> ().sizeDelta = bubbleDimensions;
		bubbleShad.GetComponent<RectTransform> ().sizeDelta = bubbleDimensions;
		textBox.GetComponent<RectTransform> ().sizeDelta = bubbleDimensions - new Vector2(40,0);
		*/

		textBox.GetComponent<RectTransform> ().sizeDelta = new Vector2(bubbleWidth, 180);
		Vector2 bubdub = new Vector2 (bubbleWidth, textBox.GetComponent<UnityEngine.UI.Text>().preferredHeight + 64);
		bubble.GetComponent<RectTransform> ().sizeDelta = bubdub + new Vector2 (50, 0);
		bubbleShad.GetComponent<RectTransform> ().sizeDelta = bubdub + new Vector2 (50, 0);
		textBox.GetComponent<RectTransform> ().sizeDelta = bubdub;
		if (bubbleWidth > textBox.GetComponent<UnityEngine.UI.Text> ().preferredWidth) {
			bubdub = new Vector2 (textBox.GetComponent<UnityEngine.UI.Text> ().preferredWidth, bubdub.y);
			bubble.GetComponent<RectTransform> ().sizeDelta = bubdub + new Vector2 (50, 0);
			bubbleShad.GetComponent<RectTransform> ().sizeDelta = bubdub + new Vector2 (50, 0);
			textBox.GetComponent<RectTransform> ().sizeDelta = bubdub;
		}

		bubble.GetComponent<UnityEngine.UI.Image> ().color = bubbleColor;
		tail.GetComponent<UnityEngine.UI.Image> ().color = bubbleColor;
		bubbleShad.GetComponent<UnityEngine.UI.Image> ().color = Color.Lerp(bubbleColor, Color.black, 0.5f);
		tailShad.GetComponent<UnityEngine.UI.Image> ().color = Color.Lerp(bubbleColor, Color.black, 0.5f);
	}

	public void changeContents(string W, float Wid)
	{
		Words = W;
		bubbleWidth = Wid;
		applyParameters ();
	}

	public void destroy() {
		Destroy (gameObject);
	}
}
