/* CheckSFX.cs
 * 
 * This script checks if
 * the sfx is on/off to set
 * the object's audio source that 
 * this script is attached to.
 * 
 **/ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSFX : MonoBehaviour {

	// public:
	public AudioSource objectAudioSource;

	// private:
	private bool zombieSFXBool;

	void Start(){
		objectAudioSource = gameObject.GetComponent<AudioSource> ();
	}

	void Update () {
		try {
			zombieSFXBool = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getSFXToggle();
			if(zombieSFXBool == false){
				objectAudioSource.mute = true;
			} else {
				objectAudioSource.mute = false;
			}

		} catch(System.Exception err){
			Debug.Log("CheckSFX error: " + err);
		}
	}

}
