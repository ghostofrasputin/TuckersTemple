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

public class MainMenuManager : MonoBehaviour {

	//publics:
	public GameObject ani;
	public GameObject panel;

	// audio
	public AudioClip MenuSlide;
	public AudioClip PlayStart;

	//private:
	private Animator anim;
	private RectTransform pan;
	private int counter = 0;

	// Use this for initialization
	void Start () {
		anim = ani.GetComponent<Animator>();
		pan = panel.GetComponent<RectTransform> ();
		anim.enabled = false;
	}

	// We might have something
	// animate on our main menu?
	void Update () {
		
	}

	// load scene by name
	public void loadScene(string scene)
	{
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
				pan.offsetMin = new Vector2 (-57,0);
				pan.offsetMax = new Vector2 (-305,0);
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

}

