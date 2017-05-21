using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintArrow : MonoBehaviour {

    private Vector2 goalPos;
    private Vector2 startPos;
    public float speed;
    private float step;
    private float time;
    private SpriteRenderer sr;
    private GameObject gm;

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        gm = GameObject.FindGameObjectWithTag("GameController");
        sr.enabled = false;
        startPos = transform.position;
        goalPos = startPos + new Vector2(0,3.0f);
        speed = .2f;
        step = speed * Time.deltaTime;
        time = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        if(gm.GetComponent<GameMasterFSM>().currentLevel >= 3 || Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            time = 0f;
            sr.enabled = false;
            transform.position = startPos;
        }
        time += Time.deltaTime;
        if (time > 4f)
        {
            sr.enabled = true;
            transform.position = Vector2.MoveTowards(transform.position, goalPos, step);
            if ((Vector2)transform.position == goalPos)
            {
                transform.position = startPos;
            }
        }
    }
}
