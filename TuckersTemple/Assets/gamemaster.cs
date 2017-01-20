using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class gamemaster : MonoBehaviour {
    // first number how many sets of arrays
    // second number is how many elements per array 
    // so far holding ints, need to make tiles.
    public int[,] array2D = new int[,] { { 0, 0 } };

	public GameObject tile;

	private GameObject touchTarget;

	private Vector3 objCenter;
	private Vector3 touchPos;
	private Vector3 offset;
	private Vector3 newObjCenter;

	RaycastHit hit;

	private bool isDrag    = false;
	private bool isLatched = false;
	private bool isVert    = false;
	private float netDrag  = 0f;


	// Use this for initialization
	void Start () {
		Instantiate (tile, new Vector3(0,0,0), Quaternion.identity);
		print ("Tile instantiated");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	/*
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
    }*/
}
