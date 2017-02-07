/*
 * GameMaster.cs
 * 
 * This script does like everything.  Be careful.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class GameMaster : MonoBehaviour
{
    // public fields:
    public float slideSpeed = .02f;
<<<<<<< HEAD
	public GameObject outerWall;
	public GameObject Tile; //The tile prefab to spawn in
	public GameObject Character;
	public GameObject Trap;
	public GameObject Enemy;
	public float tileSize; //the size of the tile prefab(should be square)
	public int numRows = 2; //number of tiles to size
	public int numCols = 2;
=======
    public GameObject outerWall;
    public GameObject Tile; //The tile prefab to spawn in
    public GameObject Character;
    public GameObject Trap;
    public GameObject Enemy;
    public float tileSize; //the size of the tile prefab(should be square)
    public int numRows = 2; //number of tiles to size
    public int numCols = 2;
    public Canvas winScreen;
>>>>>>> master

    // private fields:
    private const int N = 0;
    private const int E = 1;
    private const int S = 2;
    private const int W = 3;
    private RaycastHit hit;
    private GameObject touchTarget;
    private bool isDrag = false; //tracks if valid object is hit for drag
    private bool isVert = false; //Extablishes initial movement axis of swipe
    private bool isLatched = false; //locks movement axis to initial direction of swipe
    private bool isSelected = false;
    private Vector2 lastPos = new Vector2(0,0); //holds the last position for mouse input to calculate deltaPosition
    private float totalOffset = 0; //holds total offset for a move, to keep it locked to 1 tile away
    private GameObject[][] tileGrid; // the holder for all the tiles
    private GameObject roy; //Roy is private, he just likes it that way
<<<<<<< HEAD
	private GameObject enemy; 
=======
    private GameObject goal;
	//private GameObject enemy; // enemies also keep private affairs, right?
>>>>>>> master
    private bool canInputMove = true;
    private bool charsWalking = false;
    private bool tilesSliding = false;
    //This should hold all the actors in the scene, so we can iterate through it to tell them to walk(the plank, ARRRR)
    private List<GameObject> actors = new List<GameObject>();
    
     void Start()
     {
		//get the size of the tile (1.6)
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
					tileGrid [c] [r] = Instantiate (Tile, new Vector3 (c * tileSize, r * tileSize, 0), Quaternion.identity);

            }
        }
        Instantiate(Trap, new Vector3(tileGrid[2][2].transform.position.x, tileGrid[2][2].transform.position.y, tileGrid[2][2].transform.position.z), Quaternion.identity,tileGrid[2][2].transform);
        roy = Instantiate(Character, new Vector3(tileGrid[0][0].transform.position.x, tileGrid[0][0].transform.position.y, tileGrid[0][0].transform.position.z), Quaternion.identity, tileGrid[0][0].transform);
        actors.Add(roy);
        goal = Instantiate(Goal, new Vector3(tileGrid[1][1].transform.position.x, tileGrid[1][1].transform.position.y, tileGrid[1][1].transform.position.z), Quaternion.identity, tileGrid[1][1].transform);

<<<<<<< HEAD
		enemy = Instantiate(Enemy, new Vector3(tileGrid[1][0].transform.position.x, tileGrid[0][0].transform.position.y, tileGrid[0][0].transform.position.z), Quaternion.identity, tileGrid[0][0].transform);
		actors.Add(enemy);
=======
		//enemy = Instantiate(Character, new Vector3(tileGrid[2][0].transform.position.x, tileGrid[0][0].transform.position.y, tileGrid[0][0].transform.position.z), Quaternion.identity, tileGrid[0][0].transform);
		//actors.Add(enemy);
>>>>>>> master

        //Add in outer walls to the grid
        outerWall = Instantiate(outerWall, Vector3.zero, Quaternion.identity);
        outerWall.transform.localScale = new Vector3( (numCols + 1) * tileSize , (numRows + 1) * tileSize, 0);
        outerWall.transform.position = new Vector3((numCols + 1) * tileSize / 4, (numRows + 1) * tileSize / 4, 0);
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
            //check if tiles are done moving and characters aren't walking
            if (!tilesSliding && !charsWalking)
            {
                charsWalking = true;
                //tell all characters to walk
                foreach(GameObject actor in actors)
                {
                    actor.GetComponent<Actor>().walk();
<<<<<<< HEAD
					//actor.GetComponent<Enemy>().walk();
=======
		    // actor.GetComponent<Enemy>().walk();
>>>>>>> master
                }
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
    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase, Vector2 deltaPosition)
    {
        switch (touchPhase)
        {
           case TouchPhase.Began:
               Ray ray = Camera.main.ScreenPointToRay (touchPosition);
               touchTarget = null;
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
        //Set some bools to stop players from entering another move while animations run
        canInputMove = false;
        tilesSliding = true;
        //calculate normal offset vector and move the tiles
        Vector2 offset = new Vector2(0, 0);
        GameObject temp;
        switch (dir)
        {
            /*
             * What follows is a bunch of suprisingly straightforward 2D array logic.  It will be explained once here instead of in each loop individually
             * offset = tileSize //Set the offset vector to the appropriate direction based on dir(which we know from the switch)
             * temp = tilegrid //Save one of the tiles to a temp variable, so we don't lose it when shifting things
             * for() //Iterate through the designated column or row.  Some of these are negative because thats the direction we have to go
             *          //note that we stop or start a little early
             * tilegrid = tilegrid //replace the current index with the next index, in whatever direction we are moving
             * tilegrid...SlideTo //tell a tile to slide to its new position
             * }
             * tilegrid = temp // return temp to its new position
             * tileGrid...WrapPosition //That temp tile needs to wrap around to the other side, so this is what does it.  Note the positional vector, and how it uses no offsets
             *                          //This is because we are bad programmers, so currently the world's (0,0) needs to be the bottom left of the tile grid
             * tileGrid...SlideTo //Tell that last tile to slide
             * //We did it!  If you have any questions, ask Andrew or Elliot, but they probably don't understand it any more than was written here.
             * //We sacrificied a few animals to make the numbers work, so don't change them unless you know what you're doing!
             */
            case N:
                offset.y = tileSize;
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

    //After a swipe is finished, this is called with the touched object and the total delta of the swipe
    //It uses that information to decide which row or col needs to move, and in what direction.
    private void findTouchVector(GameObject obj, Vector2 delta)
    {
        //check which direction had the greater offset
        isVert = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
        int dir = -1;
        //check if it is vertical or horizontal, then if it is positive or negative
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
        //Loop through the tiles until you find the right one
        //This can probably be optimized
		isSelected = false;
        for (int c = 0; c < numCols; c++)
        {
            for (int r = 0; r < numRows; r++)
            {
                if(tileGrid[c][r].Equals(obj))
                {
                 row = r;
                 col = c;
                 isSelected = true;
                 break;
                }
            }
        }
        //Tell the grid to move with the information we now have!
		// only if a tile is selected
		if (isSelected == true) 
		{
			moveGrid(col, row, dir);
		}
	}

    /*
     * getTile returns a reference to the specified tile
     * Params: col - the tile's column
     *         row - the tile's row
     * Prereq: col,row are not out of bounds
     */
    public GameObject getTile(int col, int row)
    {
        return tileGrid[col][row];
    }
    /*
     * getTile returns a reference to the specified tile
     * Params: pos - a vector2 of where the tile is on the grid
     * Prereq: pos is not out of bounds
     */
    public GameObject getTile(Vector2 pos)
    {
        //These +0.01f are sacrifices to the devil, don't adjust them or
        //The floating point math gods will smite you from the earth
        int col = Mathf.FloorToInt((pos.x + 0.01f) / tileSize);
        int row = Mathf.FloorToInt((pos.y + 0.01f) / tileSize);
        return tileGrid[col][row];
    }

    //This is called by tiles when they are done with their slide animation, so characters can start walking
    //It may eventually need to count how many tiles are done and wait for them all.
    public void doneSliding()
    {
        tilesSliding = false;
    }
    //This is called by actors when they are done moving, and lets the player swipe a new move
    //It may need to eventually count how many actors are done, and wait for them all
    //Especially if some actors take longer to walk than others.
    public void doneWalking()
    {
        charsWalking = false;
        canInputMove = true;
    }
	
	// remove an actor once they're dead
	public void deleteActor(GameObject actor)
	{
		actors.Remove (actor);
	}

	
	// this will load the current level scene 
	// as of right now, levels are being generated, so
	// it reloads the level, but the tiles will be different
	public void reset()
	{
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
	}

	//Called when the level is won
	//Displays win screen
	public void levelWin()
	{
		winScreen.GetComponent<CanvasGroup>().alpha = 1;
		winScreen.GetComponent<CanvasGroup>().interactable = true;
		winScreen.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	public void nextLevel()
	{
		winScreen.GetComponent<CanvasGroup>().alpha = 0;
		winScreen.GetComponent<CanvasGroup>().interactable = false;
		winScreen.GetComponent<CanvasGroup>().blocksRaycasts = false;
		reset();
	}
}
