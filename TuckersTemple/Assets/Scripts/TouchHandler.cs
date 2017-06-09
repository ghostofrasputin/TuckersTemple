/*
 * TouchHandler.cs
 * 
 * Handles touch input for just
 * the main menu and level selection
 * screen on the mainmenu scene.
 * 
 * note: attatched to main camera
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHandler : MonoBehaviour {

	// public:
	public Vector3 lastPos;

	// private:
	private RectTransform panel;

	private float startPos;
	private float endPos;
	private float heightOfMainImage;
	private float levelScrollLimit;
	private bool jump = false;
	private GameObject top;
	private GameObject bottom;
	private GameObject canvas;
	private GameObject levelAnchor;
	float diff;
	float scalarX;


	void Start () {
		panel = GameObject.FindWithTag("controlPan").GetComponent<RectTransform>();
		heightOfMainImage = GameObject.FindWithTag ("MainImage").GetComponent<RectTransform> ().rect.height;
		startPos = panel.transform.position.y;
		endPos = panel.transform.position.y + heightOfMainImage*2;
		//levelScrollLimit = startPos + heightOfMainImage - 25; // 25 is the offset

		top = GameObject.FindWithTag ("TopAnchor");
		bottom = GameObject.FindWithTag ("BottomAnchor");
		canvas = GameObject.FindWithTag ("mainCanvas");
		levelAnchor = GameObject.FindWithTag ("LevelAnchor");
		diff = levelAnchor.GetComponent<RectTransform> ().rect.width;
		scalarX = GameObject.FindGameObjectWithTag ("mainCanvas").GetComponent<RectTransform> ().localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (scalarX);
		// jump to level selection:
		if (jump) {
			if (checkCollision (levelAnchor)) {
				jump = false;
			} else {
                panel.transform.position = Vector3.MoveTowards(panel.transform.position, new Vector3(panel.transform.position.x, panel.transform.position.y * (scalarX * 10), panel.transform.position.z), scalarX* 5f);
			}
		} 


		if (Input.touchCount == 0)
		{
			//Calls when mouse is first pressed(begin)
			if (Input.GetMouseButtonDown(0))
			{
				HandleTouch(10, Input.mousePosition, TouchPhase.Began, Vector3.zero);
				//store the last position for next tick
				lastPos = Input.mousePosition;
			}
			//called when mouse his held down(moved)
			if (Input.GetMouseButton(0))
			{
				HandleTouch(10, Input.mousePosition, TouchPhase.Moved, Input.mousePosition - lastPos);
				lastPos = Input.mousePosition;
			}
			//called when mouse is lifted up(ended)
			if (Input.GetMouseButtonUp(0))
			{
				HandleTouch(10, Input.mousePosition, TouchPhase.Ended, Vector3.zero);
			}
		}
		else
		{
			//use the first touch registered
			Touch touch = Input.touches[0];
			HandleTouch(touch.fingerId, touch.position, touch.phase, touch.deltaPosition);
		}
	}

	// move panel up and down
	public void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase, Vector3 touchDelta)
	{
		switch (touchPhase) {
		case TouchPhase.Began:
			break;
		case TouchPhase.Moved:
			float currentPos = panel.transform.position.y + touchDelta.y;

			if (panel.transform.position.y < currentPos && !checkCollision (bottom)) {
				panel.transform.position = new Vector3 (panel.transform.position.x, currentPos, panel.transform.position.z);
				return;
			}
			if (panel.transform.position.y > currentPos && !checkCollision (top)) {
				panel.transform.position = new Vector3 (panel.transform.position.x, currentPos, panel.transform.position.z);
				return;
			}

			break;
		case TouchPhase.Ended:
			break;
		default:
			break;
		}
	}

	// used by level jump button to jump to level 
	// selection screen
	public void jumpToLevelSelection(){
		jump = true;
	}

	private bool checkCollision(GameObject anchor1){
		Vector3 rect1 = anchor1.transform.position;
		Rect rect2 = canvas.GetComponent<Canvas> ().pixelRect;
		bool x = (rect1.x < rect2.x + rect2.width && rect1.x + 1 > rect2.x && rect1.y < rect2.y + rect2.height && 1 + rect1.y > rect2.y);
		//Debug.Log (x);
		return x;
	}

}
