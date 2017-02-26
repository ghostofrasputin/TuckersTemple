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

public class Menu : MonoBehaviour {

	//publics:
	public GameObject pauseAnimator;

	//private:
	private Animator anim;
	private int leftCounter = 0;
	private int rightCounter = 0;
	private bool isPaused = false; 

	// Use this for initialization
	void Start () {
		anim = pauseAnimator.GetComponent<Animator>();
		anim.enabled = false;
	}
	
	// We might have something
	// animate on our main menu?
	void Update () {
		
	}

	// load scene by name
	public void loadScene(string scene)
	{
		SceneManager.LoadScene(scene);
	}

	// controls main menu shift controls:
	public void shift(string flag){
		anim.enabled = true;
		if (flag == "right") {
			rightCounter++;
			if (rightCounter == 1) {
				anim.Play ("menuSlide1");
			}
			if (rightCounter == 2) {
				anim.Play ("menuSlide2");
			}
			if (rightCounter == 3) {
				anim.Play ("menuSlide3");
				rightCounter = 0;
			}
		}
		if (flag == "left") {
			leftCounter++;
			if (leftCounter == 1) {
				anim.Play ("menuSlide4");
			}
			if (leftCounter == 2) {
				anim.Play ("menuSlide5");
			}
			if (leftCounter == 3) {
				anim.Play ("menuSlide6");
				leftCounter = 0;
			}
		}
	}

	//function to pause the game
	public void playAnim(string anima){
		anim.enabled = true;
		anim.Play (anima);
	}
}

