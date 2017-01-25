using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Aylin Delacruz
// Collision detection for tiles
// putting in this script for now...
public class TileCollisionDet : MonoBehaviour {
	// when tile is not empty
	void OnCollisionEnter2D(Collision2D coll) {
		// for testing..
		if (coll.gameObject.tag == "character") {
			coll.gameObject.SendMessage ("An actor is on the tile");
		}
	
	// Use this for initialization
		/*void Start () {}
			
	// Update is called once per frame
	void Update () {
		
	}*/
	}
}
