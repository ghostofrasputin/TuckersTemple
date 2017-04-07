using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TileFSM : MonoBehaviour
{
    public GameObject gm;
    public FSMSystem fsm;
    public Vector2 goalPos;
    public Vector2 wrapPos;
    public Vector2 wrapGoalPos;
    public bool offGrid;
    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    public GameObject Wall;
    public Sprite upWall;
    public Sprite rightWall;
    public Sprite downWall;
    public Sprite leftWall;

    public GameObject corners;
    public GameObject pathOverlay;

    public Sprite XPath;
    public Sprite TPath;
    public Sprite IPath;
    public Sprite LPath;
    public Sprite VPath;

    public void Start()
    {
		gm = GameObject.FindGameObjectWithTag ("GameController").gameObject;
        goalPos = transform.position;
		if (goalPos.y/GetComponent<SpriteRenderer>().bounds.size.y % 2 == 0) {
			transform.position = new Vector2 (goalPos.x - 3, goalPos.y);
		} else {
			transform.position = new Vector2 (goalPos.x + 3, goalPos.y);
		}
        offGrid = false;
        MakeFSM();
    }

    public void Update()
    {
        fsm.CurrentState.Reason(gm, gameObject);
        fsm.CurrentState.Act(gm, gameObject);
    }

    // The tile has 3 states: idle, wrapping and moving
    // If it's on idle and userSwipe transition is fired, it changes to moving
    // If it's on moving and reachedGoal transition is fired, it returns to idle
    // If it's on moving and offGrid is fired, it changes to wrapping
    // If it's on wrapping and finished wrap is fired, it changes to idle
    private void MakeFSM()
    {
        MoveState moving = new MoveState(this);
        moving.AddTransition(Transition.ReachedGoal, StateID.Idle);
        moving.AddTransition(Transition.OffGrid, StateID.Wrapping);

        IdleState idle = new IdleState(this);
        idle.AddTransition(Transition.UserSwiped, StateID.Moving);

        WrapState wrap = new WrapState(this);
        wrap.AddTransition(Transition.FinishedWrap, StateID.Idle);

		SetupState setup = new SetupState (this);
		setup.AddTransition(Transition.FinishedSetup, StateID.Idle);

        fsm = new FSMSystem();
		fsm.AddState(setup);
        fsm.AddState(idle);
        fsm.AddState(moving);
        fsm.AddState(wrap);
    }

    public void setTile(string currentTileType)
    {
        int[] wallCheck = { 0, 0, 0, 0 };
        GameObject wall;
        GameObject paths = Instantiate(pathOverlay, transform.position, Quaternion.identity, transform);
        Instantiate(corners, transform.position, Quaternion.identity, transform);

        switch (currentTileType)
        {
            case "x":
            case "╬":
                paths.GetComponent<SpriteRenderer>().sprite = XPath;
                break;
            case "T0":
            case "╦":
                wallCheck[0] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = TPath;
                break;
            case "T1":
            case "╣":
                wallCheck[1] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = TPath;
                paths.transform.Rotate(Vector3.forward * -90);
                break;
            case "T2":
            case "╩":
                wallCheck[2] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = TPath;
                paths.transform.Rotate(Vector3.forward * 180);
                break;
            case "T3":
            case "╠":
                wallCheck[3] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = TPath;
                paths.transform.Rotate(Vector3.forward * 90);
                break;
            case "I0":
            case "║":
                wallCheck[1] = 1;
                wallCheck[3] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = IPath;
                break;
            case "I1":
            case "═":
                wallCheck[0] = 1;
                wallCheck[2] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = IPath;
                paths.transform.Rotate(Vector3.forward * -90);
                break;
            case "L0":
            case "╚":
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = LPath;
                break;
            case "L1":
            case "╔":
                wallCheck[0] = 1;
                wallCheck[3] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = LPath;
                paths.transform.Rotate(Vector3.forward * -90);
                break;
            case "L2":
            case "╗":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = LPath;
                paths.transform.Rotate(Vector3.forward * 180);
                break;
            case "L3":
            case "╝":
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = LPath;
                paths.transform.Rotate(Vector3.forward * 90);
                break;
            case "V0":
            case "u":
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = VPath;
                break;
            case "V1":
            case "[":
                wallCheck[0] = 1;
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = VPath;
                paths.transform.Rotate(Vector3.forward * -90);
                break;
            case "V2":
            case "n":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                wallCheck[3] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = VPath;
                paths.transform.Rotate(Vector3.forward * 180);
                break;
            case "V3":
            case "]":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                paths.GetComponent<SpriteRenderer>().sprite = VPath;
                paths.transform.Rotate(Vector3.forward * 90);
                break;
            case "N":
            case "¤":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                GameObject.Destroy(paths);
                break;
        }
        for (int i = 0; i < 4; i++)
        {
            int currentWallBoolean = wallCheck[i];
            if (currentWallBoolean == 1)
            {
                wall = Instantiate(Wall, transform.position, Quaternion.identity, transform);
                // wall.transform.localScale = new Vector3 (.01f, .05f, .1f);
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                float offset = wall.GetComponent<Renderer>().bounds.size.x;
                // right wall:
                if (i == 0)
                {
                    sr.sprite = upWall;
                    wall.transform.Translate(new Vector3(0, offset, 0));
                }
                // right wall:
                if (i == 1)
                {
                    sr.sprite = rightWall;
                    wall.transform.Translate(new Vector3(offset, 0.1f, 0));
                    wall.transform.localScale = new Vector3(1, 1.4f, 1);
                }
                // bottom wall:
                if (i == 2)
                {
                    sr.sprite = downWall;
                    wall.transform.Translate(new Vector3(0, offset * -1, 0));
                }
                // left wall:
                if (i == 3)
                {
                    sr.sprite = leftWall;
                    wall.transform.Translate(new Vector3(offset * -1, 0.1f, 0));
                    wall.transform.localScale = new Vector3(1, 1.4f, 1);
                }
            }
        }
    }
}

public class MoveState : FSMState
{
    public TileFSM controlref;
    private float speed = .05f;

    public MoveState(TileFSM control)
    {
        stateID = StateID.Moving;
        controlref = control;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (npc.transform.position.x == controlref.goalPos.x && npc.transform.position.y == controlref.goalPos.y)
        {
            if (controlref.offGrid)
            {
                //do before leaving
                controlref.offGrid = false;
                npc.transform.position = controlref.wrapPos;
                controlref.goalPos = controlref.wrapGoalPos;

                npc.GetComponent<TileFSM>().SetTransition(Transition.OffGrid);
            }
            else
            {
                npc.GetComponent<TileFSM>().SetTransition(Transition.ReachedGoal);
            }
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        npc.transform.position = Vector2.MoveTowards(npc.transform.position, controlref.goalPos, speed);
    }

} //MoveState

public class IdleState : FSMState
{
    TileFSM controlref;

    public IdleState(TileFSM control)
    {
        stateID = StateID.Idle;
        controlref = control;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if(npc.transform.position.x != controlref.goalPos.x || npc.transform.position.y != controlref.goalPos.y)
        {
            npc.GetComponent<TileFSM>().SetTransition(Transition.UserSwiped);
        }
        
    }

    public override void Act(GameObject gm, GameObject npc)
    {
    }

} // IdleState

public class WrapState : FSMState
{
    TileFSM controlref;
    private float spd = .08f;

    public WrapState(TileFSM control)
    {
        stateID = StateID.Wrapping;
        controlref = control;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        //magic number hack for tile scale
        if (npc.transform.position.x == controlref.goalPos.x && npc.transform.position.y == controlref.goalPos.y)
        {
            npc.GetComponent<TileFSM>().SetTransition(Transition.FinishedWrap);
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        npc.transform.position = Vector2.MoveTowards(npc.transform.position, controlref.goalPos, spd);
    }

} // ChasePlayerState

public class SetupState : FSMState
{
	public TileFSM controlref;
	private float speed = .05f;

	public SetupState(TileFSM control)
	{
		stateID = StateID.Setup;
		controlref = control;
	}

	public override void Reason(GameObject gm, GameObject npc)
	{
		if (controlref.transform.position.x == controlref.goalPos.x && controlref.transform.position.y == controlref.goalPos.y)
		{
			if (gm.GetComponent<GameMasterFSM> ().fsm.CurrentStateID == StateID.Juice) {
				gm.GetComponent<GameMasterFSM> ().SetTransition (Transition.DoneJuicing);
			}
			controlref.GetComponent<TileFSM>().SetTransition(Transition.FinishedSetup);
		}
	}

	public override void Act(GameObject gm, GameObject npc)
	{
		controlref.transform.position = Vector2.MoveTowards(controlref.transform.position, controlref.goalPos, speed);
	}

} //SetupState