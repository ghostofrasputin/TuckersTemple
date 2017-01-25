using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    private Vector2 goalPos;
    private float speed = 0.5f;
    private Vector2 wrapPos;
    private bool wrap = false;

	// Use this for initialization
	void Start () {
        goalPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.x == goalPos.x && transform.position.y == goalPos.y)
        {
            if (wrap)
            {
                wrap = false;
                transform.position = wrapPos;
                goalPos = wrapPos;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, goalPos, speed);
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
}
