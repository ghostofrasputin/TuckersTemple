/***************************************
 * SaveSystem.cs                       *
 *                                     *
 * saves data to json, loads data from *
 * json to zombiepasser on app startup *
 *                                     *
 ***************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class SaveSystem : MonoBehaviour {

	// private:
	private string fileName = "saveData";

	private string jsonString;
	private JsonData saveData;
	private List<bool> lockedLevels = new List<bool> ();
	private Dictionary<int,int> starRating = new Dictionary<int,int>();

	// loads saved data on start up
	void Start () {
		TextAsset saveFile = Resources.Load("saveData") as TextAsset;
		// there's no save data to load,
		// write default data (everything locked)
		if (saveFile == null) {
			Debug.Log ("Writing default JSON with everything locked.");
			writeDefaultJSON (saveFile);
		} else {
			jsonString = saveFile.ToString ();
			saveData = JsonMapper.ToObject (jsonString);
		}
	}

	// app opened for first time, everything is locked on startup,
	// this function creates the default json file from a blank json
	// file
	private void writeDefaultJSON(TextAsset file){
		
	}

}
