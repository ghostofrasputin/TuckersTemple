using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Aylin Delacruz 
// If player is on this tile, they beat the level! yay!
// declare win state, print "win!" 
public class WinState : MonoBehaviour {

	public GUIText levelWinText;

	private bool levelWin;
	// Use this for initialization
	void Start () {
		levelWin = false;
		levelWinText.text = "";
		// for future use?
		//restart = false;
	}

	void OnCollisionEnter2D(Collision2D coll) {
		// for testing..
		if (coll.gameObject.tag == "character") {
			coll.gameObject.SendMessage ("An actor is on the tile");
			//?
			//Destory(character);
			//Update()
		}
	}

	// Update is called once per frame
	void Update () {
		if (levelWin){
			levelWinText.text = "You win!";

			// again for possible future task
			// restart = true;
			break;
		}
			
	}
}
