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
	public GameObject menuSlide1;
	private Animator anim;
	private int counter = 0;
	// Use this for initialization
	void Start () {
		anim = menuSlide1.GetComponent<Animator>();
		Time.timeScale = 1;
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
			counter++;
			if (counter == 1) {
				anim.Play ("menuSlide1");
			}
			if (counter == 2) {
				anim.Play ("menuSlide2");
			}
			if (counter == 3) {
				anim.Play ("menuSlide3");
			}
			if (counter == -1) {
				anim.Play ("menuSlide4");
			}
		}
		if (flag == "left") {
			counter--;
			if (counter == 1) {
				anim.Play ("menuSlide5");
			}
			if (counter == 2) {
				anim.Play ("menuSlide6");
			}
			if (counter == 3) {
				anim.Play ("menuSlide7");
			}
			if (counter == -1) {
				anim.Play ("menuSlide8");
			}
		}
	}
}
