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

	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        startPos = transform.position;
        goalPos = startPos + new Vector2(1.5f,0);
        speed = .5f;
        step = speed * Time.deltaTime;
        time = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            time = 0f;
            sr.enabled = false;
            transform.position = startPos;
        }
        time += Time.deltaTime;
        if (time > 8f)
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
