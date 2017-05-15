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
    private int numOfStars;
	private string target;

	// Use this for initialization
	void Start () {
        return;
		levelNum = System.Convert.ToInt32(gameObject.name);
		try {
			isLocked = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getLockedLevelBool(levelNum-1);
			//Debug.Log("level: "+levelNum+" locked: "+isLocked);
			// use locked sprite and turn off button:
			if(isLocked == true) {
                this.gameObject.GetComponent<Button>().interactable = false;
				Sprite lockedSprite = Resources.Load("UI/Lock",typeof(Sprite)) as Sprite;
                this.gameObject.GetComponent<Image>().sprite = lockedSprite;
                Text buttonText = this.gameObject.GetComponent<Button>().GetComponentsInChildren<Text>()[0];
                buttonText.text = "";
                //this.gameObject.transform.Find("stars").GetComponent<Image>().enabled = false;
            } 
			// use unlocked sprite:
			else {
				this.gameObject.GetComponent<Button>().interactable = true;
				//Sprite lockedSprite = Resources.Load("UISprite",typeof(Sprite)) as Sprite;
				//this.gameObject.GetComponent<Image>().sprite = lockedSprite;
				Text buttonText = this.gameObject.GetComponent<Button>().GetComponentsInChildren<Text>()[0];
				buttonText.text = ""+levelNum+"";
				/*List<bool> stars = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getStars(levelNum-1);
				numOfStars = 0;
				for(int i=0; i<stars.Count;i++){
					if(stars[i] == false){
						numOfStars++;
					}
				}
				string target = "UI/" +numOfStars+ " star";*/
                Sprite threeStars = Resources.Load(target, typeof(Sprite)) as Sprite;
                //this.gameObject.transform.Find("stars").GetComponent<Image>().sprite = threeStars;
			}
		} catch(System.Exception error){
			Debug.Log ("Levellock error: "+error);
		}
	}
}
