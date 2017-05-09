﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ActorFSM : MonoBehaviour
{
    // public:
	public GameObject gm;
    public GameObject slash;
    public FSMSystem fsm;
    public bool doneSlide;
    public int direction;
    public Vector2 goalPos;
    public Vector2[] v2Dirs = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    public SpriteRenderer sr;
    public Sprite upSprite;
    public Sprite rightSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public String actorName;
    public int visitedWalk = 0;
    public String enemyDeath = "";
    public String trapDeath = "";

	// actor behavior
	public bool scaleFlag;
	public float scaleFactor;
	public bool rotateFlag;
	public float rotateFactor;

	//deathtexts
	private Dictionary<string, List<string>> deathTexts;
	private bool hitByLaser = false;

    // audio:
    public AudioClip playerfootsteps1;
    public AudioClip playerfootsteps2;

    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    public void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController");
        doneSlide = false;
		scaleFlag = false;
		rotateFlag = false;
		rotateFactor = 0;
		scaleFactor = gameObject.transform.localScale.x;
        goalPos = transform.position;
        sr = GetComponent<SpriteRenderer>();
        MakeFSM();
		deathTexts = new Dictionary<string, List<string>> ();
		List<string> texts = new List<string>();
		texts.Add (" was swallowed by shadows.");
		texts.Add (" was consumed by darkness.");
		deathTexts.Add ("enemy", texts);
		texts = new List<string>();
		texts.Add (" combusted.");
		texts.Add (" did not stop, drop, and roll.");
		deathTexts.Add ("trap", texts);
		texts = new List<string>();
		texts.Add (" was shot through the heart.");
		texts.Add (" was hit by a laser beam.");
		deathTexts.Add ("laser", texts);
    }

    public void Update()
    {
        //print(tag + " == " + fsm.CurrentStateID);
        fsm.CurrentState.Reason(gm, gameObject);
        fsm.CurrentState.Act(gm, gameObject);
    }

    public void setDirection(int dir)
    {
        direction = dir;
        sr = GetComponent<SpriteRenderer>();
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
            default:
                break;
        }
    }

    private void MakeFSM()
    {
        IdleAState idle = new IdleAState(this);
        idle.AddTransition(Transition.FoundMove, StateID.LookA);
		idle.AddTransition (Transition.EnterLevel, StateID.EnterA);
		idle.AddTransition (Transition.IdleDeath, StateID.EnemyDeadA);
		idle.AddTransition (Transition.LaserCollide, StateID.LaserDeadA);

        LookAState look = new LookAState(this);
        look.AddTransition(Transition.EnemyFound, StateID.EnemyDeadA);
        look.AddTransition(Transition.PathFound, StateID.WalkA);
        look.AddTransition(Transition.TrapFound, StateID.TrapDeadA);
        look.AddTransition(Transition.GoalFound, StateID.WinA);

        WalkAState walk = new WalkAState(this);
        walk.AddTransition(Transition.FinishedWalk, StateID.IdleA);
        walk.AddTransition(Transition.EnemyCollide, StateID.EnemyDeadA);
		walk.AddTransition(Transition.SecondMove, StateID.LookA);

        WinAState win = new WinAState(this);
        win.AddTransition(Transition.EnemyCollide1, StateID.EnemyDeadA);

        TrapDeadAState trap = new TrapDeadAState(this);

        EnemyDeadAState enemy = new EnemyDeadAState(this);

		LaserDeadAState laser = new LaserDeadAState (this);

		EnterState enter = new EnterState (this);
		enter.AddTransition (Transition.FinishedEnter, StateID.IdleA);

        fsm = new FSMSystem();
		fsm.AddState(idle);
		fsm.AddState(enter);
        fsm.AddState(look);
        fsm.AddState(walk);
        fsm.AddState(win);
        fsm.AddState(trap);
        fsm.AddState(enemy);
		fsm.AddState (laser);
    }

	/* foundItem is called when actors collide into an item.
	 * it sets found item to true
	 * and destroys the item - Justin
	 */ 
	public void foundItem(GameObject Item)
	{
		GameObject.Find("GameMaster").GetComponent<GameMasterFSM>().foundItem = true;
		Destroy(Item);
	}

    public void destroyObj()
    {
        Destroy(gameObject);
    }

	// Use to scale, tilt, or rotate for desired effect
	public void wiggle(float scaleSpeed, float rotateSpeed, float lowerLimit=.6f, float upperLimit=0.7f, float rLL=0.3f, float rUL=0.6f){
		// controls scaling:
		if (scaleFlag) {
			scaleFactor += scaleSpeed;
		} else {
			scaleFactor -= scaleSpeed;
		}
			
		if (scaleFactor <= lowerLimit) {
			scaleFactor = lowerLimit;
			scaleFlag = true;
		}
		if (scaleFactor >= upperLimit) {
			scaleFactor = upperLimit;
			scaleFlag = false;
		}
		gameObject.transform.localScale = new Vector3 (scaleFactor, scaleFactor, 0.0f); 

		// controls rotation:
		if (rotateFlag) {
			rotateFactor += 0.02f;
		} else {
			rotateFactor -= 0.02f;
			rotateSpeed = -rotateSpeed;
		}
		if (rotateFactor <= rLL) {
			rotateFactor = rLL;
			rotateFlag = true;
		}
		if (rotateFactor >= rUL) {
			rotateFactor = rUL;
			rotateFlag = false;
		}
		gameObject.transform.RotateAround(gameObject.transform.position,new Vector3(0,0,1),rotateSpeed);
	}

	public void setDeathText(string cause){
		List<string> texts = deathTexts[cause];
		int msg = UnityEngine.Random.Range(0,texts.Count);
		Debug.Log (msg + "/" + texts.Count);
		gm.GetComponent<GameMasterFSM>().deathText.text = actorName + texts[msg];
	}

	public void setLaserHit(bool hit){
		hitByLaser = hit;
	}

	public bool isHitByLaser(){
		return hitByLaser;
	}

    public virtual int findNextMove(int dir)
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
            //Debug.Log(this+", "+currDir);
            RaycastHit2D ray = Physics2D.Raycast(transform.position, v2Dirs[currDir], GetComponentInParent<TileFSM>().GetComponent<Renderer>().bounds.size.x, LayerMask.GetMask("Wall"));

            if (ray.collider == null || !(ray.collider.tag == "Wall" || ray.collider.tag == "OuterWall"))
            {
                return currDir;
            }
        }
        return -1;//walls in all 4 directions, no moves found
    }

    public void walk()
    {
        //decide where to move and call WalkTo based on direction
        float walkDistance = gm.GetComponent<GameMasterFSM>().tileSize;
        switch (direction)
        {
            case 0:
                sr.sprite = upSprite;
                WalkTo(new Vector2(0, walkDistance));
                break;
            case 1:
                sr.sprite = rightSprite;
                WalkTo(new Vector2(walkDistance, 0));
                break;
            case 2:
                sr.sprite = downSprite;
                WalkTo(new Vector2(0, -walkDistance));
                break;
            case 3:
                sr.sprite = leftSprite;
                WalkTo(new Vector2(-walkDistance, 0));
                break;
            default:
                break;
        }
    }

    public void WalkTo(Vector2 pos)
    {
        goalPos = new Vector2(pos.x + transform.position.x, pos.y + transform.position.y);
    }
}

public class IdleAState : FSMState
{
	public ActorFSM controlref;

	public IdleAState(ActorFSM control)
	{
		stateID = StateID.IdleA;
		controlref = control;
	}

	public override void Reason(GameObject gm, GameObject npc)
	{
		if (npc.GetComponent<ActorFSM>().isHitByLaser()) {
			npc.GetComponent<ActorFSM> ().SetTransition (Transition.LaserCollide);
			npc.GetComponent<ActorFSM> ().setDeathText ("laser");
			return;
		}

		if (gm.GetComponent<GameMasterFSM>().sameTileCollide())
		{
			npc.GetComponent<ActorFSM>().SetTransition(Transition.IdleDeath); //to enemyDead
		}
        else if (controlref.doneSlide)
        {
            int temp = controlref.findNextMove(controlref.direction);//if -1, direction does not change and state stays idle, else update direction
            if (temp >= 0)//-1 means no move found
            {
                controlref.direction = temp;
                controlref.walk();
                npc.GetComponent<ActorFSM>().SetTransition(Transition.FoundMove); //to Look
            }
            else
            {
                controlref.doneSlide = false;
            }
        }
	}

	public override void Act(GameObject gm, GameObject npc)
	{
		// Idle Behavior similar to IMBROGLIO !!!
		if (controlref.tag == "Player") {
			npc.GetComponent<ActorFSM> ().wiggle (0.001f, 0.1f);
		}
		if (controlref.tag == "Enemy") {
			npc.GetComponent<ActorFSM> ().wiggle (0.001f, 0.1f, 1.5f, 1.6f);
		}
	}

} //IdleState

public class LookAState : FSMState
{
	ActorFSM controlref;

    public LookAState(ActorFSM control)
	{
		stateID = StateID.LookA;
		controlref = control;
	}
    //right now this is based on logic with tags which is bad, but I'm not sure how to move to a polymorphic style for our actor entities
    public override void Reason(GameObject gm, GameObject npc)
    {
        bool isDead = false;
        RaycastHit2D ray = Physics2D.Raycast(npc.transform.position, controlref.v2Dirs[controlref.direction], controlref.GetComponentInParent<TileFSM>().GetComponent<Renderer>().bounds.size.x, LayerMask.GetMask("Collidables"));

        if (ray.collider != null)
        {
            if(ray.collider.tag == "Trap")//both enemy and player
            {
		ray.transform.gameObject.GetComponent<FireSystem> ().setOn ();
                npc.GetComponent<ActorFSM>().SetTransition(Transition.TrapFound); //to trapDeath
                return;
            }
            if (npc.tag == "Player") {
                if (ray.collider.tag == "Enemy")
                {
			int enemyDir = ray.collider.gameObject.GetComponent<ActorFSM>().direction;
			switch (controlref.direction)
			{
				case 0:
				{
					if (enemyDir == 2) { isDead = true; }
					break;
				}
				case 1:
				{
					if (enemyDir == 3) { isDead = true; }
					break;
				}
				case 2:
				{
					if (enemyDir == 0) { isDead = true; }
					break;
				}
				case 3:
				{
					if (enemyDir == 1) { isDead = true; }
					break;
				}
			}
		}	
                else if (ray.collider.tag == "Goal")
                {
                    ray.collider.gameObject.GetComponent<goalLight>().FlashLight();
                    npc.GetComponent<ActorFSM>().SetTransition(Transition.GoalFound); //to Win
                    return;
		}
		else if (ray.collider.tag == "Item")
		{
			npc.GetComponent<ActorFSM>().foundItem(ray.collider.gameObject);
			return;
		}
                if (isDead)
                {
                    npc.GetComponent<ActorFSM>().SetTransition(Transition.EnemyFound); //to Dead
                    return;//needed to skip pathfound transition from firing, current logic structure is a bit iffy
                }
		else
            	{
			//raycast saw something, but actor did not win or die
                	npc.GetComponent<ActorFSM>().SetTransition(Transition.PathFound); //to Walk
			return;
            	}
            }
            else
            {
	    	//Raycast saw something other than trap, actor is enemy
                npc.GetComponent<ActorFSM>().SetTransition(Transition.PathFound); //to Walk
		return;
            }
        }
        else
        {
		//raycast saw nothing
            npc.GetComponent<ActorFSM>().SetTransition(Transition.PathFound); //to Walk
	    return;
        }
    }

	public override void Act(GameObject gm, GameObject npc)
	{
        //shouldn't ever be called, which is a sign that this state is questionable, but the reason function is resolving alot of collision logic
	}

} // CheckAState

public class WalkAState : FSMState
{
	ActorFSM controlref;
    private float speed = .03f;

    public WalkAState(ActorFSM control)
	{
		stateID = StateID.WalkA;
		controlref = control;
	}

	public override void Reason(GameObject gm, GameObject npc)
	{
        if (npc.transform.position.x == controlref.goalPos.x && npc.transform.position.y == controlref.goalPos.y)
        {
            if (gm.GetComponent<GameMasterFSM>().sameTileCollide())
            {
                npc.GetComponent<ActorFSM>().SetTransition(Transition.EnemyCollide); //to enemyDead
                return;
            }
            else
            {
                controlref.doneSlide = false;
                //do before leaving
                controlref.transform.parent = gm.GetComponent<GameMasterFSM>().getTile(controlref.transform.position).transform;
                if (controlref.actorName == "Emily" || controlref.actorName == "Wraith") {
					if (controlref.visitedWalk == 0) {
						controlref.visitedWalk++;
						// can be reworked into transition to idle state.
						int temp = controlref.findNextMove(controlref.direction);//if -1, direction does not change and state stays idle, else update direction
						if (temp >= 0) {//-1 means no move found
							controlref.direction = temp;
							controlref.walk ();
						}
						npc.GetComponent<ActorFSM>().SetTransition(Transition.SecondMove);
						//
					} else {
						controlref.visitedWalk = 0;
						npc.GetComponent<ActorFSM>().SetTransition(Transition.FinishedWalk);
					}
				} else {
					npc.GetComponent<ActorFSM>().SetTransition(Transition.FinishedWalk);
				}
            }
        }

    }

    public override void Act(GameObject gm, GameObject npc)
	{
		// SoundController.instance.PlaySingleDelay(controlref.playerfootsteps1);

		// walk behavior
		if (controlref.tag == "Player") {
			npc.GetComponent<ActorFSM> ().wiggle (0.001f, 0.5f, 0.6f, 0.7f, 0.2f, 0.6f);
		}
		if (controlref.tag == "Enemy") {
			npc.GetComponent<ActorFSM> ().wiggle (0.001f, 0.5f, 1.5f, 1.6f, 0.2f, 0.6f);
		}

        npc.transform.position = Vector2.MoveTowards(npc.transform.position, controlref.goalPos, speed);
    }

} // WalkAState

public class WinAState : FSMState
{
    ActorFSM controlref;
    private float speed = .07f;

    public WinAState(ActorFSM control)
    {
        stateID = StateID.WinA;
        controlref = control; 
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (npc.transform.position.x == controlref.goalPos.x && npc.transform.position.y == controlref.goalPos.y)
        {
            if (gm.GetComponent<GameMasterFSM>().sameTileCollide())
            {
                npc.GetComponent<ActorFSM>().SetTransition(Transition.EnemyCollide1); //to enemyDead
                return;
            }
            gm.GetComponent<GameMasterFSM>().characters.Remove(npc);
            gm.GetComponent<GameMasterFSM>().actors.Remove(npc);
            controlref.destroyObj();
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        npc.transform.position = Vector2.MoveTowards(npc.transform.position, controlref.goalPos, speed);
    }

} // WinState

public class TrapDeadAState : FSMState
{
    ActorFSM controlref;
    private float speed = .07f;

    public TrapDeadAState(ActorFSM control)
    {
        stateID = StateID.TrapDeadA;
        controlref = control;
    }

    public override void DoBeforeEntering()
    {
        if (controlref.tag == "Player")
        {
			controlref.GetComponent<ActorFSM> ().setDeathText ("trap");
        }
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (npc.transform.position.x == controlref.goalPos.x && npc.transform.position.y == controlref.goalPos.y)
        {
            if (npc.tag == "Player")
            {
                //gm.GetComponent<GameMasterFSM>().characters.Remove(npc);
                //gm.GetComponent<GameMasterFSM>().actors.Remove(npc);
                //controlref.destroyObj();
                controlref.sr.enabled = false;
            }
            else if (npc.tag == "Enemy")
            {
                gm.GetComponent<GameMasterFSM>().enemies.Remove(npc);
                gm.GetComponent<GameMasterFSM>().actors.Remove(npc);
                controlref.destroyObj();
            }
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        npc.transform.position = Vector2.MoveTowards(npc.transform.position, controlref.goalPos, speed);
    }

} // TrapDeadState

public class EnemyDeadAState : FSMState
{
    ActorFSM controlref;

    public EnemyDeadAState(ActorFSM control)
    {
        stateID = StateID.EnemyDeadA;
        controlref = control;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        
    }

    public override void DoBeforeEntering()
    {
		if (controlref.tag == "Player")
        {
            UnityEngine.Object.Instantiate(controlref.slash, controlref.transform.position, Quaternion.identity);
            controlref.GetComponent<ActorFSM> ().setDeathText ("enemy");
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        if (npc.tag == "Player")
        {
            //gm.GetComponent<GameMasterFSM>().characters.Remove(npc);
            //gm.GetComponent<GameMasterFSM>().actors.Remove(npc);
            //controlref.destroyObj();
            controlref.sr.enabled = false;
        }
        else if (npc.tag == "Enemy")
        {
            //gm.GetComponent<GameMasterFSM>().enemies.Remove(npc);
            //gm.GetComponent<GameMasterFSM>().actors.Remove(npc);
            //controlref.destroyObj();
        }
    }
} // EnemyDeadState

public class LaserDeadAState : FSMState
{
	ActorFSM controlref;

	public LaserDeadAState(ActorFSM control)
	{
		stateID = StateID.LaserDeadA;
		controlref = control;
	}

	public override void Reason(GameObject gm, GameObject npc)
	{

	}

	public override void Act(GameObject gm, GameObject npc)
	{
		if (npc.tag == "Player")
		{
            //gm.GetComponent<GameMasterFSM>().characters.Remove(npc);
            //gm.GetComponent<GameMasterFSM>().actors.Remove(npc);
            //controlref.destroyObj();
			controlref.sr.enabled = false;
		}
		else if (npc.tag == "Enemy")
		{
			//gm.GetComponent<GameMasterFSM>().enemies.Remove(npc);
			//gm.GetComponent<GameMasterFSM>().actors.Remove(npc);
			//controlref.destroyObj();
		}
	}
} // LaserDeadAState

public class EnterState : FSMState
{
	ActorFSM controlref;
	private float speed = .07f;

	public EnterState(ActorFSM control)
	{
		stateID = StateID.EnterA;
		controlref = control;
	}

    public override void Reason(GameObject gm, GameObject npc)
	{
		if (controlref.transform.position.x == controlref.goalPos.x && npc.transform.position.y == controlref.goalPos.y)
		{
			npc.GetComponent<ActorFSM>().SetTransition(Transition.EnemyCollide); //to Idle
		}

	}

	public override void Act(GameObject gm, GameObject npc)
	{
		npc.transform.position = Vector2.MoveTowards(npc.transform.position, controlref.goalPos, speed);
	}
		
} // EnterState
