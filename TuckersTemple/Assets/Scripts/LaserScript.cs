using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {

	private Vector2 dir = Vector2.right;
	private LineRenderer line;
	public bool eyeOpen;
	public RaycastHit2D[] actorRay;

	// Use this for initialization
	void Start () {
		line = GetComponent<LineRenderer> ();
		line.sortingLayerName = "Wall";
		setEye (false);
	}
	
	// Update is called once per frame
	void Update () {
		if(eyeOpen) {
			//Shoot a raycast out to the next wall
			RaycastHit2D laserRay = Physics2D.Raycast (transform.position, dir, 100f, LayerMask.GetMask ("Wall"));

			//Draw the line to there
			line.SetPosition (0, transform.position);
			line.SetPosition (1, laserRay.point);

			//check if shot any characters
			actorRay = Physics2D.RaycastAll (transform.position, dir, laserRay.distance, LayerMask.GetMask("Character"));
			/*
			foreach (RaycastHit2D actorHit in actorRay) {
				if(actorHit.collider.CompareTag("Player")){
					GameObject.FindWithTag ("GameController").GetComponent<GameMasterFSM> ().SetTransition (Transition.ActorDied);
					GameObject.Find("DeathText").GetComponent<UnityEngine.UI.Text>().text = 
						actorHit.collider.GetComponent<ActorFSM>().actorName + " stepped in a laser.";
				}
			}*/
		}
	}

	//Called by tile parent to turn the eye on or off if moving
	public void setEye(bool state){
		eyeOpen = state;
		line.enabled = state;
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
