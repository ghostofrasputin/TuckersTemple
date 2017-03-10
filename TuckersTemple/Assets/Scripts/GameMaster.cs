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
    public GameObject outerWall;
    public GameObject Tile; //The tile prefab to spawn in
    public GameObject Character;
    public GameObject Trap;
    public GameObject Enemy;
    public GameObject Goal;
    public float tileSize; //the size of the tile prefab(should be square)
    public int numRows; //number of tiles to size
    public int numCols;
    public Canvas winScreen;
	public Canvas deathScreen;
	public int currentLevel = 1; // progress this every time there's a win

	// Sound 
	public AudioClip TileSlide1;
	public AudioClip TileSlide2;

    // private fields:
    private const int N = 0;
    private const int E = 1;
    private const int S = 2;
    private const int W = 3;
    private RaycastHit hit;
    private GameObject touchTarget;
    private bool isVert = false; //Extablishes initial movement axis of swipe
    private bool isSelected = false;
    private Vector2 lastPos = new Vector2(0,0); //holds the last position for mouse input to calculate deltaPosition
    private GameObject[][] tileGrid; // the holder for all the tiles
    private List<GameObject> playerChars = new List<GameObject>();
    private GameObject boundary;
    private bool canInputMove = true;
    private bool charsWalking = false;
    private bool tilesSliding = false;
    //This should hold all the actors in the scene, so we can iterate through it to tell them to walk(the plank, ARRRR)
    private List<GameObject> actors = new List<GameObject>();
    // playtest metrics
    private int moves = 0;
    private double time = 0;
    private bool ticking = true;
    private int attempts = 1;
	// JSON level file data:
	private LevelReader levelData;
	private List<Level> levelsList;
	// level selected from main menu:
    private Vector2 touchStart;

     void Start()
     {
		// extract level list from levelData and set vars we need later:
		levelData = Camera.main.GetComponent<LevelReader>();
		levelsList = levelData.getLevels();
		// Use for DEBUGGING if problems arise in more complicated level files:
		//levelData.printLevel(1);
		//levelData.printLevelsList();

		// This is in case no level has been selected from the main menu to avoid
		// crashing. level 1 will play by default.
		try {
			currentLevel = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getLevel();
		} catch(System.Exception){}

		generateLevel (levelsList [currentLevel - 1]);
    }

    // Update is called once per frame
    void Update()
    {
        if (ticking)
        {
            time += Time.deltaTime;
        }
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
                    HandleTouch(10, Input.mousePosition, TouchPhase.Began);
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
                    HandleTouch(10, Input.mousePosition, TouchPhase.Ended);
                }
            }
            else
            {
                //use the first touch registered
                Touch touch = Input.touches[0];
                HandleTouch(touch.fingerId, touch.position, touch.phase);
            }
        }
        else
        {
            //check if tiles are done moving and characters aren't walking
            if (!tilesSliding && !charsWalking)
            {
                charsWalking = true;
                //tell all characters to walk
                foreach (GameObject actor in actors)
                {
                    actor.GetComponent<Actor>().findNextMove(actor.GetComponent<Actor>().direction);
                }
                foreach (GameObject actor in actors)
                {
                    if (actor.GetComponent<Actor>().currDirection != -1)
                    {
                        actor.GetComponent<Actor>().checkCollide(actor.GetComponent<Actor>().currDirection);
                    }
                }
                foreach (GameObject actor in actors)
                {
                    actor.GetComponent<Actor>().walk(actor.GetComponent<Actor>().currDirection);
                }
            }   
        }
                //This is to check if an actor runs into a enemy on the same tile
        foreach (GameObject actor in actors)
        {

            if ((actor.gameObject.tag == "Player") && (!playerChars.Contains(actor)))
            {
                print("getting players");
                playerChars.Add(actor);
            }
        }
        foreach(GameObject actor in actors)
        {
            //print(actor.gameObject.tag);
            if(actor.gameObject.tag == "Enemy")
            {
                foreach(GameObject playerChar in playerChars)
                {
                    if(actor.transform.position == playerChar.transform.position)
                    {
                        playerChar.GetComponent<Actor>().death = true;
                    }
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
    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
    {
        switch (touchPhase)
        {
           case TouchPhase.Began:
               Ray ray = Camera.main.ScreenPointToRay (touchPosition);
               touchTarget = null;
                touchStart = new Vector2(touchPosition.x, touchPosition.y);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
                {
                    touchTarget = hit.collider.gameObject;
                }
                break;

            case TouchPhase.Moved:
                break;

            case TouchPhase.Ended:
                findTouchVector(touchTarget, ((Vector2)touchPosition)-touchStart);
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

		SoundController.instance.RandomSfxTiles (TileSlide1, TileSlide2);

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
        //check if touch vector is large enough to register as a swipe
        if (isVert)
        {
            if(Mathf.Abs(delta.y) < 20)
            {
                print("Touch too short to be swipe (delta: " + delta.y + ").");
                return;
            }
        }
        else
        {
            if (Mathf.Abs(delta.x) < 20)
            {
                print("Touch too short to be swipe (delta: " + delta.x + ").");
                return;
            }
        }
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
            moves++;
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
	
    public GameObject spawnActor(GameObject actor, int x, int y, int direction)
    {
        GameObject newActor = Instantiate(actor, new Vector3(tileGrid[x][y].transform.position.x, tileGrid[x][y].transform.position.y, tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform);
        //newActor.GetComponent<Actor>().setDirection(direction);
        return newActor;
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
		winScreen.GetComponent<CanvasGroup>().alpha = 0;
        winScreen.GetComponent<CanvasGroup>().interactable = false;
        winScreen.GetComponent<CanvasGroup>().blocksRaycasts = false;
		deathScreen.GetComponent<CanvasGroup>().alpha = 0;
		deathScreen.GetComponent<CanvasGroup>().interactable = false;
		deathScreen.GetComponent<CanvasGroup>().blocksRaycasts = false;
        attempts++;
        setupLevel(levelsList[currentLevel-1]);
    }
	
	//Called when the level is won
	//Displays win screen
	public void levelWin()
	{
		turnOffTileColliders ();
		winScreen.GetComponent<CanvasGroup>().alpha = 1;
		winScreen.GetComponent<CanvasGroup>().interactable = true;
		winScreen.GetComponent<CanvasGroup>().blocksRaycasts = true;
        ticking = false;
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("playtest.txt", true))
        {
            file.WriteLine("\"" + levelsList[currentLevel - 1].Name + "\" beaten in " + moves
            + " moves in " + System.Math.Round(time, 2) + " seconds in " + attempts + " attempts.");
        }
        moves = 0;
        time = 0;
        attempts = 0;
	}

	// displays death screen:
	public void levelDeath(){
		turnOffTileColliders ();
		deathScreen.GetComponent<CanvasGroup>().alpha = 1;
		deathScreen.GetComponent<CanvasGroup>().interactable = true;
		deathScreen.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	public void nextLevel()
	{
        currentLevel++;
        reset();
        ticking = true;
	}
	
	public void turnOffTileColliders()
	{
		for (int i = 0; i < numCols; i++)
		{
			for (int j = 0; j < numRows; j++)
			{
				tileGrid[i][j].GetComponent<BoxCollider> ().enabled = false;
			}
		}
	}

	public void turnOnTileColliders()
	{
		for (int i = 0; i < numCols; i++)
		{
			for (int j = 0; j < numRows; j++)
			{
				tileGrid[i][j].GetComponent<BoxCollider> ().enabled = true;
			}
		}
	}
	
    public void setupLevel(Level level)
    {
        // clear everything, then regenerate the level
        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
		        foreach(Transform child in tileGrid[i][j].transform)
                {
                    Destroy(child.gameObject);
                }
                Destroy(tileGrid[i][j]);
            }
        }
        actors.Clear();
        playerChars.Clear();
        tileGrid = new GameObject[numCols][];
        generateLevel(level);
        Destroy(boundary);
    }

    // takes in the current level and creates it:
    public void generateLevel(Level currentLevel){
		// extract level info:
		string name = currentLevel.Name;
		numRows = currentLevel.Rows;
		numCols = currentLevel.Cols;
		List<List<string>> tileInfo = currentLevel.Tiles;
		Dictionary<string,List<int>> actorInfo = currentLevel.Actors;
		Dictionary<string,List<int>> staticObjectInfo = currentLevel.StaticObjects;
		
		// Create the Tile Grid:
		tileSize = Tile.GetComponent<Renderer>().bounds.size.x; //get the size of the tile (1.6)
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
				List<string> row = tileInfo [numRows-r-1];
				string currentTileType = row [c];
				//instantiate a tile at the proper grid position
				tileGrid [c] [r] = Instantiate (Tile, new Vector3 (c * tileSize, r * tileSize, 0), Quaternion.identity);
				// pass the tile object the type indicator string where it will
				// create a tile based on that string
				tileGrid [c] [r].SendMessage ("setTile", currentTileType);
			}
		}
		
		// Create the Actors (Characters & Enemies):
		foreach (KeyValuePair<string, List<int>> kvp in actorInfo) {
			string key = kvp.Key;
			List<int> value = kvp.Value;
			if(key.Equals("roy")){
				GameObject roy = spawnActor(Character, value[0], value[1], value[2]);
				actors.Add(roy);
			}
			if(key.Equals("emily")){
				GameObject emily = spawnActor(Character, value[0], value[1], value[2]);
				actors.Add(emily);
			}
			if(key.Equals("jake")){
				GameObject jake = spawnActor(Character, value[0], value[1], value[2]);
				actors.Add(jake);
			}
			if(key.Equals("tank")){
				GameObject tank = spawnActor(Character, value[0], value[1], value[2]);
				actors.Add(tank);
			}
			if(key.Contains("shadow")){
				GameObject enemy = spawnActor(Enemy, value[0], value[1], value[2]);
				actors.Add(enemy);
			}
		}
		
		// Create the Static Objects (aka goal & traps):
		foreach (KeyValuePair<string, List<int>> kvp in staticObjectInfo) {
			string key = kvp.Key;
			List<int> value = kvp.Value;
			if (key.Equals("goal")) {
				int x = value [0];
				int y = value [1];
				Instantiate (Goal, new Vector3 (tileGrid [x] [y].transform.position.x, tileGrid [x] [y].transform.position.y, 
					tileGrid [x] [y].transform.position.z), Quaternion.identity, tileGrid [x] [y].transform);
			}
			if (key.Contains("trap")) {
				int x = value [0];
				int y = value [1];
				Instantiate (Trap, new Vector3 (tileGrid [x] [y].transform.position.x, tileGrid [x] [y].transform.position.y, 
					tileGrid [x] [y].transform.position.z), Quaternion.identity, tileGrid [x] [y].transform);
			}
		}
		//Add in outer walls to the grid
		boundary = Instantiate(outerWall, Vector3.zero, Quaternion.identity);
		boundary.transform.localScale = new Vector3( (numCols + 1) * tileSize , (numRows + 1) * tileSize, 0);
		boundary.transform.position = new Vector3((numCols + 1) * tileSize / 4, (numRows + 1) * tileSize / 4, 0);
	}
}
// end of code