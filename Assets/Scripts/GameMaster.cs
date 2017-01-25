using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMaster : MonoBehaviour
{
    // public fields:
    public float slideSpeed = .02f;

    // private fields:
    RaycastHit hit;
    private GameObject touchTarget;
    private bool isDrag = false; //tracks if valid object is hit for drag
    private bool isVert = false; //Extablishes initial movement axis of swipe
    private bool isLatched = false; //locks movement axis to initial direction of swipe
    private Vector2 lastPos = new Vector2(0,0); //holds the last position for mouse input to calculate deltaPosition
    private float totalOffset = 0; //holds total offset for a move, to keep it locked to 1 tile away
    public GameObject Tile; //The tile prefab to spawn in
    private float tileSize; //the size of the tile prefab(should be square)
    public int numRows = 2; //number of tiles to size
    public int numCols = 2;
    private GameObject[][] tileGrid; // the holder for all the tiles

     void Start()
    {
        //get the size of the tile
        tileSize = Tile.GetComponent<Renderer>().bounds.size.x;
        //initialize the first array
        tileGrid = new GameObject[numCols][];
        //iterate through columns
        for(int c = 0; c < numCols; c++)
        {
            //initialize the secondary arrays
            tileGrid[c] = new GameObject[numRows];
            //iterate through rows
            for(int r = 0; r < numRows; r++)
            {
                //instantiate a tile at the proper grid position
                tileGrid[c][r] = Instantiate(Tile, new Vector3(c*tileSize,r*tileSize,0), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //This code either uses touch input if it exists,
        //or uses mouse input if exists and converts it into fake touch input

        // Simulate touch events from mouse events with dummy ID out of range
        if (Input.touchCount == 0)
        {
            //Calls when mouse is first pressed(begin)
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch(10, Input.mousePosition, TouchPhase.Began, new Vector2(0,0));
                //store the last position for next tick
                lastPos = Input.mousePosition;
            }
            //called when mouse his held down(moved)
            if (Input.GetMouseButton(0))
            {
                //calculate the offset vector
                Vector2 offset = new Vector2(Input.mousePosition.x - lastPos.x, Input.mousePosition.y - lastPos.y);
                HandleTouch(10, Input.mousePosition, TouchPhase.Moved, offset);
                //store the last position for next tick
                lastPos = Input.mousePosition;
            }
            //called when mouse is lifted up(ended)
            if (Input.GetMouseButtonUp(0))
            {
                HandleTouch(10, Input.mousePosition, TouchPhase.Ended, Vector2.zero);
                //reset the total offset
                totalOffset = 0;
            }
        }
        else
        {
            //use the first touch registered
            Touch touch = Input.touches[0];
            HandleTouch(touch.fingerId, Camera.main.ScreenToWorldPoint(touch.position), touch.phase, touch.deltaPosition);
        }
    }

    //this function handles touch input
    /*
     * touchFingerId is the touch index, 10 for mouse
     * touchPosition is where on the screen the touch is
     * touchPhase is either Began, Moved, or Ended
     * deltaPosition is a vector of the difference in position since last tick
     */
    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase, Vector2 deltaPosition )
    {
        switch (touchPhase)
        {
            case TouchPhase.Began:
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                if (Physics.Raycast(ray, out hit))
                {
                    touchTarget = hit.collider.gameObject;
                    isDrag = true;
                }
                break;

            case TouchPhase.Moved:
                moveObject(touchTarget, deltaPosition);
                break;

            case TouchPhase.Ended:

                isDrag = false;
                isLatched = false;
                isVert = false;
                break;

            default:
                break;

        }
    }

    private void moveObject(GameObject obj, Vector2 delta)
    {
        print("before: " + delta);
        delta = Camera.main.ScreenToWorldPoint(delta);
        print("after: " + delta);
        if (isDrag)
        {
            if (!isLatched && delta != Vector2.zero)
            {
                isVert = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
                isLatched = true;
            }
            if (isVert)
            {
                delta.x = 0;
                delta.y *= slideSpeed;
            }
            else
            {
                delta.y = 0;
                delta.x *= slideSpeed;
            }
            totalOffset += (delta.x + delta.y) * slideSpeed;
            if(totalOffset > tileSize)
            {
                if(delta.x > 0)
                {
                    delta.x -= totalOffset - tileSize;
                }
                else
                {
                    delta.y -= totalOffset - tileSize;
                }
                totalOffset = tileSize;
            }
            if(totalOffset < -tileSize)
            {
                if (delta.x < 0)
                {
                    delta.x += totalOffset - tileSize;
                }
                else
                {
                    delta.y += totalOffset - tileSize;
                }
                totalOffset = -tileSize;
            }



            obj.transform.Translate(delta.x, delta.y, 0);
           
            
            //Old offset code, didn't really work
            /*
            //add the change to total offset
            print("offset " + totalOffset);
            print("tilesize " + tileSize);
            totalOffset += (delta.x + delta.y) * slideSpeed;
            float relativeTileSize = tileSize;
            //if the offset is too large
            if (totalOffset > relativeTileSize || totalOffset < -relativeTileSize)
            {
                delta.x = 0;
                delta.y = 0;
                //float slideDifference = 0;
                if (totalOffset > 0)
                {
                    //slideDifference = totalOffset - tileSize;
                    totalOffset = relativeTileSize;
                }
                else
                {
                    totalOffset = -relativeTileSize;
                }
            }*/
        }
    }
    
}
