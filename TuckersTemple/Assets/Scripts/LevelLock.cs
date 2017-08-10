/* LevelLock.cs
 * 
 * Used to check which sprite should
 * be displayed: locked level or unlocked level
 **/ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLock : MonoBehaviour
{
    // private:
    private int levelNum;
    private bool isLocked;
    private int numOfStars;
    private string target;
    private Button lockButton;

    public void setLock(bool isLocked)
    {
        levelNum = System.Convert.ToInt32(gameObject.name);
        lockButton = GetComponentsInChildren<Button>()[1];
        if (isLocked == true)
        {
            this.gameObject.GetComponent<Button>().interactable = false;
            lockButton.gameObject.SetActive(true);
            Sprite lockedSprite = Resources.Load("mainMenu/TT-Logo-UNlocked", typeof(Sprite)) as Sprite;
            this.gameObject.GetComponent<Image>().sprite = lockedSprite;
            Text buttonText = this.gameObject.GetComponent<Button>().GetComponentsInChildren<Text>()[0];
            buttonText.text = "";
            this.gameObject.transform.Find("Stars").GetComponent<Image>().enabled = false;
        }
        else
        {
            this.gameObject.GetComponent<Button>().interactable = true;
            lockButton.gameObject.SetActive(false);
            Sprite lockedSprite = Resources.Load("mainMenu/TT-Logo-UNlocked", typeof(Sprite)) as Sprite;
            this.gameObject.GetComponent<Image>().sprite = lockedSprite;
            Text buttonText = this.gameObject.GetComponent<Button>().GetComponentsInChildren<Text>()[0];
            buttonText.text = "" + levelNum + "";

            // set star image:
            List<bool> stars = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getStars(levelNum - 1);
            numOfStars = 0;
            for (int i = 0; i < stars.Count; i++)
            {
                if (stars[i] == true)
                {
                    numOfStars++;
                }
                //Debug.Log(numOfStars);
                string target = "UI/LevelSelectStars" + numOfStars;
                Sprite threeStars = Resources.Load(target, typeof(Sprite)) as Sprite;
                this.gameObject.transform.Find("Stars").GetComponent<Image>().sprite = threeStars;
            }
        }
    }
}
