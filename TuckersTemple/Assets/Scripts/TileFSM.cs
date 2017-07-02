using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TileFSM : MonoBehaviour
{
    public GameObject gm;
    public FSMSystem fsm;
    public Vector2 startPos;
    public Vector2 goalPos;
    public Vector2 wrapPos;
    public Vector2 wrapGoalPos;
    public bool offGrid;
    public float offset;
    public float tileSize;
    public int[] walls;
    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    public GameObject Wall;
    public Sprite upWall;
    public Sprite rightWall;
    public Sprite downWall;
    public Sprite leftWall;

    public GameObject corners;
    public GameObject pathOverlay;
    public ParticleSystem dustParticle;

    public Sprite XPath;
    public Sprite TPath;
    public Sprite IPath;
    public Sprite LPath;
    public Sprite VPath;

    public float threshold;
    public Vector2 currentDist;
    public Vector2 maxDist;
    public bool touchReleased;
    public bool incompleteMove;
    public Vector2 netDelta;


    public void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").gameObject;

        goalPos = transform.position;
        tileSize = gm.GetComponent<GameMasterFSM>().tileSize;
        if (goalPos.y / tileSize % 2 == 0)
        {
            transform.position = new Vector2(goalPos.x - 3, goalPos.y);
        }
        else
        {
            transform.position = new Vector2(goalPos.x + 3, goalPos.y);
        }
        incompleteMove = false;
        offGrid = false;
        touchReleased = false;

        setSortingLayer(-(int)Mathf.Floor(transform.position.y / tileSize));
        netDelta = Vector2.zero;

        MakeFSM();
    }

    public void Update()
    {
        fsm.CurrentState.Reason(gm, gameObject);
        fsm.CurrentState.Act(gm, gameObject);
    }

    public void moveTo(Vector2 goalOffset)
    {
        netDelta += goalOffset;
        //Debug.Log("netDelta: " + netDelta + "goalOffset: " + goalOffset);
        
        if (Mathf.Abs(netDelta.x) < maxDist.x && Mathf.Abs(netDelta.y) < maxDist.y)
        {
            Vector2 goalPosition = new Vector2(startPos.x + netDelta.x, startPos.y + netDelta.y);
            //Debug.Log("goalpos: " + netDelta + " < " + maxDist);
            transform.position = new Vector2(goalPosition.x, goalPosition.y);
        }
    }

    // The tile has 3 states: idle, wrapping and moving
    // If it's on idle and userSwipe transition is fired, it changes to moving
    // If it's on moving and reachedGoal transition is fired, it returns to idle
    // If it's on moving and offGrid is fired, it changes to wrapping
    // If it's on wrapping and finished wrap is fired, it changes to idle
    private void MakeFSM()
    {

        IdleState idle = new IdleState(this);
        idle.AddTransition(Transition.UserSwiped, StateID.Follow);

        FollowState follow = new FollowState(this);
        follow.AddTransition(Transition.FinishedFollow, StateID.Snapping);

        SnappingState snap = new SnappingState(this);
        snap.AddTransition(Transition.FinishedSnapping, StateID.Idle);
        snap.AddTransition(Transition.OffGrid, StateID.Wrapping);

        WrapState wrap = new WrapState(this);
        wrap.AddTransition(Transition.FinishedWrap, StateID.Idle);

        SetupState setup = new SetupState(this);
        setup.AddTransition(Transition.FinishedSetup, StateID.Idle);

        fsm = new FSMSystem();
        fsm.AddState(setup);
        fsm.AddState(idle);
        fsm.AddState(follow);
        fsm.AddState(snap);
        fsm.AddState(wrap);
    }

    public void setTile(string currentTileType)
    {
        int[] wallCheck = { 0, 0, 0, 0 };
        GameObject wall;
        GameObject paths = Instantiate(pathOverlay, transform.position, Quaternion.identity, transform);
        GameObject newCorners = Instantiate(corners, transform.position, Quaternion.identity, transform);
        newCorners.transform.localPosition = new Vector2(0, -0.5f);

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
        walls = wallCheck;
        for (int i = 0; i < 4; i++)
        {
            int currentWallBoolean = wallCheck[i];
            if (currentWallBoolean == 1)
            {
                wall = Instantiate(Wall, transform.position, Quaternion.identity, transform);
                // wall.transform.localScale = new Vector3 (.01f, .05f, .1f);
                SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
                offset = wall.GetComponent<Renderer>().bounds.size.x;
                // top wall:
                if (i == 0)
                {
                    sr.sprite = upWall;
                    wall.transform.Translate(new Vector3(0, offset, 0));
                }
                // right wall:
                if (i == 1)
                {
                    sr.sprite = rightWall;
                    wall.transform.Translate(new Vector3(offset, 0.12f, 0));
                    wall.transform.localScale = new Vector3(1, 1.3f, 1);
                    wall.AddComponent<SideWall>();
                }
                // bottom wall:
                if (i == 2)
                {
                    sr.sprite = downWall;
                    wall.transform.Translate(new Vector3(0, offset * -1.75f, 0));
                }
                // left wall:
                if (i == 3)
                {
                    sr.sprite = leftWall;
                    wall.transform.Translate(new Vector3(offset * -1, 0.12f, 0));
                    wall.transform.localScale = new Vector3(1, 1.3f, 1);
                    wall.AddComponent<SideWall>();
                }
            }
        }
    }

    public void setSortingLayer(int layer)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject Go = transform.GetChild(i).gameObject;
            if (Go.CompareTag("WallCorner") || Go.CompareTag("Wall"))
            {
                if (Go.GetComponent<SideWall>() != null)
                {
                    layer++;
                }
                Go.GetComponent<SpriteRenderer>().sortingOrder = layer;
            }
        }
    }
}


public class IdleState : FSMState
{
    TileFSM controlref;

    public IdleState(TileFSM control)
    {
        stateID = StateID.Idle;
        controlref = control;
        controlref.startPos.x = controlref.transform.position.x;
        controlref.startPos.y = controlref.transform.position.y;
        controlref.maxDist = new Vector2(controlref.tileSize, controlref.tileSize);
    }

    public override void DoBeforeEntering()
    {
        controlref.goalPos = controlref.transform.position;
        controlref.startPos.x = controlref.transform.position.x;
        controlref.startPos.y = controlref.transform.position.y;
        controlref.setSortingLayer(-(int)Mathf.Floor(controlref.transform.position.y / controlref.tileSize));
        controlref.netDelta = Vector2.zero;
        //Debug.Log (controlref.maxDist);
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (npc.transform.position.x != controlref.goalPos.x || npc.transform.position.y != controlref.goalPos.y)
        {
            npc.GetComponent<TileFSM>().SetTransition(Transition.UserSwiped); //to follow
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
    }

} // IdleState


public class FollowState : FSMState
{
    TileFSM controlref;

    public FollowState(TileFSM control)
    {
        stateID = StateID.Follow;
        controlref = control;
    }

    public override void DoBeforeLeaving()
    {
        controlref.touchReleased = false;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        //Debug.Log ("Follow: " + controlref.touchReleased);
        if (controlref.touchReleased)
        {
            npc.GetComponent<TileFSM>().SetTransition(Transition.FinishedFollow);
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        
    }

} // FollowState


public class SnappingState : FSMState
{
    TileFSM controlref;
    private float speed = 2.5f;

    public SnappingState(TileFSM control)
    {
        stateID = StateID.Snapping;
        controlref = control;
    }

    public override void DoBeforeLeaving()
    {
        controlref.incompleteMove = false;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (npc.transform.position.x == controlref.goalPos.x && npc.transform.position.y == controlref.goalPos.y)
        {
            if (controlref.incompleteMove)
            {
                controlref.offGrid = false;
            }
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
                npc.GetComponent<TileFSM>().SetTransition(Transition.FinishedSnapping);
            }
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        if (controlref.incompleteMove)
        {
            controlref.goalPos = new Vector2(controlref.startPos.x, controlref.startPos.y);
        }
        npc.transform.position = new Vector2(controlref.goalPos.x, controlref.goalPos.y);
    }

} // SnappingState

public class WrapState : FSMState
{
    TileFSM controlref;

    public WrapState(TileFSM control)
    {
        stateID = StateID.Wrapping;
        controlref = control;
    }

    public override void DoBeforeEntering()
    {
        int sl = -(int)Mathf.Floor(controlref.goalPos.y / controlref.tileSize);
        if (controlref.goalPos.y > controlref.transform.position.y)
        {
            --sl;
        }
        controlref.GetComponent<TileFSM>().setSortingLayer(sl);
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
        npc.transform.position = new Vector2(controlref.goalPos.x, controlref.goalPos.y);      
    }

} // WrapState

public class SetupState : FSMState
{
    public TileFSM controlref;
    private float speed = 3.5f;

    public SetupState(TileFSM control)
    {
        stateID = StateID.Setup;
        controlref = control;
    }

    public override void Reason(GameObject gm, GameObject npc)
    {
        if (controlref.transform.position.x == controlref.goalPos.x && controlref.transform.position.y == controlref.goalPos.y)
        {
            controlref.GetComponent<TileFSM>().SetTransition(Transition.FinishedSetup);
        }
    }

    public override void Act(GameObject gm, GameObject npc)
    {
        controlref.transform.position = new Vector2(controlref.goalPos.x, controlref.goalPos.y);

    }

} //SetupState