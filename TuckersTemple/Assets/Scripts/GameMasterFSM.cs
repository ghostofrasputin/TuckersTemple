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
	public GameObject TileDark;
    public GameObject Character;
    public GameObject Emily;
    public GameObject Jake;
    public GameObject Tank;
    public GameObject Trap;
    public GameObject Enemy;
    public GameObject Wraith;
    public GameObject Goal;
	public GameObject GoalDark;
    public GameObject Laser;
    public GameObject Item;
	public GameObject ItemDark;
    public GameObject shadowPS;
    public FSMSystem fsm;
    public Vector2 lastPos; //holds the last position for mouse input to calculate deltaPosition
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
    public GameObject PauseButton;
    public Dictionary<string, bool> starRequirements;
    public string[] starCriteria = { "foundItem", "killAll", "killNone" };
    public List<GameObject> lasers;
    public GameObject tutorial;

    public GameObject UIBorder;
    public bool isPaused;
    public bool foundTile;

    public GameObject zombie;

    // touch handle
    public bool latch;
    public bool isVert;
    public Vector2 offset;
    public bool swiped;
    public bool moved;
    public bool incompleteTouch;
    const int N = 0;
    const int E = 1;
    const int S = 2;
    const int W = 3;
    public GameObject wrapTile;
    public GameObject wrapCopy1;
    public GameObject wrapCopy2;
    public bool wrapLatch;
    public float scalar;

    public int maxDepth;
    public int currDepth;
    public List<int> moveHistory = new List<int>();

    // audio:
    public AudioClip TileSlide1;
    public AudioClip TileSlide2;
    public AudioClip nextLevelSound;
    public AudioClip playerdeathSound;
    public AudioClip gameOverSound;
    public AudioClip levelWinSound;
    public AudioClip tileSlideOnly;
    public AudioClip royWin1, royWin2, royWin3;
    public AudioClip emilyWin1, emilyWin2, emilyWin3;
    public AudioClip jakeWin1, jakeWin2, jakeWin3;
    public AudioClip tankWin1; // for future use

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
        latch = false;
        isPaused = false;
        foundTile = false;
        isVert = false;
        incompleteTouch = false;
        moved = false;
        lastPos = Vector2.zero;

        maxDepth = 12;
        currDepth = 0;


        tileSize = Tile.GetComponent<SpriteRenderer>().bounds.size.x * gridScale;
        RootTile = GameObject.Find("Tiles").gameObject;
        RootTile.transform.localScale = new Vector3(gridScale, gridScale, 1f);

        //intitialize cutscenes
        cutscenes = new List<int>();
        cutscenes.Add(11);
        cutscenes.Add(21);
		cutscenes.Add(26);
        cutscenes.Add(31);
        cutscenes.Add(35);
        cutscenes.Add(37);
        cutscenes.Add(41);
        cutscenes.Add(51);

        boundary = Instantiate(outerWall, Vector3.zero, Quaternion.identity);
        scalar = .006f;
        setStarRequirements();

        zombie = GameObject.FindGameObjectWithTag("Zombie");
        GameObject.FindGameObjectWithTag("Level-Num").GetComponent<Text>().text = "" + zombie.GetComponent<ZombiePasser>().getLevel();

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
        ready.AddTransition(Transition.RestartedLevel, StateID.InitLevel);
        ready.AddTransition(Transition.ActorDiedInInput, StateID.LevelDeath);

        OrderTilesState tile = new OrderTilesState(this);
        tile.AddTransition(Transition.TilesDone, StateID.OrderActors);
        tile.AddTransition(Transition.Incomplete, StateID.Ready);

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

    public void sameTileOrder()
    {
        int orderCount = 0;
        bool charactersDone = false;
        bool enemiesDone = false;
        for (int i = 0; ; ++i)
        {
            if (i < characters.Count)
            {
                characters[i].GetComponent<ActorFSM>().doneSlide = true;
                characters[i].GetComponent<SpriteRenderer>().sortingOrder = orderCount;
            }
            else
            {
                charactersDone = true;
            }
            if (i < enemies.Count)
            {
                enemies[i].GetComponent<ActorFSM>().doneSlide = true;
                enemies[i].GetComponent<SpriteRenderer>().sortingOrder = orderCount;
            }
            else
            {
                enemiesDone = true;
            }
            if (charactersDone && enemiesDone)
            {
                break;
            }
            ++orderCount;
        }
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
        //SoundController.instance.gameOver.Stop ();
        deathScreen.GetComponent<CanvasGroup>().interactable = false;
        deathScreen.GetComponent<CanvasGroup>().blocksRaycasts = false;
        attempts++;
        setupLevel(levelsList[currentLevel - 1]);
        if (fsm.CurrentStateID == StateID.LevelDeath)
        {
            GetComponent<GameMasterFSM>().SetTransition(Transition.RestartedLevelFromDeath); //to ready
        }
        else if (fsm.CurrentStateID == StateID.Ready)
        {
            GetComponent<GameMasterFSM>().SetTransition(Transition.RestartedLevel);
        }
        else if (fsm.CurrentStateID == StateID.LevelWon)
        {
            GetComponent<GameMasterFSM>().SetTransition(Transition.NextLevel);
        }

    }

    public void restartButton()
    {
        zombie.GetComponent<SocialPlatform>().AchievementProgress(GPGSIds.achievement_mulligan, false);
        reset();
    }

    public void setWinScreenEmily()
    {
        GameObject temp3 = GameObject.Find("Star3");
        temp3.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/TT-Stars");
        temp3.GetComponent<ParticleSystem>().Play();
        GameObject.FindWithTag("emilyWin").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/endscreen_emily");
        GameObject.Find("NumMoves").GetComponent<Text>().text = "";
    }

    public void setWinScreenJake()
    {
        GameObject temp2 = GameObject.Find("Star2");
        temp2.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/TT-Stars");
        temp2.GetComponent<ParticleSystem>().Play();
        GameObject.FindWithTag("jakeWin").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/endscreen_jake");
        temp2.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void setWinScreenRoy()
    {
        GameObject temp1 = GameObject.Find("Star1");
        temp1.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/TT-Stars");
        temp1.GetComponent<ParticleSystem>().Play();
        GameObject.FindWithTag("royWin").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/endscreen_roy");
    }

    public void setWinScreenTank()
    {
        GameObject.FindWithTag("tankWin").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Jumping-Tank");
    }

    //Called when the level is won
    //Displays win screen
    public void levelWin()
    {
        resetWinScreen();
        ZombiePasser zombiePasser = zombie.GetComponent<ZombiePasser>();
			
        try
        {
            // unlocks the next level. 
            //note: currentlevel-1 is the real current level for the array, currentlevel is the next level
            zombiePasser.setLockedLevelBool(currentLevel);
        }
        catch (System.Exception error)
        {
            Debug.Log(error);
        }
      
        moves = 0;
        time = 0;
        attempts = 0;

        // save game data:
        zombiePasser.Save();
    }

    public void save()
    {
        zombie.GetComponent<ZombiePasser>().Save();
    }

    private void resetWinScreen()
    {

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
        currDepth = 0;
        moveHistory = new List<int>();
        reset();
    }

    public void nextLevel()
    {
        currentLevel++;
        GameObject.FindGameObjectWithTag("Level-Num").GetComponent<Text>().text = currentLevel.ToString();

        //check if there is a cutscene before next level
        if (cutscenes.Contains(currentLevel))
        {
            loadingScreen.SetActive(true);
            zombie.GetComponent<ZombiePasser>().setLevel(currentLevel);
            GameObject.Find("pauseScreen").GetComponent<InGameMenuManager>().loadScene("cutScene");
        }

        //check if last level
        if (currentLevel > levelsList.Count)
        {
            Debug.Log("End of Demo Screen");
            GameObject.Find("endOfDemo").GetComponent<InGameMenuManager>().playAnim("winEnter");
            zombie.GetComponent<ZombiePasser>().setLevel(1);
            return;
        }

        reset();
        ticking = true;
        SoundController.instance.PlaySingle(nextLevelSound);

        //reset already sets a transition, so this is throwing an error.  Not sure why it was here? -Andrew
        //GetComponent<GameMasterFSM>().SetTransition(Transition.NextLevel); //to ready
        if (currentLevel > 4)
        {
            TutorialButton.SetActive(false);
        }
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

    public void resetStarRequirements()
    {
        starRequirements.Clear();
        //also reset number of moves
        moves = 0;
        setStarRequirements();
    }

    public void setStarRequirements()
    {
        starRequirements = new Dictionary<string, bool>();
        foreach (string s in starCriteria)
        {
            starRequirements.Add(s, false);
        }
        //set any that start true
        if (starRequirements.ContainsKey("killNone"))
        {
            starRequirements["killNone"] = true;
        }
    }

    //called by ActorFSM in the event of an enemy death
    //It processes starRequirements
    public void enemyDied()
    {
        if (starRequirements.ContainsKey("killNone"))
        {
            starRequirements["killNone"] = false;
        }
        if (enemies.Count == 0)
        {
            if (starRequirements.ContainsKey("killAll"))
            {
                starRequirements["killAll"] = true;
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
        lasers.Clear();
        tileGrid = new GameObject[numCols][];
        resetStarRequirements();
        generateLevel(level);
        //Destroy(boundary.gameObject);
        PauseButton.SetActive(true);
        turnOnTileColliders();
    }

    // takes in the current level and creates it:
    public void generateLevel(Level currLevel)
    {
		int darkLevel = 31;
		GameObject background = GameObject.FindGameObjectWithTag ("Background");
		GameObject foreground = GameObject.FindGameObjectWithTag ("Foreground");
		if (currentLevel >= darkLevel) {
			background.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("BackgroundPurple");
			foreground.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("ForegroundPurple");
		}
		else {
			background.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("BackgroundGold");
			foreground.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("ForegroundGold");
		}
        // extract level info:
        string name = currLevel.Name;
        numRows = currLevel.Rows;
        numCols = currLevel.Cols;
        List<List<string>> tileInfo = currLevel.Tiles;
        Dictionary<string, List<int>> actorInfo = currLevel.Actors;
        Dictionary<string, List<int>> staticObjectInfo = currLevel.StaticObjects;

        //zoom the camera and scale the background
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        GameObject UIBorder = GameObject.FindWithTag("UI-Border");
        //Debug.Log ("UI Border: " + UIBorder.transform.position + " " + UIBorder.transform.localScale);
        if (numCols == 4)
        {
            mainCamera.transform.localScale = new Vector3(1.31f, 1.333f, 1);
            mainCamera.transform.position = new Vector3(2.3f, 1.6f, -10);
            mainCamera.GetComponent<Camera>().orthographicSize = 7;
            // scale UI border to work with new camera paramters
            UIBorder.transform.localScale = new Vector3(1.76075f, 1.915672f, 1f);
            UIBorder.transform.position = new Vector3(2.25f, 1.6f, 0.0f);
        }
        if (numCols == 3)
        {
            mainCamera.transform.localScale = new Vector3(1f, 1f, 1);
            mainCamera.transform.position = new Vector3(1.55f, 1f, -10);
            mainCamera.GetComponent<Camera>().orthographicSize = 5;
            // scale UI border to work with new camera paramters
            UIBorder.transform.localScale = new Vector3(1.26393f, 1.371209f, 1f);
            UIBorder.transform.position = new Vector3(1.5f, .995f, 0.0f);
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
				if ( currentLevel >= darkLevel) {
					tileGrid [c] [r] = Instantiate (TileDark, new Vector3 (c * tileSize, r * tileSize, 0), Quaternion.identity, RootTile.transform);
				} else {
					tileGrid [c] [r] = Instantiate (Tile, new Vector3 (c * tileSize, r * tileSize, 0), Quaternion.identity, RootTile.transform);
				}
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
                GameObject jake = spawnActor(Jake, value[0], value[1], value[2]);
                actors.Add(jake);
                characters.Add(jake);
            }
            if (key.Contains("tank"))
            {
                GameObject tank = spawnActor(Tank, value[0], value[1], value[2]);
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
				if (currentLevel >= darkLevel) {
					Instantiate (GoalDark, new Vector3 (tileGrid [x] [y].transform.position.x, tileGrid [x] [y].transform.position.y,
						tileGrid [x] [y].transform.position.z), Quaternion.identity, tileGrid [x] [y].transform);
					} else {
					Instantiate (Goal, new Vector3 (tileGrid [x] [y].transform.position.x, tileGrid [x] [y].transform.position.y,
						tileGrid [x] [y].transform.position.z), Quaternion.identity, tileGrid [x] [y].transform);
				}
            }
            if (key.Contains("trap"))
            {
                int x = value[0];
                int y = value[1];
                Instantiate(Trap, new Vector3(tileGrid[x][y].transform.position.x, tileGrid[x][y].transform.position.y,
                    tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform);
            }
            if (key.Contains("laser"))
            {
                int x = value[0];
                int y = value[1];
                float offset = tileSize / 3;
                GameObject las = Instantiate(Laser, new Vector3(tileGrid[x][y].transform.position.x, tileGrid[x][y].transform.position.y,
                    tileGrid[x][y].transform.position.z), Quaternion.identity, tileGrid[x][y].transform);
                lasers.Add(las);
                las.GetComponent<LaserScript>().setDir(value[2], offset);
            }
            if (key.Contains("item"))
            {
                int x = value[0];
                int y = value[1];
				if (currentLevel >= darkLevel) {
					Instantiate (ItemDark, new Vector3 (tileGrid [x] [y].transform.position.x, tileGrid [x] [y].transform.position.y,
						tileGrid [x] [y].transform.position.z), Quaternion.identity, tileGrid [x] [y].transform);
					} else {
					Instantiate (Item, new Vector3 (tileGrid [x] [y].transform.position.x, tileGrid [x] [y].transform.position.y,
						tileGrid [x] [y].transform.position.z), Quaternion.identity, tileGrid [x] [y].transform);
				}
            }
        }
        //Add in outer walls to the grid
        boundary.transform.localScale = new Vector3((numCols * 4 / 3) * tileSize, (numRows * 4 / 3) * tileSize, 1);
        boundary.transform.position = new Vector3((numCols * 4 / 3) * tileSize / 4, (numRows * 4 / 3) * tileSize / 4, 0);
		if (numCols == 4) {
			boundary.transform.localScale = new Vector3(8.25f, 8.25f, 1);
			boundary.transform.position = new Vector3(2.25f, 2.25f, 0);
		}
        //Debug.Log(boundary.transform.localScale.ToString());
    }

    //called to skip animations
    public void skipAnimation()
    {
        //Level Juicing
        foreach (GameObject[] a in tileGrid)
        {
            foreach (GameObject t in a)
            {
                t.transform.position = t.GetComponent<TileFSM>().goalPos;
            }
        }
    }

    public void flipPause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            turnOffTileColliders();
        }
        else
        {
            turnOnTileColliders();
        }
        UIBorder.GetComponent<Animator>().speed = 1.5f;
    }

    public bool HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase, Vector3 touchDelta = default(Vector3))
    {
        foundTile = true;
        bool touchSuccess = false;


        Row = 0;
        Column = 0;
        isVert = true;
        bool flip = true;

        int move = Mathf.FloorToInt(UnityEngine.Random.Range(0, 11));
        switch (move)
        {
            case 0:
                break;

            case 1:
                Column = 1;
                break;

            case 2:
                Column = 2;
                break;

            case 3:
                flip = false;
                break;

            case 4:
                Column = 1;
                flip = false;
                break;

            case 5:
                Column = 2;
                flip = false;
                break;

            case 6:
                isVert = false;
                break;

            case 7:
                Row = 1;
                isVert = false;
                break;

            case 8:
                Row = 2;
                isVert = false;
                break;

            case 9:
                flip = false;
                isVert = false;
                break;

            case 10:
                Row = 1;
                flip = false;
                isVert = false;
                break;

            case 11:
                Row = 2;
                flip = false;
                isVert = false;
                break;

            default:
                Debug.Log("LOGIC ERROR");
                break;
        }

        incompleteTouch = false;

        if (isVert)
        {
            for (int r = 0; r < numRows; r++)
            {
                tileGrid[Column][r].GetComponent<TileFSM>().touchReleased = true;
            }

            Direction = flip ? S : N;
        }
        else
        {
            for (int c = 0; c < numCols; c++)
            {

                tileGrid[c][Row].GetComponent<TileFSM>().touchReleased = true;
            }

            Direction = flip ? W : E;

        }

        latch = false;
        touchSuccess = true;
        foundTile = false;

        ++currDepth;
        moveHistory.Add(move);

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
        if (!incompleteTouch)
        {
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
                        tileGrid[col][r].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[col][r].GetComponent<TileFSM>().startPos.x, offset.y + tileGrid[col][r].GetComponent<TileFSM>().startPos.y);
                    }
                    tileGrid[col][0] = temp;
                    tileGrid[col][0].GetComponent<TileFSM>().offGrid = true;
                    tileGrid[col][0].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[col][0].GetComponent<TileFSM>().startPos.x, offset.y + tileGrid[col][0].GetComponent<TileFSM>().startPos.y);
                    tileGrid[col][0].GetComponent<TileFSM>().wrapPos = new Vector2(tileSize * col, -tileSize);
                    tileGrid[col][0].GetComponent<TileFSM>().wrapGoalPos = new Vector2(tileSize * col, 0);

                    break;
                case S:
                    offset.y = -tileSize;
                    temp = tileGrid[col][0];
                    for (int r = 0; r < numRows - 1; r++)
                    {
                        tileGrid[col][r] = tileGrid[col][r + 1];
                        tileGrid[col][r].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[col][r].GetComponent<TileFSM>().startPos.x, offset.y + tileGrid[col][r].GetComponent<TileFSM>().startPos.y);
                    }
                    tileGrid[col][numRows - 1] = temp;
                    tileGrid[col][numRows - 1].GetComponent<TileFSM>().offGrid = true;
                    tileGrid[col][numRows - 1].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[col][numRows - 1].GetComponent<TileFSM>().startPos.x, offset.y + tileGrid[col][numRows - 1].GetComponent<TileFSM>().startPos.y);
                    tileGrid[col][numRows - 1].GetComponent<TileFSM>().wrapPos = new Vector2(tileSize * col, numRows * tileSize);
                    tileGrid[col][numRows - 1].GetComponent<TileFSM>().wrapGoalPos = new Vector2(tileSize * col, (numRows - 1) * tileSize);
                    break;
                case E:
                    offset.x = tileSize;
                    temp = tileGrid[numCols - 1][row];
                    for (int c = numCols - 1; c > 0; c--)
                    {
                        tileGrid[c][row] = tileGrid[c - 1][row];
                        tileGrid[c][row].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[c][row].GetComponent<TileFSM>().startPos.x, offset.y + tileGrid[c][row].GetComponent<TileFSM>().startPos.y);
                    }
                    tileGrid[0][row] = temp;
                    tileGrid[0][row].GetComponent<TileFSM>().offGrid = true;
                    tileGrid[0][row].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[0][row].GetComponent<TileFSM>().startPos.x, offset.y + tileGrid[0][row].GetComponent<TileFSM>().startPos.y);
                    tileGrid[0][row].GetComponent<TileFSM>().wrapPos = new Vector2(-tileSize, tileSize * row);
                    tileGrid[0][row].GetComponent<TileFSM>().wrapGoalPos = new Vector2(0, tileSize * row);
                    break;
                case W:
                    offset.x = -tileSize;
                    temp = tileGrid[0][row];
                    for (int c = 0; c < numCols - 1; c++)
                    {
                        tileGrid[c][row] = tileGrid[c + 1][row];
                        tileGrid[c][row].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[c][row].GetComponent<TileFSM>().startPos.x, offset.y + tileGrid[c][row].GetComponent<TileFSM>().startPos.y);
                    }
                    tileGrid[numCols - 1][row] = temp;
                    tileGrid[numCols - 1][row].GetComponent<TileFSM>().offGrid = true;
                    tileGrid[numCols - 1][row].GetComponent<TileFSM>().goalPos = new Vector2(offset.x + tileGrid[numCols - 1][row].GetComponent<TileFSM>().startPos.x, offset.y + tileGrid[numCols - 1][row].GetComponent<TileFSM>().startPos.y);
                    tileGrid[numCols - 1][row].GetComponent<TileFSM>().wrapPos = new Vector2(numCols * tileSize, row * tileSize);
                    tileGrid[numCols - 1][row].GetComponent<TileFSM>().wrapGoalPos = new Vector2((numCols - 1) * tileSize, row * tileSize);
                    break;
            }
            moves++;
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
            StateID currState = character.GetComponent<ActorFSM>().fsm.CurrentStateID;
            if (currState == StateID.EnemyDeadA || currState == StateID.TrapDeadA || currState == StateID.LaserDeadA)
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

    public bool sameTileCollide(GameObject actor)
    {
        if (actor.tag == "Player")
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                GameObject enemy = enemies[i];
                if (actor.transform.position == enemy.transform.position)
                {
                    if (actor.GetComponent<ActorFSM>().actorName == "Tank")
                    {
                        enemy.GetComponent<ActorFSM>().sprayShadows();
                        actors.Remove(enemy.gameObject);
                        enemies.Remove(enemy.gameObject);
                        GameObject.Destroy(enemy.gameObject);
                        i--;

                        zombie.GetComponent<SocialPlatform>().AchievementProgress(GPGSIds.achievement_going_bearzerk, true, 1);
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        else if (actor.tag == "Enemy")
        {
            foreach (GameObject character in characters)
            {
                if (character.transform.position == actor.transform.position)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void sameTileOffset()
    {
        List<GameObject> charList = new List<GameObject>();
        List<GameObject> enemList = new List<GameObject>();

        foreach (GameObject tile in tiles)
        {
            foreach (Transform child in tile.transform)
            {
                if (child.CompareTag("Player"))
                {
                    charList.Add(child.gameObject);
                }
                else if (child.CompareTag("Enemy"))
                {
                    enemList.Add(child.gameObject);
                }
            }

            switch (charList.Count)
            {
                case 2:
                    charList[0].transform.position -= new Vector3(tileSize / 4, 0, 0);
                    charList[1].transform.position += new Vector3(tileSize / 4, 0, 0);
                    break;
                case 3:
                    charList[1].transform.position -= new Vector3(tileSize / 4, tileSize / 6, 0);
                    charList[2].transform.position += new Vector3(tileSize / 4, -tileSize / 6, 0);
                    charList[0].transform.position += new Vector3(0, tileSize / 6, 0);
                    break;
                case 4:
                    charList[2].transform.position -= new Vector3(tileSize / 4, tileSize / 6, 0);
                    charList[3].transform.position += new Vector3(tileSize / 4, -tileSize / 6, 0);
                    charList[0].transform.position -= new Vector3(tileSize / 4, -tileSize / 6, 0);
                    charList[1].transform.position += new Vector3(tileSize / 4, tileSize / 6, 0);
                    break;
                default:
                    //0, 1, or more than 4? do nothing
                    break;
            }

            switch (enemList.Count)
            {
                case 2:
                    enemList[0].transform.position -= new Vector3(tileSize / 4, 0, 0);
                    enemList[1].transform.position += new Vector3(tileSize / 4, 0, 0);
                    break;
                case 3:
                    enemList[1].transform.position -= new Vector3(tileSize / 4, tileSize / 6, 0);
                    enemList[2].transform.position += new Vector3(tileSize / 4, -tileSize / 6, 0);
                    enemList[0].transform.position += new Vector3(0, tileSize / 6, 0);

                    break;
                case 4:
                    enemList[2].transform.position -= new Vector3(tileSize / 4, tileSize / 6, 0);
                    enemList[3].transform.position += new Vector3(tileSize / 4, -tileSize / 6, 0);
                    enemList[0].transform.position -= new Vector3(tileSize / 4, -tileSize / 6, 0);
                    enemList[1].transform.position += new Vector3(tileSize / 4, tileSize / 6, 0);
                    break;
                default:
                    //0, 1, or more than 4? do nothing
                    break;
            }

            charList.Clear();
            enemList.Clear();
        }
    }

    public void sameTileReset()
    {
        foreach (GameObject tile in tiles)
        {
            foreach (Transform child in tile.transform)
            {
                if (child.CompareTag("Player") || child.CompareTag("Enemy"))
                {
                    child.position = tile.transform.position;
                }
            }
        }
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
            controlref.levelsList = controlref.zombie.GetComponent<ZombiePasser>().getLevels();
            controlref.currentLevel = controlref.zombie.GetComponent<ZombiePasser>().getLevel();
            foreach (Level level in controlref.levelsList)
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
        if (controlref.doneSliding())
        {
            npc.GetComponent<GameMasterFSM>().SetTransition(Transition.DoneJuicing);
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {

    }

    public override void DoBeforeLeaving()
    {
        foreach (GameObject child in controlref.lasers)
        {
            if (child.tag == "Laser")
            {
                child.GetComponent<LaserScript>().setEye(true);
            }
        }
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

    public override void DoBeforeEntering()
    {
        if (!controlref.incompleteTouch)
        {
            controlref.sameTileOffset();
        }
        controlref.moved = false;
        controlref.incompleteTouch = false;
    }

    public override void DoBeforeLeaving()
    {
        controlref.lastPos = Vector2.zero;
        controlref.isVert = false;
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
        if (controlref.doneSliding())
        {
            foreach (GameObject child in controlref.lasers)
            {
                if (child.tag == "Laser")
                {
                    if (!child.GetComponent<LaserScript>().eyeOpen)
                    {
                        child.GetComponent<LaserScript>().setEye(true);
                    }
                }
            }
        }
        else
        {
            foreach (GameObject child in controlref.lasers)
            {
                if (child.tag == "Laser")
                {
                    if (child.GetComponent<LaserScript>().eyeOpen)
                    {
                        child.GetComponent<LaserScript>().setEye(false);
                    }
                }
            }
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        controlref.swiped = false;
        if (controlref.currDepth < controlref.maxDepth) { controlref.swiped = controlref.HandleTouch(0, Vector2.zero, TouchPhase.Ended, Vector2.zero); } 
        else
        {
            controlref.moveHistory = new List<int>();
            controlref.currDepth = 0;
            controlref.reset();
        }
    }

} //InputState

public class OrderTilesState : FSMState
{
    GameMasterFSM controlref;

    public OrderTilesState(GameMasterFSM control)
    {
        stateID = StateID.OrderTiles;
        controlref = control;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (controlref.doneSliding())
        {
            if (controlref.incompleteTouch)
            {
                npc.GetComponent<GameMasterFSM>().SetTransition(Transition.Incomplete); //to Input
            }
            else
            {
                npc.GetComponent<GameMasterFSM>().SetTransition(Transition.TilesDone); //to orderactors
            }
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {

    }

    public override void DoBeforeEntering()
    {
        controlref.moveGrid(controlref.Column, controlref.Row, controlref.Direction);
        foreach (GameObject child in controlref.lasers)
        {
            if (child.tag == "Laser")
            {
                child.GetComponent<LaserScript>().setEye(false);
            }
        }
    }

    public override void DoBeforeLeaving()
    {
        if (controlref.wrapCopy1 != null)
        {
            MonoBehaviour.Destroy(controlref.wrapCopy1);
            controlref.wrapCopy1 = null;
        }
        if (controlref.wrapCopy2 != null)
        {
            MonoBehaviour.Destroy(controlref.wrapCopy2);
            controlref.wrapCopy2 = null;
        }
        controlref.moved = false;
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
        controlref.sameTileReset();
        controlref.sameTileOrder();
        foreach (GameObject laser in controlref.lasers)
        {
            laser.GetComponent<LaserScript>().setEye(false);
        }
        //controlref.startOrderActors();
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

    public override void DoBeforeLeaving()
    {
        foreach (GameObject child in controlref.lasers)
        {
            if (child.tag == "Laser")
            {
                child.GetComponent<LaserScript>().setEye(true);
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
    }

    public override void Reason(GameObject gm, GameObject npc)
    {

    }

    public override void Act(GameObject gm, GameObject npc)
    {
        Debug.Log("CurrDepth: " + controlref.currDepth + ", Max Depth: " + controlref.maxDepth + ", \nMoves made: ");
        string movesString = "";
        Debug.Log("History length: " + controlref.moveHistory.Count);
        foreach(int move in controlref.moveHistory)
        {
            movesString += move;
            movesString += ", ";
        }
        Debug.Log(movesString);
        controlref.maxDepth = controlref.currDepth;
        controlref.moveHistory = new List<int>();
        controlref.reset();
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
