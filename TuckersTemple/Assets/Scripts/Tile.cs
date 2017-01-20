using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
	// fields:
	public bool isGoal;
	public bool hasCharas;
	public int[] directions;

	public Tile(){
		isGoal = false;
		hasCharas = false;
		directions = new int[4];
	}
	public Tile(bool goal, bool charas, int[] paths){
		isGoal = goal;
		hasCharas = charas;
		directions = paths;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
