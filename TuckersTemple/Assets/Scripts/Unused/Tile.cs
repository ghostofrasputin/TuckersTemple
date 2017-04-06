﻿/*
 * Tile.cs
 * 
 * This script is attached to the tile prefab and helps it do its job
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    // public fields:
    public GameObject Wall;
    public Sprite upWall;
    public Sprite rightWall;
    public Sprite downWall;
    public Sprite leftWall;

    // private fields:
    private float speed = 0.05f;
    private bool wrap = false;
    private Vector2 goalPos;
    private Vector2 wrapPos;
    private GameMaster gm;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x != goalPos.x || transform.position.y != goalPos.y)
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
    public void setTile(string currentTileType)
    {
        //print (tileType);

        //find and save the GameMaster
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMaster>();
        goalPos = transform.position;
        int[] wallCheck = { 0, 0, 0, 0 };
        GameObject wall;

        switch (currentTileType)
        {
            case "x":
            case "╬":
                break;
            case "T0":
            case "╦":
                wallCheck[0] = 1;
                break;
            case "T1":
            case "╣":
                wallCheck[1] = 1;
                break;
            case "T2":
            case "╩":
                wallCheck[2] = 1;
                break;
            case "T3":
            case "╠":
                wallCheck[3] = 1;
                break;
            case "I0":
            case "║":
                wallCheck[1] = 1;
                wallCheck[3] = 1;
                break;
            case "I1":
            case "═":
                wallCheck[0] = 1;
                wallCheck[2] = 1;
                break;
            case "L0":
            case "╚":
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                break;
            case "L1":
            case "╔":
                wallCheck[0] = 1;
                wallCheck[3] = 1;
                break;
            case "L2":
            case "╗":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                break;
            case "L3":
            case "╝":
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                break;
            case "V0":
            case "u":
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                break;
            case "V1":
            case "[":
                wallCheck[0] = 1;
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                break;
            case "V2":
            case "n":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                wallCheck[3] = 1;
                break;
            case "V3":
            case "]":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                break;
            case "N":
            case "¤":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                break;
        }
        for (int i = 0; i < 4; i++)
        {
            int currentWallBoolean = wallCheck[i];
            if (currentWallBoolean == 1)
            {
                wall = Instantiate(Wall, transform.position, Quaternion.identity, transform);
                // wall.transform.localScale = new Vector3 (.01f, .05f, .1f);
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                float offset = wall.GetComponent<Renderer>().bounds.size.x;
                // right wall:
                if (i == 0)
                {
                    sr.sprite = upWall;
                    wall.transform.Translate(new Vector3(0, offset, 0));
                }
                // right wall:
                if (i == 1)
                {
                    sr.sprite = rightWall;
                    wall.transform.Translate(new Vector3(offset, 0.1f, 0));
                    wall.transform.localScale = new Vector3(1, 1.4f, 1);
                }
                // bottom wall:
                if (i == 2)
                {
                    sr.sprite = downWall;
                    wall.transform.Translate(new Vector3(0, offset * -1, 0));
                }
                // left wall:
                if (i == 3)
                {
                    sr.sprite = leftWall;
                    wall.transform.Translate(new Vector3(offset * -1, 0.1f, 0));
                    wall.transform.localScale = new Vector3(1, 1.4f, 1);
                }
            }
        }
    }
}