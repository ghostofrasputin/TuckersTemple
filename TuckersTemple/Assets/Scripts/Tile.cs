/*
 * Tile.cs
 * 
 * This script is attached to the tile prefab and helps it do its job
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	// public fields:
	public GameObject Wall;

	// private fields:
	private float speed = 0.05f;
	private bool wrap = false;
    private Vector2 goalPos;
	private Vector2 wrapPos;
    private GameMaster gm;
	private string tileType = "";
	
	// Update is called once per frame
	void Update () {
        if(transform.position.x != goalPos.x || transform.position.y != goalPos.y)
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPos, speed);
            if (transform.position.x == goalPos.x && transform.position.y == goalPos.y)
            {
                if (wrap)
                {
                    wrap = false;
                    transform.position = wrapPos;
                    goalPos = wrapPos;
                }
                gm.doneSliding();
            }
        }
	}
    /*
     * Slide is called by GameMaster, and moves the tile
     * x is the offset in the x direction
     * y is the offset in the y direction
     */
    public void SlideTo(Vector2 pos)
    {
        goalPos = new Vector2(pos.x + transform.position.x, pos.y + transform.position.y);
    }

    public void WrapPosition(Vector2 pos)
    {
        wrap = true;
        wrapPos = pos;
    }
    //legacy, remove later thanks
    public int wallInDir(int dir)
    {
        return 0;
    }

	// creates the tile object:
	public void setTile(string currentTileType){
		tileType = currentTileType;
		//print (tileType);

		//find and save the GameMaster
		gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMaster>();
		goalPos = transform.position;
		GameObject wall;
		GameObject wall1;
		GameObject wall2;
		GameObject wall3;
		GameObject wall4;
		/*
		// create walls if isn't a 4 path tile aka "x" tile
		if(!tileType.Equals("x")){
			// we know one wall must be created
			wall1 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
			wall1.transform.localScale = new Vector3(.01f,.05f,.1f);
			float offset = wall.GetComponent<Renderer>().bounds.size.x;
			if(tileType.Equals("T0")){
			}
			if(tileType.Equals("T1")){
			}
			if(tileType.Equals("T2")){
			}
			if(tileType.Equals("T3")){
			}
			if(tileType.Equals("L0")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);
			}
			if(tileType.Equals("L1")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);
			}
			if(tileType.Equals("L2")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);
			}
			if(tileType.Equals("L3")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);
			}
			if(tileType.Equals("I0")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);

			}
			if(tileType.Equals("I1")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);

			}
			if(tileType.Equals("I2")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);

			}
			if(tileType.Equals("I3")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);

			}
			if(tileType.Equals("V0")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);
				wall3 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall3.transform.localScale = new Vector3(.01f,.05f,.1f);
			}
			if(tileType.Equals("V1")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);
				wall3 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall3.transform.localScale = new Vector3(.01f,.05f,.1f);
			}
			if(tileType.Equals("V2")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);
				wall3 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall3.transform.localScale = new Vector3(.01f,.05f,.1f);
			}
			if(tileType.Equals("V3")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);
				wall3 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall3.transform.localScale = new Vector3(.01f,.05f,.1f);
			}
			if(tileType.Equals("N")){
				wall2 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall2.transform.localScale = new Vector3(.01f,.05f,.1f);
				wall3 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall3.transform.localScale = new Vector3(.01f,.05f,.1f);
				wall4 = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall4.transform.localScale = new Vector3(.01f,.05f,.1f);
			}
		}*/

		//create walls on tiles:
		for(int i = 0; i < 4; i++)
		{
			// create walls if it isn't a tile with 4 paths aka an "x" tile
			if(Random.value>.5)
			{
				wall = Instantiate(Wall, transform.position, Quaternion.identity, transform);
				wall.transform.localScale = new Vector3(.01f,.05f,.1f);

				//This is mostly a visual thing, so its easy to see which wall belongs to which tile
				float offset = wall.GetComponent<Renderer>().bounds.size.x;

				switch (i)
				{
				case 0:
					//Translate is a local axis, so doing Rotate AFTER Translate is important!
					// top side wall;
					wall.transform.Translate(new Vector3(0, GetComponent<Renderer>().bounds.size.x/2 - offset, 0));
					wall.transform.Rotate(new Vector3(0, 0, 90));
					break;
				case 1:
					// right side wall:
					wall.transform.Translate(new Vector3(GetComponent<Renderer>().bounds.size.x / 2 - offset, 0, 0));
					break;
				case 2:
					// bottom side wall:
					wall.transform.Translate(new Vector3(0, offset - GetComponent<Renderer>().bounds.size.x / 2, 0));
					wall.transform.Rotate(new Vector3(0, 0, 90));
					break;
				case 3:
					// left side wall:
					wall.transform.Translate(new Vector3(offset - GetComponent<Renderer>().bounds.size.x / 2, 0, 0));
					break;
				}
			}
		}
	}
}
