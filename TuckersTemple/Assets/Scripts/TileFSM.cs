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

    public void Start()
    {
        goalPos = transform.position;
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

        fsm = new FSMSystem();
        fsm.AddState(idle);
        fsm.AddState(moving);
        fsm.AddState(wrap);
    }

    public void setTile(string currentTileType)
    {
        int[] wallCheck = { 0, 0, 0, 0 };
        GameObject wall;

        switch (currentTileType)
        {
            case "x":
            case "╬":
                break;
            case "T0":
            case "╦":
                wallCheck[0] = 1;
                break;
            case "T1":
            case "╣":
                wallCheck[1] = 1;
                break;
            case "T2":
            case "╩":
                wallCheck[2] = 1;
                break;
            case "T3":
            case "╠":
                wallCheck[3] = 1;
                break;
            case "I0":
            case "║":
                wallCheck[1] = 1;
                wallCheck[3] = 1;
                break;
            case "I1":
            case "═":
                wallCheck[0] = 1;
                wallCheck[2] = 1;
                break;
            case "L0":
            case "╚":
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                break;
            case "L1":
            case "╔":
                wallCheck[0] = 1;
                wallCheck[3] = 1;
                break;
            case "L2":
            case "╗":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                break;
            case "L3":
            case "╝":
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                break;
            case "V0":
            case "u":
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                break;
            case "V1":
            case "[":
                wallCheck[0] = 1;
                wallCheck[2] = 1;
                wallCheck[3] = 1;
                break;
            case "V2":
            case "n":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                wallCheck[3] = 1;
                break;
            case "V3":
            case "]":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                break;
            case "N":
            case "¤":
                wallCheck[0] = 1;
                wallCheck[1] = 1;
                wallCheck[2] = 1;
                wallCheck[3] = 1;
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