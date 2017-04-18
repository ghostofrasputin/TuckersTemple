﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameMasterFSM : MonoBehaviour
{
    // public:
    public GameObject gm;
    public GameObject outerWall;
    public GameObject Tile; //The tile prefab to spawn in
    public GameObject Character;
    public GameObject Trap;
    public GameObject Enemy;
    public GameObject Goal;
	public GameObject Laser;
    public FSMSystem fsm;
    public Vector2 lastPos = new Vector2(0, 0); //holds the last position for mouse input to calculate deltaPosition
    public int numRows; //number of tiles to size
    public int numCols;
    public int Column;
    public int Row;
    public int Direction;
    public Canvas winScreen;
    public GameObject deathScreen;
    public List<GameObject> actors;
    public List<GameObject> characters;
    public List<GameObject> enemies;
	public List<GameObject> lasers;
    public int currentLevel = 1; // progress this every time there's a win
    public List<Level> levelsList;
    public float tileSize;
    public double time = 0;
    public bool ticking = true;
    public int attempts = 1;
    public GameObject boundary;
    public List<GameObject> playerChars = new List<GameObject>();
    public Text deathText;

    // audio:
    public AudioClip TileSlide1;
    public AudioClip TileSlide2;
    public AudioClip nextLevelSound;
    public AudioClip playerdeathSound;

    // private:
    private RaycastHit hit;
    private GameObject touchTarget;
    private Vector2 touchStart;
    private int moves = 0;
    private GameObject[][] tileGrid;
    private List<GameObject> tiles = new List<GameObject>();


    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    public void Start()
    {
        MakeFSM();
    }

    public void Update()
    {
        //print(tag + " == " + fsm.CurrentStateID);
        fsm.CurrentState.Reason(gm, gameObject);
        fsm.CurrentState.Act(gm, gameObject);
    }

    private void MakeFSM()
    {
        InitState init = new InitState(this);
        init.AddTransition(Transition.LevelLoaded, StateID.Juice);

        InputState ready = new InputState(this);
        ready.AddTransition(Transition.InputReceived, StateID.OrderTiles);
		ready.AddTransition (Transition.RestartedLevel, StateID.InitLevel);

        OrderTilesState tile = new OrderTilesState(this);
        tile.AddTransition(Transition.TilesDone, StateID.OrderActors);

        OrderActorsState actor = new OrderActorsState(this);
        actor.AddTransition(Transition.ActorsDone, StateID.Ready);
        actor.AddTransition(Transition.LevelDone, StateID.LevelWon);
        actor.AddTransition(Transition.ActorDied, StateID.LevelDeath);

        LevelWonState win = new LevelWonState(this);
        win.AddTransition(Transition.NextLevel, StateID.InitLevel);

        LevelDeathState death = new LevelDeathState(this);
        death.AddTransition(Transition.RestartedLevelFromDeath, StateID.InitLevel);

        LevelJuiceState juice = new LevelJuiceState(this);
        juice.AddTransition(Transition.DoneJuicing, StateID.Ready);

        fsm = new FSMSystem();
        fsm.AddState(init);
        fsm.AddState(ready);
        fsm.AddState(tile);
        fsm.AddState(actor);
        fsm.AddState(win);
        fsm.AddState(death);
        fsm.AddState(juice);
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
        actors.Remove(actor);
    }

    // this will load the current level scene 
    // as of right now, levels are being generated, so
    // it reloads the level, but the tiles will be different
    public void reset()
    {
		deathScreen.SetActive(false);
        attempts++;
		//check if last level
		if (currentLevel > levelsList.Count) {
			GameObject.Find("endOfDemo").GetComponent<InGameMenuManager>().playAnim("winEnter");
			GameObject.FindGameObjectWithTag ("Zombie").GetComponent<ZombiePasser> ().setLevel (1);
			return;
		}
        setupLevel(levelsList[currentLevel - 1]);
		if (fsm.CurrentStateID == StateID.LevelDeath) {
			GetComponent<GameMasterFSM> ().SetTransition (Transition.RestartedLevelFromDeath); //to ready
		} else if (fsm.CurrentStateID == StateID.Ready) {
			GetComponent<GameMasterFSM> ().SetTransition (Transition.RestartedLevel);
		} 
    }


    //Called when the level is won
    //Displays win screen
    public void levelWin()
    {
        try
        {
            // unlocks the next level. 
            //note: currentlevel-1 is the real current level for the array, currentlevel is the next level
            GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().setLockedLevelBool(currentLevel);
        }
        catch (System.Exception error)
        {
            Debug.Log(error);
        }
        turnOffTileColliders();
        winScreen.GetComponent<InGameMenuManager>().playAnim("winEnter");
        ticking = false;
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter("playtest.txt", true))
        {
            file.WriteLine("\"" + levelsList[currentLevel - 1].Name + "\" beaten in " + moves
            + " moves in " + System.Math.Round(time, 2) + " seconds in " + attempts + " attempts.");
        }
        //GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().setStars(currentLevel - 1, 3);
        moves = 0;
        time = 0;
        attempts = 0;
    }

    private void setCanvas(Canvas c, bool b)
    {
        //its beautiful don't touch it
        c.GetComponent<CanvasGroup>().alpha = (float)System.Convert.ToDouble(b);
        c.GetComponent<CanvasGroup>().interactable = b;
        c.GetComponent<CanvasGroup>().blocksRaycasts = b;
    }

    // displays death screen:
    public void levelDeath()
    {
        SoundController.instance.PlaySingle(playerdeathSound);
        turnOffTileColliders();
		deathScreen.SetActive(true);
    }

    public void nextLevel()
    {
        currentLevel++;
        reset();
        ticking = true;
        SoundController.instance.PlaySingle(nextLevelSound);
        GetComponent<GameMasterFSM>().SetTransition(Transition.NextLevel); //to ready
    }

    public void turnOffTileColliders()
    {
        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                tileGrid[i][j].GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    public void turnOnTileColliders()
    {
        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                tileGrid[i][j].GetComponent<BoxCollider>().enabled = true;
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
                foreach (Transform child in tileGrid[i][j].transform)
                {
                    Destroy(child.gameObject);
                }
                Destroy(tileGrid[i][j]);
            }
        }
        actors.Clear();
        enemies.Clear();
        characters.Clear();
        tiles.Clear();
        tileGrid = new GameObject[numCols][];
        generateLevel(level);
        Destroy(boundary);
        turnOnTileColliders();
    }

    // takes in the current level and creates it:
    public void generateLevel(Level currentLevel)
    {
        // extract level info:
        string name = currentLevel.Name;
        numRows = currentLevel.Rows;
        numCols = currentLevel.Cols;
        List<List<string>> tileInfo = currentLevel.Tiles;
        Dictionary<string, List<int>> actorInfo = currentLevel.Actors;
        Dictionary<string, List<int>> staticObjectInfo = currentLevel.StaticObjects;

        // Create the Tile Grid:
        tileSize = Tile.GetComponent<Renderer>().bounds.size.x; //get the size of the tile (1.6)
                                                                //initialize the first array
        tileGrid = new GameObject[numCols][];
        //iterate through columns
        for (int c = 0; c < numCols; c++)
        {
            //initialize the secondary arrays
            tileGrid[c] = new GameObject[numRows];
            //iterate through rows
            for (int r = 0; r < numRows; r++)
            {
                List<string> row = tileInfo[numRows - r - 1];
                string currentTileType = row[c];
                //instantiate a tile at the proper grid position
                tileGrid[c][r] = Instantiate(Tile, new Vector3(c * tileSize, r * tileSize, 0), Quaternion.identity);
                tiles.Add(tileGrid[c][r]);
                // pass the tile object the type indicator string where it will
                // create a tile based on that string
                tileGrid[c][r].SendMessage("setTile", currentTileType);
            }
        }

        // Create the Actors (Characters & Enemies):
        foreach (KeyValuePair<string, List<int>> kvp in actorInfo)
        {
            string key = kvp.Key;
            List<int> value = kvp.Value;
            if (key.Equals("roy"))
            {
                GameObject roy = spawnActor(Character, value[0], value[1], value[2]);
                actors.Add(roy);
                characters.Add(roy);
            }
            if (key.Equals("emily"))
            {
                GameObject emily = spawnActor(Character, value[0], value[1], value[2]);
                actors.Add(emily);
                characters.Add(emily);
            }
            if (key.Equals("jake"))
            {
                GameObject jake = spawnActor(Character, value[0], value[1], value[2]);
                actors.Add(jake);
                characters.Add(jake);
            }
            if (key.Equals("tank"))
            {
                GameObject tank = spawnActor(Character, value[0], value[1], value[2]);
                actors.Add(tank);
                characters.Add(tank);
            }
            if (key.Contains("shadow"))
            {
                GameObject shadow = spawnActor(Enemy, value[0], value[1], value[2]);
                actors.Add(shadow);
                enemies.Add(shadow);
            }
        }

        // Create the Static Objects (aka goal & traps):
        foreach (KeyValuePair<string, List<int>> kvp in staticObjectInfo)
        {
            string key = kvp.Key;
            List<int> value = kvp.Value;
            if (key.Equals("goal"))
            {
                int x = value[0];
                int y = value[1];
                Instantiate(Goal, new Vector3(tileGrid[x][y].transform.position.x, tileGrid[x][y].transform.position.y,
                    tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform);
            }
            if (key.Equals("trap"))
            {
                int x = value[0];
                int y = value[1];
                Instantiate(Trap, new Vector3(tileGrid[x][y].transform.position.x, tileGrid[x][y].transform.position.y,
                    tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform);
            }
			if (key.Equals ("laser")) {
				int x = value [0];
				int y = value [1];
				float offset = Tile.GetComponent<SpriteRenderer> ().bounds.size.x / 3;
				lasers.Add(Instantiate(Laser, new Vector3(tileGrid[x][y].transform.position.x - offset, tileGrid[x][y].transform.position.y,
					tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform));
			}
        }
        //Add in outer walls to the grid
        boundary = Instantiate(outerWall, Vector3.zero, Quaternion.identity);
        boundary.transform.localScale = new Vector3((numCols + 1) * tileSize, (numRows + 1) * tileSize, 0);
        boundary.transform.position = new Vector3((numCols + 1) * tileSize / 4, (numRows + 1) * tileSize / 4, 0);
    }

    public bool HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
    {
        bool touchSuccess = false;
        switch (touchPhase)
        {
            case TouchPhase.Began:
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
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
                touchSuccess = findTouchVector(touchTarget, ((Vector2)touchPosition) - touchStart);
                break;

            default:
                break;
        }
        return touchSuccess;
    }

	//called to skip animations
	public void skipAnimation(){
		//Level Juicing
		foreach (GameObject[] a in tileGrid) {
			foreach (GameObject t in a) {
				t.transform.position = t.GetComponent<TileFSM>().goalPos;
			}
		}
	}

    public bool findTouchVector(GameObject obj, Vector2 delta)
    {
        bool isVert;
        const int N = 0;
        const int E = 1;
        const int S = 2;
        const int W = 3;

        //check which direction had the greater offset
        isVert = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
        //check if touch vector is large enough to register as a swipe
        if (isVert)
        {
            if (Mathf.Abs(delta.y) < 20)
            {
                //print("Touch too short to be swipe (delta: " + delta.y + ").");
                return false;
            }
        }
        else
        {
            if (Mathf.Abs(delta.x) < 20)
            {
                //print("Touch too short to be swipe (delta: " + delta.x + ").");
                return false;
            }
        }
        int dir = -1;
        //check if it is vertical or horizontal, then if it is positive or negative
        if (isVert)
        {
            dir = N;
            if (delta.y < 0)
            {
                dir = S;
            }
        }
        else
        {
            dir = E;
            if (delta.x < 0)
            {
                dir = W;
            }
        }

        int row = -1;
        int col = -1;

        //get row, col from obj
        //Loop through the tiles until you find the right one
        //This can probably be optimized
        bool isSelected = false;
        for (int c = 0; c < numCols; c++)
        {
            for (int r = 0; r < numRows; r++)
            {
                if (tileGrid[c][r].Equals(obj))
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
        //Debug.Log("isselected" + isSelected);
        if (isSelected == true)
        {
            //print("isSelected " + isSelected);
            moves++;
            Column = col;
            Row = row;
            Direction = dir;
            return true;
            //moveGrid(col, row, dir);
        } else
        {
            return false;
        }
    }

    public void moveGrid(int col, int row, int dir)
    {
        const int N = 0;
        const int E = 1;
        const int S = 2;
        const int W = 3;
        float tileSize = tileGrid[0][0].GetComponent<Renderer>().bounds.size.x;
        SoundController.instance.RandomSfxTiles(TileSlide1, TileSlide2);

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
                    tileGrid[col][r] = tileGrid[col][r - 1];
                    tileGrid[col][r].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[col][r].transform.position.x, offset.y + tileGrid[col][r].transform.position.y);
                }
                tileGrid[col][0] = temp;
                tileGrid[col][0].GetComponent<TileFSM>().offGrid = true;
                tileGrid[col][0].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[col][0].transform.position.x, offset.y + tileGrid[col][0].transform.position.y);
                tileGrid[col][0].GetComponent<TileFSM>().wrapPos = new Vector2(tileSize * col, -tileSize);
                tileGrid[col][0].GetComponent<TileFSM>().wrapGoalPos = new Vector2(tileSize * col, 0);

                break;
            case S:
                offset.y = -tileSize;
                temp = tileGrid[col][0];
                for (int r = 0; r < numRows - 1; r++)
                {
                    tileGrid[col][r] = tileGrid[col][r + 1];
                    tileGrid[col][r].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[col][r].transform.position.x, offset.y + tileGrid[col][r].transform.position.y);
                }
                tileGrid[col][numRows - 1] = temp;
                tileGrid[col][numRows - 1].GetComponent<TileFSM>().offGrid = true;
                tileGrid[col][numRows - 1].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[col][numRows - 1].transform.position.x, offset.y + tileGrid[col][numRows - 1].transform.position.y);
                tileGrid[col][numRows - 1].GetComponent<TileFSM>().wrapPos = new Vector2(tileSize * col, numRows * tileSize);
                tileGrid[col][numRows - 1].GetComponent<TileFSM>().wrapGoalPos = new Vector2(tileSize * col, (numRows - 1) * tileSize);
                break;
            case E:
                offset.x = tileSize;
                temp = tileGrid[numCols - 1][row];
                for (int c = numCols - 1; c > 0; c--)
                {
                    tileGrid[c][row] = tileGrid[c - 1][row];
                    tileGrid[c][row].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[c][row].transform.position.x, offset.y + tileGrid[c][row].transform.position.y);
                }
                tileGrid[0][row] = temp;
                tileGrid[0][row].GetComponent<TileFSM>().offGrid = true;
                tileGrid[0][row].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[0][row].transform.position.x, offset.y + tileGrid[0][row].transform.position.y);
                tileGrid[0][row].GetComponent<TileFSM>().wrapPos = new Vector2(-tileSize, tileSize * row);
                tileGrid[0][row].GetComponent<TileFSM>().wrapGoalPos = new Vector2(0, tileSize * row);
                break;
            case W:
                offset.x = -tileSize;
                temp = tileGrid[0][row];
                for (int c = 0; c < numCols - 1; c++)
                {
                    tileGrid[c][row] = tileGrid[c + 1][row];
                    tileGrid[c][row].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[c][row].transform.position.x, offset.y + tileGrid[c][row].transform.position.y);
                }
                tileGrid[numCols - 1][row] = temp;
                tileGrid[numCols - 1][row].GetComponent<TileFSM>().offGrid = true;
                tileGrid[numCols - 1][row].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[numCols - 1][row].transform.position.x, offset.y + tileGrid[numCols - 1][row].transform.position.y);
                tileGrid[numCols - 1][row].GetComponent<TileFSM>().wrapPos = new Vector2(numCols * tileSize, row * tileSize);
                tileGrid[numCols - 1][row].GetComponent<TileFSM>().wrapGoalPos = new Vector2((numCols - 1) * tileSize, row * tileSize);
                break;
        }
    }

    public bool doneSliding()
    {
        int numTiles = numCols * numRows;
        int idleCount = 0;
        foreach (GameObject tile in tiles)
        {
            if (tile.GetComponent<TileFSM>().fsm.CurrentStateID == StateID.Idle)
            {
                ++idleCount;
            }
        }
        if (idleCount == numTiles)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool doneWalking()
    {
        int idleCount = 0;
        if (actors != null)
        {
            foreach (GameObject actor in actors)
            {
                if (actor.GetComponent<ActorFSM>().fsm.CurrentStateID == StateID.IdleA)
                {
                    ++idleCount;
                }
            }
            if (idleCount == actors.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    public bool characterDied()
    {
        foreach (GameObject character in characters)
        {
            if (character.GetComponent<ActorFSM>().fsm.CurrentStateID == StateID.EnemyDeadA || character.GetComponent<ActorFSM>().fsm.CurrentStateID == StateID.TrapDeadA)
            {
                return true;
            }
        }
        return false;
    }

    public GameObject getTile(Vector2 pos)
    {
        //These +0.01f are sacrifices to the devil, don't adjust them or
        //The floating point math gods will smite you from the earth
        int col = Mathf.FloorToInt((pos.x + 0.01f) / tileSize);
        int row = Mathf.FloorToInt((pos.y + 0.01f) / tileSize);
        return tileGrid[col][row];
    }

    public bool sameTileCollide()
    {
        foreach (GameObject character in characters)
        {
            foreach (GameObject enemy in enemies)
            {
                if (character.transform.position == enemy.transform.position)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

public class InitState : FSMState
{
    GameMasterFSM controlref;
    bool isDone;

    public InitState(GameMasterFSM control)
    {
        stateID = StateID.InitLevel;
        controlref = control;
        isDone = false;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (isDone)
        {
            npc.GetComponent<GameMasterFSM>().SetTransition(Transition.LevelLoaded); //to juice
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        //build level:
        try
        {
            controlref.levelsList = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getLevels();
            controlref.currentLevel = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getLevel();
            foreach(Level level in controlref.levelsList)
            {
                //Debug.Log(level.Name + "\n");
            }
            controlref.generateLevel(controlref.levelsList[controlref.currentLevel - 1]);
            isDone = true;
        }
        catch (System.Exception err)
        {
            Debug.Log(err);
            Debug.Log("Error: start game from mainMenu scene.");
            Application.Quit();
        }
    }

} // InitState

public class LevelJuiceState : FSMState
{
    GameMasterFSM controlref;

    public LevelJuiceState(GameMasterFSM control)
    {
        stateID = StateID.Juice;
        controlref = control;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
		if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
			controlref.skipAnimation();
		}
		if(controlref.doneSliding()){
			npc.GetComponent<GameMasterFSM>().SetTransition(Transition.DoneJuicing);
		}
    }

    public override void Act(GameObject gm, GameObject npc)
    {

    }

	public override void DoBeforeLeaving ()
	{
		foreach (GameObject child in controlref.lasers) {
			if (child.tag == "Laser") {
				child.GetComponent<LaserScript> ().setEye (true);
			}
		}
	}

} // LevelJuiceState

public class InputState : FSMState
{
    public GameMasterFSM controlref;
    private bool swiped;

    public InputState(GameMasterFSM control)
    {
        stateID = StateID.Ready;
        controlref = control;
        swiped = false;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (swiped)
        {
            npc.GetComponent<GameMasterFSM>().SetTransition(Transition.InputReceived); //to Look
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        swiped = false;
        if (Input.touchCount == 0)
        {
            //Calls when mouse is first pressed(begin)
            if (Input.GetMouseButtonDown(0))
            {
                controlref.HandleTouch(10, Input.mousePosition, TouchPhase.Began);
                //store the last position for next tick
                controlref.lastPos = Input.mousePosition;
            }
            //called when mouse his held down(moved)
            if (Input.GetMouseButton(0))
            {

            }
            //called when mouse is lifted up(ended)
            if (Input.GetMouseButtonUp(0))
            {
                swiped = controlref.HandleTouch(10, Input.mousePosition, TouchPhase.Ended);
                //Debug.Log(swiped);
            }
        }
        else
        {
            //use the first touch registered
            Touch touch = Input.touches[0];
            swiped = controlref.HandleTouch(touch.fingerId, touch.position, touch.phase);
        }
    }

} //InputState

public class OrderTilesState : FSMState
{
    GameMasterFSM controlref;
    bool hasExecuted;

    public OrderTilesState(GameMasterFSM control)
    {
        stateID = StateID.OrderTiles;
        controlref = control;
        hasExecuted = false;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (hasExecuted && controlref.doneSliding())
        {
            hasExecuted = false;
            npc.GetComponent<GameMasterFSM>().SetTransition(Transition.TilesDone); //to Look
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        if (!hasExecuted) { controlref.moveGrid(controlref.Column, controlref.Row, controlref.Direction); }
        hasExecuted = true;
    }

	public override void DoBeforeEntering ()
	{
		foreach (GameObject child in controlref.lasers) {
			if (child.tag == "Laser") {
				child.GetComponent<LaserScript> ().setEye (false);
			}
		}
	}

	public override void DoBeforeLeaving ()
	{
		foreach (GameObject child in controlref.lasers) {
			if (child.tag == "Laser") {
				child.GetComponent<LaserScript> ().setEye (true);
			}
		}
	}

} // OrderTilesState

public class OrderActorsState : FSMState
{
    GameMasterFSM controlref;
    bool hasExecuted;

    public OrderActorsState(GameMasterFSM control)
    {
        stateID = StateID.OrderActors;
        controlref = control;
    }

    public override void DoBeforeEntering()
    {
        foreach (GameObject actor in controlref.actors)
        {
            actor.GetComponent<ActorFSM>().doneSlide = true;
        }
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (controlref.characterDied())
        {
            npc.GetComponent<GameMasterFSM>().SetTransition(Transition.ActorDied); //to leveldeath
        }
        else if (controlref.doneWalking())
        {
            //Debug.Log("fsm actor reason");
            if (controlref.characters.Count == 0)
            {
                npc.GetComponent<GameMasterFSM>().SetTransition(Transition.LevelDone); //to levelwin
            }
            else
            {
                npc.GetComponent<GameMasterFSM>().SetTransition(Transition.ActorsDone); //to idle
            }
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
    }

} // OrderActorsState

public class LevelWonState : FSMState
{
    GameMasterFSM controlref;

    public LevelWonState(GameMasterFSM control)
    {
        stateID = StateID.LevelWon;
        controlref = control;
    }

    public override void DoBeforeEntering()
    {
        controlref.levelWin();
    }

    public override void Reason(GameObject gm, GameObject npc)
    {

    }

    public override void Act(GameObject gm, GameObject npc)
    {

    }

} // LevelWonState

public class LevelDeathState : FSMState
{
    GameMasterFSM controlref;

    public LevelDeathState(GameMasterFSM control)
    {
        stateID = StateID.LevelDeath;
        controlref = control;
    }

    public override void DoBeforeEntering()
    {
        controlref.levelDeath();
    }

    public override void Reason(GameObject gm, GameObject npc)
    {

    }

    public override void Act(GameObject gm, GameObject npc)
    {

    }

} // LevelDeathState

