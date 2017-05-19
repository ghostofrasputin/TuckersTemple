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
	private bool jump = false;

	void Start () {
		panel = GameObject.FindWithTag("controlPan").GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		// jump to level selection:
		if (jump) {
			panel.transform.position = Vector3.MoveTowards(panel.transform.position, new Vector3 (panel.transform.position.x, 1925.0f, panel.transform.position.z), 30.0f);
			if(panel.transform.position.y>=1925){
				jump = false;
			}
		}


		if (Input.touchCount == 0)
		{
			//Calls when mouse is first pressed(begin)
			if (Input.GetMouseButtonDown(0))
			{
				HandleTouch(10, Input.mousePosition, TouchPhase.Began);
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
				HandleTouch(10, Input.mousePosition, TouchPhase.Ended);
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
	public void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase, Vector3 touchDelta = default(Vector3))
	{
		switch (touchPhase) {
		case TouchPhase.Began:
			break;
		case TouchPhase.Moved:
			//if(limits not passed)
				panel.transform.position = new Vector3 (panel.transform.position.x, panel.transform.position.y + touchDelta.y, panel.transform.position.z);
			// }
			//Debug.Log(panel.transform.position);
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

}
