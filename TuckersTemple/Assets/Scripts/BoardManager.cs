using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class BoardManager : MonoBehaviour {
	public GameObject exit;

	public void SetupScene (int level){
		// choose place to set exit tile on game board
		// sample code from Rogue-like :

		//Instantiate the exit tile in the upper right hand corner of our game board
		//Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
	}


}
