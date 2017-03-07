/*
 * Actor.cs
 * 
 * Is attached to the Actor prefab
 */

// namespaces:
using UnityEngine;
using System.Collections;

// Actor class - base class for characters
public class Actor : MonoBehaviour {

	// public fields:
	public int direction;
	public float speed = 0.05f;
	
	// Sfx for player 
	public AudioClip playerfootsteps1;
	public AudioClip playerfootsteps2;
	public AudioClip playerWinSound;
	public AudioClip playerdeathSound;

    // actor sprites for each direction.
    public Sprite upSprite;
    public Sprite rightSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public bool death;
    // private fields:
    protected bool isWalking;

    protected bool foundWall;
    protected bool escaped;
	protected GameMaster gm;
    protected Vector2 goalPos;
    protected Vector2[] v2Dirs = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    protected SpriteRenderer sr;


	// Actor Initialization
	void Start() {
        //find and save the GameMaster
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameMaster>();
        // get a reference to this actor's sprite renderer.
        sr = GetComponent<SpriteRenderer>();
		// extract initial direction from lvl file
		direction = 0; 
        goalPos = transform.position;
        isWalking = false;
        death = false;
        foundWall = false;
        escaped = false;
	}
	
	// Actor Update
	void Update() {
        // check for characters turn to walk
        if (isWalking)
        {
		SoundController.instance.RandomSfx (playerfootsteps1, playerfootsteps2);
            if(goalPos.x == transform.position.x && goalPos.y == transform.position.y)
            {
                // print(goalPos); debugging
                //set new parent based on goalPos
                transform.parent = gm.getTile(transform.position).transform;
                //if (gameObject.tag == "Player") print(death);
                if (death)
                {
                    SoundController.instance.RandomSfx (playerdeathSound, playerdeathSound);
                    // cleanup later
                    isWalking = false;
                    gm.doneWalking();
                    gm.deleteActor(this.gameObject);
                    if(gameObject.tag == "Player")
                    {
						gm.levelDeath ();
					}
                    else
                    {
                        Destroy(this.gameObject);
                        //print("actor died");
                    }
                }
                else if (escaped)
                {
		            //SoundController.instance.RandomSfx (playerWinSound, playerWinSound);
                    //print("made it out!");
                    // Application.Quit();
                }
                else
                {
                    death = false;
                }
                isWalking = false;
                gm.doneWalking();
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, goalPos, speed);
            }
        }
	}

    public void setDirection(int dir)
    {
        direction = dir;
        print(dir);
        switch (direction)
        {
            case 0:
                sr.sprite = upSprite;
                break;
            case 1:
                sr.sprite = rightSprite;
                break;
            case 2:
                sr.sprite = downSprite;
                break;
            case 3:
                sr.sprite = leftSprite;
                break;
        }
    }

	// walk to new tile
	public void walk(){
        int directionToWalk = findNextMove(direction);

        //decide where to move and call WalkTo based on directionToWalk
        float walkDistance = gm.tileSize;
        switch (directionToWalk)
        {
            //no moves
            case -1:
                gm.doneWalking();
                break;
			case 0:
				direction = directionToWalk;
                sr.sprite = upSprite;
                WalkTo(new Vector2(0, walkDistance));
                break;
            case 1:
                direction = directionToWalk;
                sr.sprite = rightSprite;
                WalkTo(new Vector2(walkDistance, 0));
                break;
            case 2:
                direction = directionToWalk;
                sr.sprite = downSprite;
                WalkTo(new Vector2(0, -walkDistance));
                break;
            case 3:
                direction = directionToWalk;
                sr.sprite = leftSprite;
                WalkTo(new Vector2(-walkDistance, 0));
                break;
        }

	}

    //Tells the character the offset to walk to
    public void WalkTo(Vector2 pos)
    {
        goalPos = new Vector2(pos.x + transform.position.x, pos.y + transform.position.y);
        isWalking = true;
    }

    //take in direction for actor to move
    //returns 0,1,2,3 for which direction they should move
    //returns -1 if no valid move found
    private int findNextMove(int dir)
    {

        //order to try in is straight->right->left->back

        //this is the modifies to the directions something can face
        int[] dirMods = { 0, 1, -1, 2 };
        //directions are 0,1,2,3, with 0 being up and going clockwise.
        for (int i = 0; i < 4; i++)
        {
            //make a current direction by adding the direction modifier to the direction
            int currDir = dir + dirMods[i];

            //Normalize currDir within 0 to 3
            if (currDir > 3)
            {
                currDir -= 4;
            }
            else if (currDir < 0)
            {
                currDir += 4;
            }


            //RAYCAST LASER BEAMS ♫♫♫♫♫
            //Actual comment: Raycasts are complicated.  Here it is then I'll explain
            RaycastHit2D[] raycasts = Physics2D.RaycastAll(transform.position, v2Dirs[currDir], gm.tileSize, LayerMask.GetMask("Wall"));
            //RaycastHit2D raycastEnemy = Physics2D.Raycast(transform.position, v2Dirs[currDir], gm.tileSize, LayerMask.GetMask("Enemy"));
            /*
             * RaycastHit2D is a return type of raycasts, that holds something
             * If it holds nothing, then there was nothing! woooo
             * Arg1 - position, where we start the laser beam
             * Arg2 - direction, from Vector2.up, down etc
             * Arg3 - distance, we don't want further than tile size
             * Arg4 - The layer mask, currently walls so thats all we shoot at
             */

            // using RAYCAAAAST! for enemy collision
            //if (raycastEnemy
            //if raycast doesn't return anything, then there isn't anything there!
            foreach (RaycastHit2D ray in raycasts)
            {
                // print(currDir + " " + ray.collider);
                if (ray.collider.tag == "Wall" || ray.collider.tag == "OuterWall")
                {
                    foundWall = true;
                    //print("I found a wall");
                    break;
                }
            }
            if (!foundWall)
            {
                foreach (RaycastHit2D ray in raycasts)
                {
                    //print(ray.collider.tag);
                    if (ray.collider.tag == "Enemy")
                    {
						print ("Enemy");
						int EnemyView = ray.collider.gameObject.GetComponent<Actor>().direction;
                        switch (EnemyView)
                        {
                            case 0:
                                {
                                    if (direction == 2)
                                    {
                                        death = true;
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    if (direction == 3)
                                    {
                                        death = true;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (direction == 0)
                                    {
                                        death = true;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (direction == 1)
                                    {
                                        death = true;
                                    }
                                    break;
                                }
                        }
                    }
                    else if (ray.collider.tag == "Goal")
                    {
                        //Set a win varible to true
                        escaped = true;
                        //print("Escape!");
                        if (this.GetType().Name == "Actor") {
                          gm.levelWin ();
                        }
                    }
                    else if (ray.collider.tag == "Trap")
                    {
                        //print("You activated my Trap card");
                        death = true;
                    }

                }
                return currDir;
            }
            foundWall = false;
        }
        //if no moves were found
        return -1;
    }

}
