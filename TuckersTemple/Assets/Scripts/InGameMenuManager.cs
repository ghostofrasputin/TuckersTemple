using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InGameMenuManager : MonoBehaviour {

	//publics:
	public GameObject ani;

	public AudioClip InGameMenuSound;

	//private:
	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = ani.GetComponent<Animator>();
		anim.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// load scene by name
	public void loadScene(string scene)
	{
		//SoundController.instance.PlaySingle (InGameMenuSound);
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
