using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

	// public:
	public AudioSource sfxSource;
	public AudioSource musicSource;
	public static SoundController instance = null;

	// pitch variations for a suttle difference
	public float lowPitch = .95f; // - 5%
	public float highPitch = 1.05f; // + 5%

	// private:
	private bool zombieMusicBool = true;
	private bool zombieSFXBool = true;

	void Update(){
		try {
			zombieMusicBool = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getMusicToggle();
			zombieSFXBool = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getMusicToggle();

			// check music
			if(zombieMusicBool==false){
				musicSource.mute = true;
			} else{
				musicSource.mute = false;
			}
			// check sfx
			if(zombieSFXBool==false){
				sfxSource.mute = true;
			} else{
				sfxSource.mute = false;
			}
		}
		catch(System.Exception) {
		}
	}

	// Use this for initialization
	void Awake () {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

	// Play single audio clips 
	public void PlaySingle (AudioClip clip){
		sfxSource.clip = clip;
		sfxSource.Play ();
	}

	// Randomizes between two audio clips so it doesnt sound so repetitive all the time!
	public void RandomSfx (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length); // chooses randoml clip
		float randomPitch = Random.Range (lowPitch, highPitch); // chooses range of pitch

		sfxSource.pitch = randomPitch; // set pitch
		sfxSource.clip = clips [randomIndex]; // set random index from array
		sfxSource.Play (); // play clip!

	}

}