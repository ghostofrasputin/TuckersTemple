using System;
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
    public GameObject Emily;
    public GameObject Trap;
    public GameObject Enemy;
    public GameObject Wraith;
    public GameObject Goal;
    public GameObject Laser;
    public GameObject Item;
    public FSMSystem fsm;
    public Vector2 lastPos = new Vector2(0, 0); //holds the last position for mouse input to calculate deltaPosition
    public int numRows; //number of tiles to size
    public int numCols;
    public int Column;
    public int Row;
    public int Direction;
    public Canvas winScreen;
    public GameObject deathScreen;
    public GameObject loadingScreen;
    public List<GameObject> actors;
    public List<GameObject> characters;
    public List<GameObject> enemies;
    public int currentLevel = 1; // progress this every time there's a win
    public List<Level> levelsList;
    public float tileSize;
    public double time = 0;
    public bool ticking = true;
    public int attempts = 1;
    public GameObject boundary;
    public List<GameObject> playerChars = new List<GameObject>();
    public Text deathText;
	public GameObject RootTile;
	public float gridScale = 0.25f;
	public GameObject TutorialButton;
	public bool foundItem = false;
	public List<GameObject> lasers;
    public GameObject UIBorder;
    public bool isPaused;
    public bool foundTile = false;

    // touch handle
    public bool latch = false;
    public bool isVert = false;
    public Vector2 offset;
    public bool swiped;
    public bool incompleteTouch = false;
    const int N = 0;
    const int E = 1;
    const int S = 2;
    const int W = 3;
    public GameObject wrapTile;
    public GameObject wrapCopy1;
    public GameObject wrapCopy2;
    public bool wrapLatch;
    public float scalar;
    
    // audio:
    public AudioClip TileSlide1;
    public AudioClip TileSlide2;
    public AudioClip nextLevelSound;
    public AudioClip playerdeathSound;
	public AudioClip gameOverSound;

    // private:
    private RaycastHit hit;
    private GameObject touchTarget;
    private Vector2 touchStart;
    private int moves = 0;
    private GameObject[][] tileGrid;
    private List<GameObject> tiles = new List<GameObject>();
	private List<int> cutscenes; //holds which levels have cutscenes

    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    public void Start()
    {
        MakeFSM();
		wrapLatch = false;
        isPaused = false;
		tileSize = Tile.GetComponent<SpriteRenderer>().bounds.size.x * gridScale;
		RootTile = GameObject.Find ("Tiles").gameObject;
		RootTile.transform.localScale = new Vector3(gridScale, gridScale, 1f);

		//intitialize cutscenes
		cutscenes = new List<int>();
        cutscenes.Add (15);
        cutscenes.Add(17);

        boundary = Instantiate(outerWall, Vector3.zero, Quaternion.identity);
	    scalar = .006f;
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
	
        LevelJuiceState juice = new LevelJuiceState(this);
        juice.AddTransition(Transition.DoneJuicing, StateID.Ready);
	
        InputState ready = new InputState(this);
        ready.AddTransition(Transition.InputReceived, StateID.OrderTiles);
        ready.AddTransition (Transition.RestartedLevel, StateID.InitLevel);
		ready.AddTransition (Transition.ActorDiedInInput, StateID.LevelDeath);

        OrderTilesState tile = new OrderTilesState(this);
        tile.AddTransition(Transition.TilesDone, StateID.OrderActors);
        tile.AddTransition (Transition.Incomplete, StateID.Ready);

        OrderActorsState actor = new OrderActorsState(this);
        actor.AddTransition(Transition.ActorsDone, StateID.Ready);
        actor.AddTransition(Transition.LevelDone, StateID.LevelWon);
        actor.AddTransition(Transition.ActorDied, StateID.LevelDeath);

        LevelWonState win = new LevelWonState(this);
        win.AddTransition(Transition.NextLevel, StateID.InitLevel);

        LevelDeathState death = new LevelDeathState(this);
        death.AddTransition(Transition.RestartedLevelFromDeath, StateID.InitLevel);

        fsm = new FSMSystem();
        fsm.AddState(init);
        fsm.AddState(ready);
        fsm.AddState(tile);
        fsm.AddState(actor);
        fsm.AddState(win);
        fsm.AddState(death);
        fsm.AddState(juice);
    }

    public GameObject spawnActor(GameObject actor, int x, int y, int dir)
    {
        GameObject newActor = Instantiate(actor, new Vector3(tileGrid[x][y].transform.position.x, tileGrid[x][y].transform.position.y, tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform);
        newActor.GetComponent<ActorFSM>().setDirection(dir);
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
		SoundController.instance.gameOver.Stop ();
        deathScreen.GetComponent<CanvasGroup>().interactable = false;
        deathScreen.GetComponent<CanvasGroup>().blocksRaycasts = false;

        attempts++;
        setupLevel(levelsList[currentLevel - 1]);
		if (fsm.CurrentStateID == StateID.LevelDeath) {
			GetComponent<GameMasterFSM> ().SetTransition (Transition.RestartedLevelFromDeath); //to ready
		} else if (fsm.CurrentStateID == StateID.Ready) {
			GetComponent<GameMasterFSM> ().SetTransition (Transition.RestartedLevel);
		} else if(fsm.CurrentStateID == StateID.LevelWon) {
            GetComponent<GameMasterFSM>().SetTransition(Transition.NextLevel);
        }
    }


    //Called when the level is won
    //Displays win screen
    public void levelWin()
    {
		ZombiePasser zombie = GameObject.FindGameObjectWithTag ("Zombie").GetComponent<ZombiePasser> ();
        try
        {
            // unlocks the next level. 
            //note: currentlevel-1 is the real current level for the array, currentlevel is the next level
			zombie.setLockedLevelBool(currentLevel);
        }
        catch (System.Exception error)
        {
            Debug.Log(error);
        }

		//Checking stars for ZombiePasser - Justin
		zombie.setStars(currentLevel - 1, 1);
		GameObject.Find("Star1").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/GoldStar");
		//set the second star
		if (moves < 4)
		{
			zombie.setStars(currentLevel - 1, zombie.getStars(currentLevel - 1) + 1);
			GameObject.Find("Star2").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/GoldStar");
		}
		//set the third star
		//if (foundItem)
		if(true) //for now we're just giving the star
		{
			zombie.setStars(currentLevel - 1, zombie.getStars(currentLevel-1)+1);
			GameObject.Find("Star3").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/GoldStar");
		}
		print("Num of Moves : " + moves);
		print("foundItem: " + foundItem);
		print(zombie.getStars(currentLevel - 1));


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
		//SoundController.instance.musicSource.Stop ();
        //SoundController.instance.PlaySingle(playerdeathSound);
        turnOffTileColliders();
		deathScreen.GetComponent<Animator>().Play("DeathFadeIn");
        deathScreen.GetComponent<CanvasGroup>().interactable = true;
        deathScreen.GetComponent<CanvasGroup>().blocksRaycasts = true;
        //SoundController.instance.PlaySingleGameOver (gameOverSound);
    }

    public void nextLevel()
    {
        currentLevel++;

		//check if there is a cutscene before next level
		if (cutscenes.Contains(currentLevel)) {
			loadingScreen.SetActive (true);
			GameObject.Find("ZombiePasser").GetComponent<ZombiePasser>().setLevel(currentLevel);
			GameObject.Find("pauseScreen").GetComponent<InGameMenuManager>().loadScene ("cutScene");
		}

		//check if last level
		if (currentLevel > levelsList.Count) {
			Debug.Log ("End of Demo Screen");
			GameObject.Find("endOfDemo").GetComponent<InGameMenuManager>().playAnim("winEnter");
			GameObject.FindGameObjectWithTag ("Zombie").GetComponent<ZombiePasser> ().setLevel (1);
			return;
		}

        reset();
        ticking = true;
        SoundController.instance.PlaySingle(nextLevelSound);

		//reset already sets a transition, so this is throwing an error.  Not sure why it was here? -Andrew
        //GetComponent<GameMasterFSM>().SetTransition(Transition.NextLevel); //to ready
		TutorialButton.SetActive(false);
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
		lasers.Clear ();
        tileGrid = new GameObject[numCols][];
        generateLevel(level);
        //Destroy(boundary.gameObject);
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

		//zoom the camera and scale the background
		GameObject mainCamera = GameObject.Find("Main Camera");
		GameObject UIBorder = GameObject.Find ("UIBorderPause");
		//Debug.Log ("UI Border: " + UIBorder.transform.position + " " + UIBorder.transform.localScale);
		if (numCols == 4) {
			mainCamera.transform.localScale = new Vector3 (1.31f, 1.333f, 1);
			mainCamera.transform.position = new Vector3 (2.25f, 1.6f, -10);
			mainCamera.GetComponent<Camera> ().orthographicSize = 7;
			// scale UI border to work with new camera paramters
			UIBorder.transform.localScale = new Vector3 (.596367f,.643526f,.532441f);
			UIBorder.transform.position = new Vector3 (2.26f,1.63f,0.0f);
		}
		if (numCols == 3) {
			mainCamera.transform.localScale = new Vector3 (1f, 1f, 1);
			mainCamera.transform.position = new Vector3 (1.5f, 1f, -10);
			mainCamera.GetComponent<Camera> ().orthographicSize = 5;
			// scale UI border to work with new camera paramters
			UIBorder.transform.localScale = new Vector3 (.4209864f,.4542772f,.3758603f);
			UIBorder.transform.position = new Vector3 (1.485596f,.9803f,0.0f);
		}


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
                tileGrid[c][r] = Instantiate(Tile, new Vector3(c * tileSize, r * tileSize, 0), Quaternion.identity, RootTile.transform);
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
			if (key.Contains("roy"))
            {
                GameObject roy = spawnActor(Character, value[0], value[1], value[2]);
                actors.Add(roy);
                characters.Add(roy);
            }
			if (key.Contains("emily"))
            {
                GameObject emily = spawnActor(Emily, value[0], value[1], value[2]);
                actors.Add(emily);
                characters.Add(emily);
            }
			if (key.Contains("jake"))
            {
                GameObject jake = spawnActor(Character, value[0], value[1], value[2]);
                actors.Add(jake);
                characters.Add(jake);
            }
			if (key.Contains("tank"))
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
            if (key.Contains("wraith"))
            {
                GameObject wraith = spawnActor(Wraith, value[0], value[1], value[2]);
                actors.Add(wraith);
                enemies.Add(wraith);
            }
        }

        // Create the Static Objects (aka goal & traps):
        foreach (KeyValuePair<string, List<int>> kvp in staticObjectInfo)
        {
            string key = kvp.Key;
            List<int> value = kvp.Value;
			if (key.Contains("goal"))
            {
                int x = value[0];
                int y = value[1];
                Instantiate(Goal, new Vector3(tileGrid[x][y].transform.position.x, tileGrid[x][y].transform.position.y,
                    tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform);
            }
			if (key.Contains("trap"))
            {
                int x = value[0];
                int y = value[1];
                Instantiate(Trap, new Vector3(tileGrid[x][y].transform.position.x, tileGrid[x][y].transform.position.y,
                    tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform);
            }
			if (key.Contains ("laser")) {
				int x = value [0];
				int y = value [1];
				float offset = tileSize / 3;
				GameObject las = Instantiate(Laser, new Vector3(tileGrid[x][y].transform.position.x, tileGrid[x][y].transform.position.y,
					tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform);
				lasers.Add (las);
				las.GetComponent<LaserScript> ().setDir (value [2], offset);
			}
			if (key.Contains ("item")) {
				int x = value [0];
				int y = value [1];
				GameObject item = Instantiate(Item, new Vector3(tileGrid[x][y].transform.position.x, tileGrid[x][y].transform.position.y, 
					tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform);
			}
        }
        //Add in outer walls to the grid
        boundary.transform.localScale = new Vector3((numCols * 4/3) * tileSize, (numRows * 4/3) * tileSize, 1);
        boundary.transform.position = new Vector3((numCols * 4/3) * tileSize / 4, (numRows * 4/3) * tileSize / 4, 0);
        //Debug.Log(boundary.transform.localScale.ToString());
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

    public void flipPause()
    {
        isPaused = !isPaused;
        UIBorder.GetComponent<Animator>().speed = 1.5f;
    }

    public bool HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase, Vector3 touchDelta = default(Vector3))
    {
        bool touchSuccess = false;
		int row = 0;
		int col = 0;
        Debug.Log(foundTile);
        switch (touchPhase)
        {
            case TouchPhase.Began:
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                touchTarget = null;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile")))
                {
                    touchTarget = hit.collider.gameObject;
                    //Debug.Log(touchTarget);
                    for (int c = 0; c < numCols; c++)
                    {
                        for (int r = 0; r < numRows; r++)
                        {
                            if (tileGrid[c][r].Equals(touchTarget))
                            {
                                row = r;
                                Row = row;
                                col = c;
                                Column = col;
                                foundTile = true;
                                touchStart = new Vector2(touchPosition.x, touchPosition.y);
                                break;
                            }
                        }
                        if (foundTile) { break; }
                    }
                }
                Debug.Log(Row + ", " + Column);
                break;

            case TouchPhase.Stationary:

                if (foundTile) { spinGear(0); }
                break;

			case TouchPhase.Moved:

                if (foundTile)
                {
                    spinGear(.5f);
                    Vector2 offsetLocal = (Vector2)touchPosition - touchStart;
                    if (Math.Abs(offsetLocal.x) > 10 || Math.Abs(offsetLocal.y) > 10)
                    {
                        if (latch == false)
                        {
                            offset = (Vector2)touchPosition - touchStart;
                            isVert = Mathf.Abs(offset.y) > Mathf.Abs(offset.x);
                            latch = true;
                        }

                        // move the row or column 
                        // with user touch
                        float tileS = touchTarget.GetComponent<SpriteRenderer>().bounds.size.x * gridScale;
                        /* Debug.Log("spriterenderer" + tileS); */
                        Vector3 origScale;
                        if (isVert)
                        {
                            if (!wrapLatch)
                            {
                                wrapTile = tileGrid[Column][numRows - 1];
                                origScale = wrapTile.transform.localScale;
                                wrapTile.transform.localScale = Vector3.one;
                                wrapCopy1 = Instantiate(wrapTile, new Vector3(wrapTile.transform.position.x, -tileSize, 0), Quaternion.identity, wrapTile.transform);
                                Destroy(wrapCopy1.GetComponent<TileFSM>());
                                wrapTile.transform.localScale = origScale;

                                wrapTile = tileGrid[Column][0];
                                origScale = wrapTile.transform.localScale;
                                wrapTile.transform.localScale = Vector3.one;
                                wrapCopy2 = Instantiate(wrapTile, new Vector3(wrapTile.transform.position.x, tileSize * numRows, 0), Quaternion.identity, wrapTile.transform);
                                Destroy(wrapCopy2.GetComponent<TileFSM>());
                                wrapTile.transform.localScale = origScale;

                                wrapLatch = true;
                            }
                        }
                        else
                        {
                            if (!wrapLatch)
                            {
                                wrapTile = tileGrid[numCols - 1][Row];
                                origScale = wrapTile.transform.localScale;
                                wrapTile.transform.localScale = Vector3.one;
                                wrapCopy1 = Instantiate(wrapTile, new Vector3(-tileSize, wrapTile.transform.position.y, 0), Quaternion.identity, wrapTile.transform);
                                Destroy(wrapCopy1.GetComponent<TileFSM>());
                                wrapTile.transform.localScale = origScale;

                                wrapTile = tileGrid[0][Row];
                                origScale = wrapTile.transform.localScale;
                                wrapTile.transform.localScale = Vector3.one;
                                wrapCopy2 = Instantiate(wrapTile, new Vector3(tileSize * numCols, wrapTile.transform.position.y, 0), Quaternion.identity, wrapTile.transform);
                                Destroy(wrapCopy2.GetComponent<TileFSM>());
                                wrapTile.transform.localScale = origScale;

                                wrapLatch = true;
                            }
                        }
                        // moving horizontal rows:
                        if (!isVert)
                        {
                            for (int c = 0; c < numCols; c++)
                            {
                                tileGrid[c][Row].GetComponent<TileFSM>().goalPos = new Vector2(tileGrid[c][Row].transform.position.x + touchDelta.x * scalar,
                                                                                        tileGrid[c][Row].transform.position.y);
                                //Debug.Log (tileGrid [c] [row].GetComponent<TileFSM> ().goalPos);
                            }
                        }
                        // moving vertical cols:
                        else
                        {
                            for (int r = 0; r < numRows; r++)
                            {
                                tileGrid[Column][r].GetComponent<TileFSM>().goalPos = new Vector2(tileGrid[Column][r].transform.position.x,
                                                                                        tileGrid[Column][r].transform.position.y + touchDelta.y * scalar);
                            }
                        }
                    }
                }
				break;
			case TouchPhase.Ended:

                if (foundTile)
                {
                    float swipeDist;
                    //float tileSize = tileGrid [0] [0].GetComponent<Renderer> ().bounds.size.x * gridScale;
                    bool validSwipe;
                    Destroy(wrapCopy1, 0.5f);
                    Destroy(wrapCopy2, 0.5f);
                    wrapLatch = false;

                    //Debug.Log (isVert);
                    if (isVert)
                    {
                        swipeDist = (touchPosition.y - touchStart.y) * scalar;
                        if (Mathf.Abs(swipeDist) < tileSize / 2)
                        {
                            validSwipe = false;
                            incompleteTouch = true;
                        }
                        else
                        {
                            validSwipe = true;
                            incompleteTouch = false;
                        }
                        for (int r = 0; r < numRows; r++)
                        {
                            //Debug.Log(swipeDist);
                            if (!validSwipe)
                            {
                                tileGrid[Column][r].GetComponent<TileFSM>().incompleteMove = true;
                            }
                            tileGrid[Column][r].GetComponent<TileFSM>().touchReleased = true;
                        }
                        if (swipeDist < 0)
                        {
                            Direction = S;
                        }
                        else
                        {
                            Direction = N;
                        }
                    }
                    else
                    {
                        swipeDist = (touchPosition.x - touchStart.x) * scalar;
                        if (Mathf.Abs(swipeDist) < tileSize / 2)
                        {
                            validSwipe = false;
                            incompleteTouch = true;
                        }
                        else
                        {
                            validSwipe = true;
                            incompleteTouch = false;
                        }
                        for (int c = 0; c < numCols; c++)
                        {
                            if (!validSwipe)
                            {
                                tileGrid[c][Row].GetComponent<TileFSM>().incompleteMove = true;
                            }
                            tileGrid[c][Row].GetComponent<TileFSM>().touchReleased = true;
                        }
                        if (swipeDist < 0)
                        {
                            Direction = W;
                        }
                        else
                        {
                            Direction = E;
                        }
                    }
                    latch = false;
                    touchSuccess = true;
                    foundTile = false;
                }
	                break;

	            default:
	                break;
        }
        return touchSuccess;
    }

    public void spinGear(float s)
    {
        if (s == 0)
        {
            UIBorder.GetComponent<Animator>().enabled = false;
        }
        else
        {
            UIBorder.GetComponent<Animator>().enabled = true;
            UIBorder.GetComponent<Animator>().speed = s;
            UIBorder.GetComponent<Animator>().Play("UIBorderGear");
        }
    }

    public void moveGrid(int col, int row, int dir)
    {
        //float tileSize = tileGrid[0][0].GetComponent<Renderer>().bounds.size.x * gridScale;
        //SoundController.instance.RandomSfx(TileSlide1, TileSlide2);
        //calculate normal offset vector and move the tiles
        Vector2 offset = new Vector2(0, 0);
        GameObject temp;
	if (!incompleteTouch) {
		switch (dir) {
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
				temp = tileGrid [col] [numRows - 1];
				for (int r = numRows - 1; r > 0; r--) {
					tileGrid [col] [r] = tileGrid [col] [r - 1];
					tileGrid [col] [r].GetComponent<TileFSM> ().goalPos = new Vector2 (offset.x + tileGrid [col] [r].GetComponent<TileFSM> ().startPos.x, offset.y + tileGrid [col] [r].GetComponent<TileFSM> ().startPos.y);
				}
				tileGrid [col] [0] = temp;
				tileGrid [col] [0].GetComponent<TileFSM> ().offGrid = true;
				tileGrid [col] [0].GetComponent<TileFSM> ().goalPos = new Vector2 (offset.x + tileGrid [col] [0].GetComponent<TileFSM> ().startPos.x, offset.y + tileGrid [col] [0].GetComponent<TileFSM> ().startPos.y);
				tileGrid [col] [0].GetComponent<TileFSM> ().wrapPos = new Vector2 (tileSize * col, -tileSize);
				tileGrid [col] [0].GetComponent<TileFSM> ().wrapGoalPos = new Vector2 (tileSize * col, 0);

				break;
			case S:
				offset.y = -tileSize;
				temp = tileGrid [col] [0];
				for (int r = 0; r < numRows - 1; r++) {
					tileGrid [col] [r] = tileGrid [col] [r + 1];
					tileGrid [col] [r].GetComponent<TileFSM> ().goalPos = new Vector2 (offset.x + tileGrid [col] [r].GetComponent<TileFSM> ().startPos.x, offset.y + tileGrid [col] [r].GetComponent<TileFSM> ().startPos.y);
				}
				tileGrid [col] [numRows - 1] = temp;
				tileGrid [col] [numRows - 1].GetComponent<TileFSM> ().offGrid = true;
				tileGrid [col] [numRows - 1].GetComponent<TileFSM> ().goalPos = new Vector2 (offset.x + tileGrid [col] [numRows - 1].GetComponent<TileFSM> ().startPos.x, offset.y + tileGrid [col] [numRows - 1].GetComponent<TileFSM> ().startPos.y);
				tileGrid [col] [numRows - 1].GetComponent<TileFSM> ().wrapPos = new Vector2 (tileSize * col, numRows * tileSize);
				tileGrid [col] [numRows - 1].GetComponent<TileFSM> ().wrapGoalPos = new Vector2 (tileSize * col, (numRows - 1) * tileSize);
				break;
			case E:
				offset.x = tileSize;
				temp = tileGrid [numCols - 1] [row];
				for (int c = numCols - 1; c > 0; c--) {
					tileGrid [c] [row] = tileGrid [c - 1] [row];
					tileGrid [c] [row].GetComponent<TileFSM> ().goalPos = new Vector2 (offset.x + tileGrid [c] [row].GetComponent<TileFSM> ().startPos.x, offset.y + tileGrid [c] [row].GetComponent<TileFSM> ().startPos.y);
				}
				tileGrid [0] [row] = temp;
				tileGrid [0] [row].GetComponent<TileFSM> ().offGrid = true;
				tileGrid [0] [row].GetComponent<TileFSM> ().goalPos = new Vector2 (offset.x + tileGrid [0] [row].GetComponent<TileFSM> ().startPos.x, offset.y + tileGrid [0] [row].GetComponent<TileFSM> ().startPos.y);
				tileGrid [0] [row].GetComponent<TileFSM> ().wrapPos = new Vector2 (-tileSize, tileSize * row);
				tileGrid [0] [row].GetComponent<TileFSM> ().wrapGoalPos = new Vector2 (0, tileSize * row);
				break;
			case W:
				offset.x = -tileSize;
				temp = tileGrid [0] [row];
				for (int c = 0; c < numCols - 1; c++) {
					tileGrid [c] [row] = tileGrid [c + 1] [row];
					tileGrid [c] [row].GetComponent<TileFSM> ().goalPos = new Vector2 (offset.x + tileGrid [c] [row].GetComponent<TileFSM> ().startPos.x, offset.y + tileGrid [c] [row].GetComponent<TileFSM> ().startPos.y);
				}
				tileGrid [numCols - 1] [row] = temp;
				tileGrid [numCols - 1] [row].GetComponent<TileFSM> ().offGrid = true;
				tileGrid [numCols - 1] [row].GetComponent<TileFSM> ().goalPos = new Vector2 (offset.x + tileGrid [numCols - 1] [row].GetComponent<TileFSM>().startPos.x, offset.y + tileGrid [numCols - 1] [row].GetComponent<TileFSM> ().startPos.y);
				tileGrid [numCols - 1] [row].GetComponent<TileFSM> ().wrapPos = new Vector2 (numCols * tileSize, row * tileSize);
				tileGrid [numCols - 1] [row].GetComponent<TileFSM> ().wrapGoalPos = new Vector2 ((numCols - 1) * tileSize, row * tileSize);
				break;
			}
		}
	moves++;
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
		StateID currState = character.GetComponent<ActorFSM> ().fsm.CurrentStateID;
		if (currState == StateID.EnemyDeadA || currState == StateID.TrapDeadA || currState == StateID.LaserDeadA)            {
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
			
		GameObject.Find("Star1").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/BlackStar");
		GameObject.Find("Star2").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/BlackStar");
		GameObject.Find("Star3").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/BlackStar");
		GameObject.Find("GameMaster").GetComponent<GameMasterFSM>().foundItem = false;
	}

} // LevelJuiceState

public class InputState : FSMState
{
    public GameMasterFSM controlref;

    public InputState(GameMasterFSM control)
    {
        stateID = StateID.Ready;
        controlref = control;
        controlref.swiped = false;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
		if (controlref.characterDied())
		{
			npc.GetComponent<GameMasterFSM>().SetTransition(Transition.ActorDiedInInput); //to leveldeath
		}
        if (controlref.swiped)
        {
            npc.GetComponent<GameMasterFSM>().SetTransition(Transition.InputReceived); //to Look
        }
		if (controlref.doneSliding()) {
			foreach (GameObject child in controlref.lasers) {
				if (child.tag == "Laser") {
					if (!child.GetComponent<LaserScript> ().eyeOpen) {
						child.GetComponent<LaserScript> ().setEye (true);
					}
				}
			}
        } else {
			foreach (GameObject child in controlref.lasers) {
				if (child.tag == "Laser") {
					if (child.GetComponent<LaserScript> ().eyeOpen) {
						child.GetComponent<LaserScript> ().setEye (false);
					}
				}
			}
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        controlref.swiped = false;

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
                if (controlref.lastPos == (Vector2)Input.mousePosition)
                {
                    controlref.HandleTouch(10, Input.mousePosition, TouchPhase.Stationary, Input.mousePosition - (Vector3)controlref.lastPos);
                }
                else
                {
                    controlref.HandleTouch(10, Input.mousePosition, TouchPhase.Moved, Input.mousePosition - (Vector3)controlref.lastPos);
                    controlref.lastPos = Input.mousePosition;
                }
            }
            //called when mouse is lifted up(ended)
            if (Input.GetMouseButtonUp(0))
            {
                controlref.swiped = controlref.HandleTouch(10, Input.mousePosition, TouchPhase.Ended);
            }
        }
        else
        {
            //use the first touch registered
            Touch touch = Input.touches[0];
            controlref.swiped = controlref.HandleTouch(touch.fingerId, touch.position, touch.phase, touch.deltaPosition);
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
            if (controlref.incompleteTouch) {
                npc.GetComponent<GameMasterFSM>().SetTransition(Transition.Incomplete); //to Input
            } else {
                npc.GetComponent<GameMasterFSM>().SetTransition(Transition.TilesDone); //to orderactors
            }
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
        controlref.spinGear(0.5f);
    }

    public override void DoBeforeLeaving()
    {
        SoundController.instance.RandomSfxTiles(controlref.TileSlide1, controlref.TileSlide2);
        controlref.spinGear(0);
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
		foreach (GameObject child in controlref.lasers) {
			if (child.tag == "Laser") {
				child.GetComponent<LaserScript> ().setEye (false);
			}
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
		
	public override void DoBeforeLeaving ()
	{
		foreach (GameObject child in controlref.lasers) {
			if (child.tag == "Laser") {
				child.GetComponent<LaserScript> ().setEye (true);
			}
		}
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

    public override void Reason(GameObject gm, GameObject npc)
    {

    }

    public override void Act(GameObject gm, GameObject npc)
    {
        controlref.levelDeath();
    }

} // LevelDeathState

