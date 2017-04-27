using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ActorFSM : MonoBehaviour
{
    public GameObject gm;
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
	private bool hitByLaser = false;

    // audio:
    public AudioClip playerfootsteps1;
    public AudioClip playerfootsteps2;

    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    public void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController");
        doneSlide = false;
        goalPos = transform.position;
        sr = GetComponent<SpriteRenderer>();
        MakeFSM();
		int msg = UnityEngine.Random.Range(0, 2);
		switch (msg)
		{
		case 0:
			enemyDeath = actorName + " was swallowed by shadows.";
			break;
		case 1:
			enemyDeath = actorName + " let the darkness consume them.";
			break;
		}
		msg = UnityEngine.Random.Range(0,4);
		switch (msg)
		{
		case 0:
			trapDeath = actorName + " activated a trap card.";
			break;
		case 1:
			trapDeath = actorName + " spontaneously combusted.";
			break;
		case 2:
			trapDeath = actorName + " did not stop, drop, and roll.";
			break;
		case 3:
			trapDeath = actorName + " forgot to turn off the oven.";
			break;
		}
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

    public void destroyObj()
    {
        Destroy(gameObject);
    }

	public void setDeathText(string cause){
		int msg = UnityEngine.Random.Range(0,0);
		switch (msg)
		{
			case 0:
				gm.GetComponent<GameMasterFSM>().deathText.text = actorName + " walked into a laser.";
				break;
		}
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
		//idle	
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
                    npc.GetComponent<ActorFSM>().SetTransition(Transition.GoalFound); //to Win
                    return;
                }
                if (isDead)
                {
                    npc.GetComponent<ActorFSM>().SetTransition(Transition.EnemyFound); //to Dead
                    return;//needed to skip pathfound transition from firing, current logic structure is a bit iffy
                }
            }
            else
            {
                npc.GetComponent<ActorFSM>().SetTransition(Transition.PathFound); //to Walk
            }
        }
        else
        {
            npc.GetComponent<ActorFSM>().SetTransition(Transition.PathFound); //to Walk
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
    private float speed = .07f;

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
        SoundController.instance.PlaySingleDelay(controlref.playerfootsteps1);
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
        if (controlref.actorName == "Roy" || controlref.actorName == "Emily")
        {
            controlref.gm.GetComponent<GameMasterFSM>().deathText.text = controlref.trapDeath;
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
        if (controlref.actorName == "Roy" || controlref.actorName == "Emily")
        {
            controlref.gm.GetComponent<GameMasterFSM>().deathText.text = controlref.enemyDeath;
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
