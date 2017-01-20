using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {
    // first number how many sets of arrays
    // second number is how many elements per array 
    // so far holding ints, need to make tiles.
    public int[,] array2D = new int[,] { { 0, 0 } };

	// Touch vars
	private GameObject touchTarget;
	private Vector3 objCenter;
	private Vector3 touchPos;
	private Vector3 offset;
	private Vector3 newObjCenter;
	RaycastHit hit;
	private bool isDrag    = false;
	private bool isLatched = false;
	private bool isVert    = false;
	//private float netDrag  = 0f;

	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0)
		{
			Touch touch = Input.touches[0];

			switch (touch.phase)
			{
			case TouchPhase.Began:

				isLatched = false;

				Ray ray = Camera.main.ScreenPointToRay(touch.position);

				if (Physics.Raycast(ray, out hit))
				{
					touchTarget = hit.collider.gameObject;
					objCenter = touchTarget.transform.position;
					touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					offset = touchPos - objCenter;
					isDrag = true;
				}
				break;

			case TouchPhase.Moved:
				if (isDrag)
				{
					touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					newObjCenter = touchPos - offset;

					if (!isLatched && offset != Vector3.zero)
					{
						isVert = Mathf.Abs(offset.y) > Mathf.Abs(offset.x);
						isLatched = true;
						print ("isVert" + isVert);
					}
					if (isVert)
					{
						newObjCenter.x = 0;
					}
					else
					{
						newObjCenter.y = 0;
					}
					touchTarget.transform.position = new Vector3(newObjCenter.x, newObjCenter.y, objCenter.z);
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
