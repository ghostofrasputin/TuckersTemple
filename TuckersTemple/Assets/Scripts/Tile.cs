/*
 * Tile.cs
 * 
 * This script is attached to the tile prefab and helps it do its job
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    private Vector2 goalPos;
    private float speed = 0.05f;
    private Vector2 wrapPos;
    private bool wrap = false;
    public GameObject Wall;
    private GameMaster gm;

	// Use this for initialization
	void Start () {
        //find and save the GameMaster
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMaster>();

        goalPos = transform.position;
        GameObject wall;
        for(int i = 0; i < 4; i++)
        {
            if(Random.value > 0.5)
            {
                wall = Instantiate(Wall, transform.position, Quaternion.identity, transform);
                wall.transform.localScale = new Vector3(.01f,.05f,.1f);

                //This is mostly a visual thing, so its easy to see which wall belongs to which tile
                float offset = wall.GetComponent<Renderer>().bounds.size.x;

                switch (i)
                {
                    case 0:
                        //Translate is a local axis, so doing Rotate AFTER Translate is important!
                        wall.transform.Translate(new Vector3(0, GetComponent<Renderer>().bounds.size.x/2 - offset, 0));
                        wall.transform.Rotate(new Vector3(0, 0, 90));
                        break;
                    case 1:
                        wall.transform.Translate(new Vector3(GetComponent<Renderer>().bounds.size.x / 2 - offset, 0, 0));
                        break;
                    case 2:
                        wall.transform.Translate(new Vector3(0, offset - GetComponent<Renderer>().bounds.size.x / 2, 0));
                        wall.transform.Rotate(new Vector3(0, 0, 90));
                        break;
                    case 3:
                        wall.transform.Translate(new Vector3(offset - GetComponent<Renderer>().bounds.size.x / 2, 0, 0));
                        break;
                }
            }
        }
	}
	
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
}
