/* LevelLock.cs
 * 
 * Used to check which sprite should
 * be displayed: locked level or unlocked level
 **/ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLock : MonoBehaviour {

	// public:

	// private:
	private int levelNum;
	private bool isLocked;

	// Use this for initialization
	void Start () {
		levelNum = System.Convert.ToInt32(gameObject.name);
		try {
			isLocked = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getLockedLevelBool(levelNum-1);
			// use locked sprite and turn off button:
			if(isLocked == true) {
				this.gameObject.GetComponent<Button>().interactable = false;
				Sprite lockedSprite = Resources.Load("UI/Lock",typeof(Sprite)) as Sprite;
				this.gameObject.GetComponent<Image>().sprite = lockedSprite; 
				Text buttonText = this.gameObject.GetComponent<Button>().GetComponentsInChildren<Text>()[0];
				buttonText.text = "";
			} 
			// use unlocked sprite:
			else {
				this.gameObject.GetComponent<Button>().interactable = true;
				//Sprite lockedSprite = Resources.Load("UISprite",typeof(Sprite)) as Sprite;
				//this.gameObject.GetComponent<Image>().sprite = lockedSprite;
				Text buttonText = this.gameObject.GetComponent<Button>().GetComponentsInChildren<Text>()[0];
				buttonText.text = ""+levelNum+"";
			}
		} catch(System.Exception){}
	}
}
