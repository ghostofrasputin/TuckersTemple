using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

	string[] loadingTexts;
	public float delay = 0.3f;

	// Use this for initialization
	void Start () {
		loadingTexts = new string[] {"Loading", "Loading.", "Loading..", "Loading..."};
		StartCoroutine ("ShowText");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator ShowText(){
		int i = 0;
		while(true){
			this.GetComponent<Text>().text = loadingTexts[i];
			i++;
			if (i >= loadingTexts.Length) {
				i = 0;
			}
			yield return new WaitForSeconds(delay);
		}
	}
}
