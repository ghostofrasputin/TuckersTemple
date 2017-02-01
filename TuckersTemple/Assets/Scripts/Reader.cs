/**************************************
 * Reader.cs                          *
 *                                    *
 * reads in json level files          *
 *                                    *
 **************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

public class Reader : MonoBehaviour {
	private string jsonString;
	// Use this for initialization
	void Start () {
		jsonString = File.ReadAllText(Application.dataPath + "/Resources/example.json");
		// Debug.Log (jsonString);


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
