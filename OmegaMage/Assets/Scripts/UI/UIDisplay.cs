using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplay : MonoBehaviour {

	GameObject player;
	PlayerStats stats;

	GameObject HPCounter;
	Text HPText;
	RectTransform HPBar;

	GameObject CoinCounter;
	Text CoinText;

	Image ElementLabel;
	Text ElementText;

	Animator animator;

	int lastHPCount;
	int lastCoinCount;

	// Use this for initialization
	void Start ()
    {
		HPCounter = gameObject.transform.Find ("HUD/HPCounterBG/HPCounter").gameObject;
		HPText = HPCounter.GetComponent<Text>();
		HPBar = gameObject.transform.Find ("HUD/HPBar").gameObject.GetComponent<RectTransform>();

		CoinCounter = gameObject.transform.Find ("HUD/CoinCounterBG/CoinCounter").gameObject;
		CoinText = CoinCounter.GetComponent<Text>();

		ElementLabel = gameObject.transform.Find ("HUD/ElementNameBG").gameObject.GetComponent<Image> ();
		ElementText = gameObject.transform.Find ("HUD/ElementNameBG/ElementName").gameObject.GetComponent<Text>();

		player = GameManager.gm.player;
		stats = player.GetComponent<PlayerStats>();

		animator = GetComponent<Animator>();

		int lastHPCount = stats.currentHP;
		int lastCoinCount = stats.coins;
		RefreshHUD ();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (GameManager.gm.gameState == gameStates.playing && animator.GetBool ("Playing") == false) {
			animator.SetBool ("Playing", true);
		} else if (GameManager.gm.gameState == gameStates.cutscene && animator.GetBool ("Playing") == true) {
			animator.SetBool ("Playing", false);
		}

		//HPText.text = stats.currentHP + "/" + stats.maxHP;
		//CoinText.text = stats.coins.ToString ();

		//ElementText.text = stats.elementalPowers[stats.currentElement].elementName;

		// Gradual SMOOTH change in bar
		if (HPBar.sizeDelta.x != (((float)stats.currentHP / (float)stats.maxHP) * 100) )
			HPBar.sizeDelta = new Vector2(Mathf.Lerp(HPBar.sizeDelta.x, (((float)stats.currentHP / (float)stats.maxHP) * 100), 0.5f), HPBar.sizeDelta.y);
		
    }

	public void RefreshHUD() {
		if (Input.GetButtonDown ("ChangeElement")) {
			animator.SetTrigger ("ElementSwap");
			gameObject.transform.Find ("Cursor").gameObject.GetComponent<Image>().sprite = stats.elementalPowers[stats.currentElement].cursor;
			gameObject.transform.Find ("Cursor/BarBG").gameObject.GetComponent<Image>().color = stats.elementalPowers[stats.currentElement].mainColor;
			gameObject.transform.Find ("Cursor/ElementBar").gameObject.GetComponent<Image>().color = stats.elementalPowers[stats.currentElement].secondaryColor;
		} if (stats.currentHP != lastHPCount) {
			animator.SetTrigger ("HPGet");
			lastHPCount = stats.currentHP;
		} if (stats.coins != lastCoinCount) {
			animator.SetTrigger ("CoinGet");
			lastCoinCount = stats.coins;
		}


		HPText.text = stats.currentHP + "/" + stats.maxHP;
		CoinText.text = stats.coins.ToString ();
		ElementText.text = stats.elementalPowers[stats.currentElement].elementName;
		ElementLabel.color = stats.elementalPowers[stats.currentElement].mainColor;
	}
}
