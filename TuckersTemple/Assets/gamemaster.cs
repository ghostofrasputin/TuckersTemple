using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamemaster : MonoBehaviour {
    // first number how many sets of arrays
    // second number is how many elements per array 
    // so far holding ints, need to make tiles.
    public int[,] array2D = new int[,] { { 0, 0 } };
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public class tile
    {
        public bool isGoal;
        public bool hasCharas;
        public int[] directions;

        public tile()
        {
            isGoal = false;
            hasCharas = false;
            directions = new int[4];
        }
        public tile(bool goal, bool charas, int[] paths)
        {
            isGoal = goal;
            hasCharas = charas;
             directions = paths;
        }
    }
}
