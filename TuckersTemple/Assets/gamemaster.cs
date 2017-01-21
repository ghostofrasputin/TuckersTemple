using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class gamemaster : MonoBehaviour {
	// public fields:
	public float slideSpeed = .02f;

	// private fields:
	RaycastHit hit;
	private GameObject touchTarget;
	private bool isDrag    = false; //tracks if valid object is hit for drag
	private bool isVert    = false; //Extablishes initial movement axis of swipe
	private bool isLatched = false; //locks movement axis to initial direction of swipe


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0)
		{
			Touch touch = Input.touches[0];

			switch (touch.phase)
			{
			case TouchPhase.Began:
				Ray ray = Camera.main.ScreenPointToRay(touch.position);
				if (Physics.Raycast(ray, out hit))
				{
					touchTarget = hit.collider.gameObject;
					isDrag = true;
				}
				break;

			case TouchPhase.Moved:
				if (isDrag)
				{
					Vector3 delta = touch.deltaPosition;
					if (!isLatched)
					{
						isVert = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);
						isLatched = true;
					}
					if (isVert)
					{
						delta.x = 0;
					}
					else
					{
						delta.y = 0;
					}

					touchTarget.transform.Translate(delta.x * slideSpeed, delta.y * slideSpeed, 0);
				}
				break;

			case TouchPhase.Ended:
				isDrag = false;
				isLatched = false;
				isVert = false;
				break;

			default:
				break;

			}
		}
	}
}