/* 
 * DialogueWriter.cs
 * 
 * 
 * Writes end of level character dialogue. 
 * 
 **/ 

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class DialogueWriter : MonoBehaviour {

	// public:
	public float delay = 0.1f;
	public string currentText = "";
	public string fullText = "";
	public SoundController sound;
	public AudioClip typewriterSound;
	// private:

	// start
	void Start(){

	}

	// Use this for initialization
	public void writeDialogue (string text) {
		//List<string> levelDialogue = dialogueMap [currentLevel-1];
		//fullText = levelDialogue [0];
		fullText = text;
		currentText = "";
		StopCoroutine ("ShowText");
		StartCoroutine("ShowText");
	}

	public void skipTyping(){
		print ("skipping typing");
		StopCoroutine ("ShowText");
		currentText = fullText;
		this.GetComponent<Text> ().text = fullText;
	}

	IEnumerator ShowText(){
		for(int i = 0; i <= fullText.Length; i++){
			currentText = fullText.Substring(0,i);
			this.GetComponent<Text>().text = currentText;
			sound.PlaySingle (typewriterSound);
			yield return new WaitForSeconds(delay);
		}
	}
}