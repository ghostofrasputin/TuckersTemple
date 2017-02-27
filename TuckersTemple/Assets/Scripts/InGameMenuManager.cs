using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InGameMenuManager : MonoBehaviour {

	//publics:
	public GameObject ani;

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
		SceneManager.LoadScene(scene);
	}

	//function to pause the game
	public void playAnim(string anima){
		anim.enabled = true;
		anim.Play (anima);
	}
}
