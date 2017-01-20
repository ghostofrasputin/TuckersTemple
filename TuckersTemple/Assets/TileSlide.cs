// Tucker's Temple
// Kenny Wong
// 1/19/2017

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSlide : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
