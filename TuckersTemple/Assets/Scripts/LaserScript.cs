using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {

	private Vector2 dir = Vector2.right;
    private Vector2[] dirs = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
	private LineRenderer line;
	public bool eyeOpen;
	private Vector3 drawPoint;
	public GameObject laserHit;
	private GameObject currLaserHit;
    private float promptTimer;

    // Use this for initialization
    void Start () {
		line = GetComponent<LineRenderer> ();
        line.startWidth = 0.02f;
        line.endWidth = 0.07f;
        line.sortingLayerName = "Actor";
		setEye (false);
        promptTimer = 0;
    }
	
	// Update is called once per frame
	void Update () {
        //promptTimer += Time.deltaTime * 2;
        //float scale = Mathf.Abs(Mathf.Sin(promptTimer)) * 0.01f + 0.06f;
    }

	void fireRayCast(){
		//Shoot a raycast out to the next wall
		RaycastHit2D laserRay = Physics2D.Raycast (transform.position, dir, 100f, LayerMask.GetMask ("Wall"));
		drawPoint = laserRay.point;
		if (laserRay.collider.gameObject.tag.Equals ("Wall")) {
            int[] walls = laserRay.collider.transform.parent.GetComponent<TileFSM>().walls;
            //if left wall and facing right, or similar situation, don't apply offset
            bool applyOffset = true;
            for(int i = 0; i < walls.Length; i++)
            {
                if(dir == dirs[i])
                {
                    int checkWall = i + 2;
                    if (checkWall >= 4) checkWall -= 4;
                    if (walls[checkWall] == 1)
                    {
                        applyOffset = false;
                    }

                    if(i == 2) //if a bot wall don't apply the offset
                    {
                        applyOffset = false;
                    }
                }
            }
            if (applyOffset)
            {
                float offset = 3 * laserRay.collider.bounds.size.x / 4;
                drawPoint.x += offset * dir.x;
                drawPoint.y += offset * dir.y;
            }
		}

        RaycastHit2D[] actorRay;
        //check if shot any characters
        actorRay = Physics2D.RaycastAll (transform.position, dir, laserRay.distance, LayerMask.GetMask("Character"));

		//check if hit each actor and tell that actor they were hit
		foreach (RaycastHit2D actorHit in actorRay) {
			if (actorHit.transform.CompareTag("Player"))
            {
				actorHit.transform.gameObject.GetComponent<ActorFSM> ().setLaserHit (true);
			}
		}

        RaycastHit2D[] enemyRay;
        enemyRay = Physics2D.RaycastAll(transform.position, dir, laserRay.distance, LayerMask.GetMask("Collidables"));

        //check if hit each actor and tell that actor they were hit
        foreach (RaycastHit2D enemyHit in enemyRay)
        {
            if (enemyHit.transform.CompareTag("Enemy"))
            {
                enemyHit.transform.gameObject.GetComponent<ActorFSM>().setLaserHit(true);
            }
        }
    }
/*
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
    */
	//Called by tile parent to turn the eye on or off if moving
	public void setEye(bool state){
		eyeOpen = state;
		line.enabled = state;
		GameObject.Destroy (currLaserHit);
        if (eyeOpen)
        {
            fireRayCast();
            currLaserHit = Instantiate(laserHit, drawPoint, Quaternion.identity, this.transform);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, drawPoint);
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
