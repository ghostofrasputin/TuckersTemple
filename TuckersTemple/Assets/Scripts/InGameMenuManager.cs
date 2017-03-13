using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuManager : MonoBehaviour {

	//publics:
	public GameObject ani;

	// Audio
	public AudioClip InGameMenuSound;

	//private:
	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = ani.GetComponent<Animator>();
		anim.enabled = false;

		Toggle music = GameObject.Find("MusicToggle").GetComponent<Toggle>();
		music.isOn = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getMusicToggle();
		Toggle sfx = GameObject.Find("SFXToggle").GetComponent<Toggle>();
		sfx.isOn = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getSFXToggle();

		//Toggle vibration = GameObject.Find("VibToggle").GetComponent<Toggle>();
		//vibration.isOn = GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().getVibToggle();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void musicToggle(){
		try {
			GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().setMusicToggle();
		} catch(System.Exception){}
	}

	public void sfxToggle(){
		try {
			GameObject.FindGameObjectWithTag("Zombie").GetComponent<ZombiePasser>().setSFXToggle();
		} catch(System.Exception){}
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
}
