/* Menu.cs 
 * 
 * Contains functions applicable for the the main
 * menu, death screen, title screen, setting scene,
 * etc.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

	//publics:
	public GameObject ani;
	public GameObject panel;

	// load screen
	public GameObject loadingImage;

	// audio
	public AudioClip MenuSlide;
	public AudioClip PlayStart;

	//private:
	private Animator anim;
	private int counter = 0;

	// settings menu controls:
	private int settingsScroll;
	private RectTransform settingsPanel;
	private float speed = 7.0f;
	private float lowerLimit;
	private float upperLimit;

	// Use this for initialization
	void Start () {
		settingsPanel = GameObject.FindWithTag("settingsPan").GetComponent<RectTransform>();
		lowerLimit = settingsPanel.transform.position.y;
		upperLimit = lowerLimit-(settingsPanel.rect.height-30);
		settingsScroll = 0;
		anim = ani.GetComponent<Animator>();
		anim.enabled = false;

		Toggle music = GameObject.Find("MusicToggle").GetComponent<Toggle>();
		music.isOn = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getMusicToggle();
		Toggle sfx = GameObject.Find("SFXToggle").GetComponent<Toggle>();
		sfx.isOn = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getSFXToggle();

	}
		
	void Update () {
		// have to update settings panel manually because aniamtion won't
		// work depending where the main menu panel is located, basically
		// it just bugs out and looks bad so this fixes it

		float currentPos = settingsPanel.transform.position.y;
		Debug.Log (currentPos);
		Debug.Log ("upper"+upperLimit);
		// scroll down
		if (settingsScroll == 1) {
			currentPos -= speed;
			settingsPanel.transform.position = new Vector3 (settingsPanel.transform.position.x, currentPos, settingsPanel.transform.position.z);
			if (currentPos <= upperLimit) {
				settingsScroll = 0;
			}
		}

		// scroll up
		if (settingsScroll == 2) {
			currentPos += speed;
			settingsPanel.transform.position = new Vector3 (settingsPanel.transform.position.x, currentPos, settingsPanel.transform.position.z);
			if (currentPos >= lowerLimit) {
				settingsScroll = 0;
			}
		}
	}

	public void musicToggle(){
		GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().setMusicToggle();
	}

	public void sfxToggle(){
		GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().setSFXToggle();
	}

	// sets level through zombie passer
	// this works from scene to scene
	public void updateLevelNum(int newLevelNum){
		try {
			GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().setLevel(newLevelNum);
		} catch(System.Exception){}
	}

	// load scene by name
	public void loadScene(string scene)
	{
		loadingImage.SetActive (true);
		SoundController.instance.PlaySingle (PlayStart);
		SceneManager.LoadScene(scene);
	}

	// controls main menu shift controls:
	public void shift(string flag){
		anim.enabled = true;
		if (flag == "right") {
			if (counter == 0) {
				anim.Play ("slideRight1");
				SoundController.instance.PlaySingle (MenuSlide);
			}
			if (counter == 1) {
				anim.Play ("slideRight2");
				SoundController.instance.PlaySingle (MenuSlide);
			}
			if (counter == 2) {
				anim.Play ("slideRight3");
				SoundController.instance.PlaySingle (MenuSlide);
				counter = 0;
				//pan.offsetMin = new Vector2 (-57,0);
				//pan.offsetMax = new Vector2 (-305,0);
				return;
			}
			counter++;
		}
		if (flag == "left") {
			if (counter == 0) {
				anim.Play ("slideLeft1");
				SoundController.instance.PlaySingle (MenuSlide);
				counter = 2;
				return;
			}
			if (counter == 1) {
				anim.Play ("slideLeft2");
				SoundController.instance.PlaySingle (MenuSlide);
			}
			if (counter == 2) {
				anim.Play ("slideLeft3");
				SoundController.instance.PlaySingle (MenuSlide);
			}
			counter--;
		}
	}

	void startLevel(int levelNum){
		//gm.currentLevel = levelNum;
		loadScene ("main");
	}

	//function to pause the game
	public void playAnim(string anima){
		//SoundController.instance.PlaySingle (InGameMenuSound);
		anim.enabled = true;
		anim.Play (anima);
	}

	public void scrollDown(){
		settingsScroll = 1;
	}

	public void scrollUp(){
		settingsScroll = 2;
	}

}



