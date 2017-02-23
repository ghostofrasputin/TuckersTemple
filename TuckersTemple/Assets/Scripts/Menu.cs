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

	// Use this for initialization
	void Start () {
		
	}
	
	// We might have something
	// animate on our main menu?
	void Update () {
		
	}

	// Start Game from Level One:
	public void play()
	{
		SceneManager.LoadScene("main");
	}
}
