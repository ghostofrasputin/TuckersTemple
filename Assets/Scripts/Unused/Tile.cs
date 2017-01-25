using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /*
     * Slide is called by GameMaster, and moves the tile
     * x is the offset in the x direction
     * y is the offset in the y direction
     */
    public void Slide(float x, float y)
    {
        transform.Translate(x, y, 0);
    }
}
