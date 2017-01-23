// Tucker's Temple
// Kenny Wong
// 1/20/2017

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSlide : MonoBehaviour {

    public GameObject tile1;
    public GameObject tile2;
    public GameObject tile3;

    public GameObject testtile;

	// Use this for initialization
	void Start () {
        testtile = Instantiate(tile1, new Vector3(-2.0f, 0, 0), Quaternion.identity);
        Instantiate(tile2, new Vector3(0, 0, 0), Quaternion.identity);
        Instantiate(tile3, new Vector3(2.0f, 0, 0), Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            testtile.transform.Translate(Vector3.up);
            print("up key pressed");
        }
	}

    // prerequisite: touch controls resolve to a row/column and a direction to move
    /**
     * save the position of the first tile in the moved row/col (the tail)
     * for(each tile in designated row or column){
     *      shift position by one tile in appropriate direction
     *      if it's the last one that would loop, instead move the position to the saved tail position.
     * }
    **/
}
