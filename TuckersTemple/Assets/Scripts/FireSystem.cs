using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystem : MonoBehaviour {

	private float timeDelay = 1;
	private float onTime = 0;

	private bool isOn = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (isOn) {
			if (Time.time - timeDelay > onTime) {
				isOn = false;
				transform.Find ("Flame").gameObject.SetActive (false);
			}
		}
	}

	public void setOn(){
		isOn = true;
		transform.Find ("Flame").gameObject.SetActive (true);
		onTime = Time.time;
	}
}
