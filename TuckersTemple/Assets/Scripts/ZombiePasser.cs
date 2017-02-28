/*
 * ZombiePasser.cs
 * 
 * Stores level to be started in main
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePasser : MonoBehaviour {

	// public:

	// private:
	private int levelNum = 1;

	// Use this for initialization
	void Start () {
		Awake ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Make this game object and all its transform children
	// survive when loading a new scene.
	private void Awake () {
		DontDestroyOnLoad(transform.gameObject);
	}

	// update level to be played through button
	// clicks
	public void updateLevelNum(int newLevelNum){
		levelNum = newLevelNum;
	}

	// return the private level int
	public int getLevel(){
		return levelNum;
	}

}
