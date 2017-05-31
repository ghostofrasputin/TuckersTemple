/* JiggleLock.cs
 * 
 * attach to lock button in unity
 * 
 * very simple class that stores
 * a lock jiggle toggle to activate
 * a jggle function
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JiggleLock : MonoBehaviour {

	// public:
	public AudioClip jigglelocksound;
	// private:
	private bool jiggleFlag = false;
	private float rotateSpeed;
	private float[] jolts;
	private float tracker;
	private int index = 0;

	public void Start(){
		// jolts must be positive
		jolts = new float[] {20.0f,40.0f,30.0f,10.0f,10.0f,10.0f,5.0f,5.0f,5.0f,5.0f,2.5f,2.5f,2.5f,2.5f};
		rotateSpeed = 7.0f;
		tracker = 0.0f;
	}

	public void Update(){
		if (jiggleFlag == true) {
			float jolt = jolts [index];
			if (tracker <= jolt) {
				jiggleLock ();
				tracker += 4.0f;
			} else {
				index++;
				tracker = 0.0f;
				rotateSpeed = -rotateSpeed;
				// lock is done jolting
				if (index >= jolts.Length) {
					index = 0;
					jiggleFlag = false;
					gameObject.transform.rotation = Quaternion.identity;
				}
			}
		}
	}

	public void setJiggleFlag(){
		jiggleFlag = true;
	}

	private void jiggleLock(){
		gameObject.transform.RotateAround (this.gameObject.transform.position, new Vector3 (0, 0, 1), rotateSpeed);
		SoundController.instance.RandomSfx (jigglelocksound, jigglelocksound);
	}

}
