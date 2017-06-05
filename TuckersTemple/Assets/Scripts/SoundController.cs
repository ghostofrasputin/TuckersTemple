using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

	// public:
	public AudioSource sfxSource;
	public AudioSource musicSource;
	public AudioSource sfxSourceTiles;
	public AudioSource flameOn;
	public AudioSource gameOver;
	public AudioSource roySounds;
	public AudioSource jakeSounds;
	public AudioSource emilySounds;
	public AudioSource tankSounds;
	public AudioSource shadowSounds;
	public AudioSource wraithSounds;

	public static SoundController instance = null;

	// pitch variations for a suttle difference
	public float lowPitch = .95f; // - 5%
	public float highPitch = 1.05f; // + 5%

	// private:
	private bool zombieMusicBool;
	private bool zombieSFXBool;
	private int menuToggleBool;

	void Update(){
		try {
			zombieMusicBool = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getMusicToggle();
			zombieSFXBool = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getSFXToggle();
			menuToggleBool = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getMenuToggle();
			// check music
			if(zombieMusicBool==false){
				musicSource.mute = true;
			} else{
				musicSource.mute = false;
			}
			// check sfx
			if(zombieSFXBool==false){
				sfxSource.mute = true;
				sfxSourceTiles.mute = true;
				if(menuToggleBool!=1){
					flameOn.mute = true;
					roySounds.mute = true;
					emilySounds.mute = true;
					jakeSounds.mute = true;
					tankSounds.mute = true;
					shadowSounds.mute = true;
					wraithSounds.mute = true;
				}
			} else{
				sfxSource.mute = false;
				sfxSourceTiles.mute = false;
				if(menuToggleBool!=1){
					flameOn.mute = false;
					roySounds.mute = false;
					emilySounds.mute = false;
					jakeSounds.mute = false;
					tankSounds.mute = false;
					shadowSounds.mute = false;
					wraithSounds.mute = false;
				}
			}
		} catch(System.Exception err){
			Debug.Log("SoundController error: " +err);
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

		//sfxSourceTiles.clip = clip;
		//sfxSourceTiles.Play ();
	}

	public void PlaySingleGameOver (AudioClip clip){
		gameOver.clip = clip;
		gameOver.Play ();
	}

    // Play single audio clips with delay
    public void PlaySingleDelay(AudioClip clip)
    {
        // !!!!! footsteps need work !!!!!!!!!!
        sfxSource.clip = clip;
        sfxSource.PlayDelayed(.02f);
    }

    // Randomizes between two audio clips so it doesnt sound so repetitive all the time!
    public void RandomSfx (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length); // chooses randoml clip
		//float randomPitch = Random.Range (lowPitch, highPitch); // chooses range of pitch

		//sfxSource.pitch = randomPitch; // set pitch
		sfxSource.clip = clips [randomIndex]; // set random index from array
		sfxSource.Play (); // play clip!

	}

	public void RandomSfxTiles (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length); // chooses randoml clip
		float randomPitch = Random.Range (lowPitch, highPitch); // chooses range of pitch

		sfxSourceTiles.pitch = randomPitch; // set pitch
		sfxSourceTiles.clip = clips [randomIndex]; // set random index from array
		sfxSourceTiles.Play ();

	}

	public void TrapOn (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length); // chooses randoml clip
		float randomPitch = Random.Range (lowPitch, highPitch); // chooses range of pitch

		flameOn.pitch = randomPitch; // set pitch
		flameOn.clip = clips [randomIndex]; // set random index from array
		flameOn.Play ();

	}

	public void RoyVoice (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length); // chooses randoml clip
		float randomPitch = Random.Range (lowPitch, highPitch); // chooses range of pitch

		roySounds.pitch = randomPitch; // set pitch
		roySounds.clip = clips [randomIndex]; // set random index from array
		roySounds.Play ();

	}

	public void JakeVoice (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length); // chooses randoml clip
		float randomPitch = Random.Range (lowPitch, highPitch); // chooses range of pitch

		jakeSounds.pitch = randomPitch; // set pitch
		jakeSounds.clip = clips [randomIndex]; // set random index from array
		jakeSounds.Play ();

	}

	public void EmilyVoice (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length); // chooses randoml clip
		float randomPitch = Random.Range (lowPitch, highPitch); // chooses range of pitch

		emilySounds.pitch = randomPitch; // set pitch
		emilySounds.clip = clips [randomIndex]; // set random index from array
		emilySounds.Play ();

	}

	public void TankVoice (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length); // chooses randoml clip
		float randomPitch = Random.Range (lowPitch, highPitch); // chooses range of pitch

		tankSounds.pitch = randomPitch; // set pitch
		tankSounds.clip = clips [randomIndex]; // set random index from array
		tankSounds.Play ();

	}

	public void ShadowVoice (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length); // chooses randoml clip
		float randomPitch = Random.Range (lowPitch, highPitch); // chooses range of pitch

		shadowSounds.pitch = randomPitch; // set pitch
		shadowSounds.clip = clips [randomIndex]; // set random index from array
		shadowSounds.Play ();

	}

	public void WraithVoice (params AudioClip [] clips){
		int randomIndex = Random.Range (0, clips.Length); // chooses randoml clip
		float randomPitch = Random.Range (lowPitch, highPitch); // chooses range of pitch

		wraithSounds.pitch = randomPitch; // set pitch
		wraithSounds.clip = clips [randomIndex]; // set random index from array
		wraithSounds.Play ();

	}
}