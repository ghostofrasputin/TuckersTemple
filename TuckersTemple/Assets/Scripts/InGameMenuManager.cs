using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuManager : MonoBehaviour {

	//publics:
	public GameObject ani;
	public bool pauseFlag;

	// Audio
	public AudioClip InGameMenuSound;

	//private:
	private Animator anim;
	private RectTransform pausePosition;

	// Use this for initialization
	void Start () {
		pauseFlag = false;
		pausePosition = GameObject.FindGameObjectWithTag ("pauseButton").GetComponent<RectTransform> ();
		anim = ani.GetComponent<Animator>();
		anim.enabled = false;
		try {
			/*Toggle music = GameObject.Find("MusicToggle").GetComponent<Toggle>();
			music.isOn = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getMusicToggle();
			Toggle sfx = GameObject.Find("SFXToggle").GetComponent<Toggle>();
			sfx.isOn = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getSFXToggle();*/
		} catch(System.Exception){
			print ("ZombiePasser hasn't been created. Start from mainMenu scene.");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void musicToggle(){
		try {
			GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().setMusicToggle();
		} catch(System.Exception){
		}
	}

	public void sfxToggle(){
		try {
			GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().setSFXToggle();
		} catch(System.Exception){
		}
	}
    
	// load scene by name
	public void loadScene(string scene)
	{
		SceneManager.LoadScene(scene);
		SoundController.instance.PlaySingle (InGameMenuSound);
	}

	//function to pause the game
	public void playAnim(string anima){
		SoundController.instance.PlaySingle (InGameMenuSound);
		anim.enabled = true;
		anim.Play (anima);
	}

	public void pauseAnim(){
		if (GameObject.FindGameObjectWithTag ("UI-Border").GetComponent<Animator> ().enabled == false) {
			GameObject.FindGameObjectWithTag ("UI-Border").GetComponent<Animator> ().enabled = true;
			GameObject.FindGameObjectWithTag ("UI-Border").GetComponent<Animator> ().Play ("UIBorderPause");
			pauseFlag = true;
			pausePosition.transform.position = new Vector3 (1.5f, -1.7f, 90.0f);
			//Debug.Log (pausePosition.transform.position);
		} else {
			if (pauseFlag == true) {
				anim.enabled = true;
				anim.Play ("PauseMenuSlideOut");
				GameObject.FindGameObjectWithTag ("UI-Border").GetComponent<Animator> ().enabled = true;
				GameObject.FindGameObjectWithTag ("UI-Border").GetComponent<Animator> ().Play ("UIBorderPauseW");
				pauseFlag = false;
				pausePosition.transform.position = new Vector3 (1.5f, -3.2f, 90.0f);
			} else {
				anim.enabled = true;
				anim.Play ("PauseMenuSlideIn");
				GameObject.FindGameObjectWithTag ("UI-Border").GetComponent<Animator> ().enabled = true;
				GameObject.FindGameObjectWithTag ("UI-Border").GetComponent<Animator> ().Play ("UIBorderPause");
				pauseFlag = true;
				pausePosition.transform.position = new Vector3 (1.5f, -1.7f, 90.0f);
			}
		}
	}
}
