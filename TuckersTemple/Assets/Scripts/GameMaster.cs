using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMaster : MonoBehaviour
{
    // public fields:
    public float slideSpeed = .02f;


    // private fields:
    private const int N = 0;
    private const int E = 1;
    private const int S = 2;
    private const int W = 3;
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
    public GameObject Character;
    private GameObject roy;
    private bool canInputMove = true;
    private bool charsWalking = false;
    private bool tilesSliding = false;
    private List<GameObject> actors = new List<GameObject>();

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
        roy = Instantiate(Character, new Vector3(tileGrid[0][0].transform.position.x, tileGrid[0][0].transform.position.y, tileGrid[0][0].transform.position.z), Quaternion.identity, tileGrid[0][0].transform);
        actors.Add(roy);
    }

    // Update is called once per frame
    void Update()
    {
        //This code either uses touch input if it exists,
        //or uses mouse input if exists and converts it into fake touch input

        // Simulate touch events from mouse events with dummy ID out of range
        if (canInputMove)
        {
            if (Input.touchCount == 0)
            {
                //Calls when mouse is first pressed(begin)
                if (Input.GetMouseButtonDown(0))
                {
                    HandleTouch(10, Input.mousePosition, TouchPhase.Began, new Vector2(0, 0));
                    //store the last position for next tick
                    lastPos = Input.mousePosition;
                }
                //called when mouse his held down(moved)
                if (Input.GetMouseButton(0))
                {

                }
                //called when mouse is lifted up(ended)
                if (Input.GetMouseButtonUp(0))
                {
                    Vector2 offset = new Vector2(Input.mousePosition.x - lastPos.x, Input.mousePosition.y - lastPos.y);
                    HandleTouch(10, Input.mousePosition, TouchPhase.Ended, offset);
                    //reset the total offset
                    totalOffset = 0;
                }
            }
            else
            {
                //use the first touch registered
                Touch touch = Input.touches[0];
                HandleTouch(touch.fingerId, touch.position, touch.phase, touch.deltaPosition);
            }
        }
        else
        {
            //check if tiles are done moving
            if (!tilesSliding && !charsWalking)
            {
                charsWalking = true;
                //tell all characters to walk

            }   
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
                }
                break;

            case TouchPhase.Moved:
                break;

            case TouchPhase.Ended:
                findTouchVector(touchTarget, deltaPosition);
                break;

            default:

                break;

        }
    }

    //takes in row, col, and offset, and then then tells the correct tiles to move
    private void moveGrid(int col, int row, int dir)
    {
        canInputMove = false;
        tilesSliding = true;
        Vector2 offset = new Vector2(0, 0);
        GameObject temp;
        //calculate normal offset vector and move the tiles
        switch (dir)
        {
            case N:
                offset.y = tileSize;
                //Update grid
                temp = tileGrid[col][numRows - 1];
                for (int r = numRows - 1; r > 0; r--)
                {
                    tileGrid[col][r] = tileGrid[col][r-1];
                    tileGrid[col][r-1].GetComponent<Tile>().SlideTo(offset);
                }
                tileGrid[col][0] = temp;
                tileGrid[col][0].GetComponent<Tile>().WrapPosition(new Vector2(tileSize * col,0));
                tileGrid[col][0].GetComponent<Tile>().SlideTo(offset);
                break;
            case S:
                offset.y = -tileSize;
                temp = tileGrid[col][0];
                for (int r = 0; r < numRows - 1; r++)
                {
                    tileGrid[col][r] = tileGrid[col][r + 1];
                    tileGrid[col][r].GetComponent<Tile>().SlideTo(offset);
                }
                tileGrid[col][numRows - 1] = temp;
                tileGrid[col][numRows - 1].GetComponent<Tile>().WrapPosition(new Vector2(tileSize * col, (numRows-1)*tileSize));
                tileGrid[col][numRows - 1].GetComponent<Tile>().SlideTo(offset);
                break;
            case E:
                offset.x = tileSize;
                //Update grid
                temp = tileGrid[numCols - 1][row];
                for (int c = numCols - 1; c > 0; c--)
                {
                    tileGrid[c][row] = tileGrid[c - 1][row];
                    tileGrid[c - 1][row].GetComponent<Tile>().SlideTo(offset);
                }
                tileGrid[0][row] = temp;
                tileGrid[0][row].GetComponent<Tile>().WrapPosition(new Vector2(0, tileSize * row));
                tileGrid[0][row].GetComponent<Tile>().SlideTo(offset);
                break;
            case W:
                offset.x = -tileSize;
                temp = tileGrid[0][row];
                for (int c = 0; c < numCols - 1; c++)
                {
                    tileGrid[c][row] = tileGrid[c+1][row];
                    tileGrid[c][row].GetComponent<Tile>().SlideTo(offset);
                }
                tileGrid[numCols - 1][row] = temp;
                tileGrid[numCols - 1][row].GetComponent<Tile>().WrapPosition(new Vector2((numCols - 1) * tileSize, tileSize * row));
                tileGrid[numCols - 1][row].GetComponent<Tile>().SlideTo(offset);
                break;
        }
    }

    private void findTouchVector(GameObject obj, Vector2 delta)
    {
        isVert = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
        int dir = -1;
        if (isVert)
        {
            dir = N;
            if(delta.y < 0)
            {
                dir = S;
            }
        }
        else
        {
            dir = E;
            if(delta.x < 0)
            {
                dir = W;
            }
        }

        int row = -1;
        int col = -1;

        //get row, col from obj
        for (int c = 0; c < numCols; c++)
        {
            for (int r = 0; r < numRows; r++)
            {
                if(tileGrid[c][r].Equals(obj))
                {
                    row = r;
                    col = c;
                }
            }
        }
        moveGrid(col, row, dir);
    }

    public GameObject getTile(int col, int row)
    {
        return tileGrid[col][row];
    }

    public void doneSliding()
    {
        tilesSliding = false;
    }
    public void doneWalking()
    {
        charsWalking = false;
        canInputMove = true;
    }
}
