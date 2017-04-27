using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {

	private Vector2 dir = Vector2.right;
	private LineRenderer line;
	public bool eyeOpen;
	public RaycastHit2D[] actorRay;
	private Vector3 drawPoint;

	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer> ();
		line.sortingLayerName = "Wall";
		setEye (false);
	}
	
	// Update is called once per frame
	void Update () {
		if(eyeOpen) {
			//Draw the line to there
			line.SetPosition (0, transform.position);
			line.SetPosition (1, drawPoint);
		}
	}

	void fireRayCast(){
		//Shoot a raycast out to the next wall
		RaycastHit2D laserRay = Physics2D.Raycast (transform.position, dir, 100f, LayerMask.GetMask ("Wall"));
		drawPoint = (Vector3)laserRay.point;
		//check if shot any characters
		actorRay = Physics2D.RaycastAll (transform.position, dir, laserRay.distance, LayerMask.GetMask("Character"));

		//check if hit each actor and tell that actor they were hit
		foreach (RaycastHit2D actorHit in actorRay) {
			if (actorHit.transform.CompareTag("Player")) {
				actorHit.transform.gameObject.GetComponent<ActorFSM> ().setLaserHit (true);
			}
		}
	}

	public bool hitByLaser(Transform target){
		if (actorRay != null) {
			foreach (RaycastHit2D actorHit in actorRay) {
				if (actorHit.transform.Equals (target)) {
					return true;
				}
			}
		}
		return false;
	}

	//Called by tile parent to turn the eye on or off if moving
	public void setEye(bool state){
		eyeOpen = state;
		line.enabled = state;
		if (eyeOpen) {
			fireRayCast ();
		} else {
			actorRay = new RaycastHit2D[0];
		}
	}

	//called to set the initial direction
	public void setDir(int direction, float tileSize){
		float offset = tileSize * 4;
		switch (direction) {
		case 0:
			dir = Vector2.up;
			this.transform.localPosition = new Vector2 (0, -offset);
			break;
		case 1:
			dir = Vector2.right;
			this.transform.localPosition = new Vector2 (-offset, 0);
			this.transform.Rotate(new Vector3(0, 0, -90f));
			break;
		case 2:
			dir = Vector2.down;
			this.transform.localPosition = new Vector2 (0, offset);
			this.transform.Rotate(new Vector3(0, 0, 180f));
			break;
		case 3:
			dir = Vector2.left;
			this.transform.localPosition = new Vector2 (offset, 0);
			this.transform.Rotate(new Vector3(0, 0, 90f));
			break;
		}
	}
}
