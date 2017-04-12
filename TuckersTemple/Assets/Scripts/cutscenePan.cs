using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cutscenePan : MonoBehaviour {

    Vector3 goalPos;
    float speed;
    public Sprite first;
    public Sprite second;
	// Use this for initialization
	void Start () {
        goalPos = transform.position;
        goalPos.x -= GetComponent<SpriteRenderer>().bounds.size.x;
        speed = 1.2f;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            if (transform.position != goalPos) { transform.position = goalPos; }
            else if(GetComponent<SpriteRenderer>().sprite == first) { GetComponent<SpriteRenderer>().sprite = second; }
            else
            {
                //Destroy(gameObject);
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, goalPos, speed);
    }
}
