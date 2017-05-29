/*
 * ZombiePasser.cs
 * 
 * Passes information between Unity scenes:
 * Stores level to be started in main
 * Controls musicToggle, sfx, vibration bools
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
class GameData {
	public string settings;
	public string lockedLevels;
	public string starRatings;
}

public class ZombiePasser : MonoBehaviour {

	// public:

	// default string data
	public string settingsString;
	public string lockedLevelsString;
	public string starRatingsString;
	// data to save
	public SaveSystem saveSys;
	public bool musicToggle = true;
	public bool sfxToggle = true;
	public bool vibToggle = true;
	public List<bool> settings = new List<bool> ();
	public List<bool> lockedLevels = new List<bool>();
	public List<List<bool>> starRatings = new List<List<bool>>();

	// private:
	private int levelNum = 1;
    //private int numLevels = 50;
	private LevelReader levelData;
	private List<Level> levelsList;

	//------------------------------------------------------------------------------------------------
	// Singleton
	//------------------------------------------------------------------------------------------------

	// Make this game object and all its transform children
	// survive when loading a new scene.
	private void Awake () {
		//set the screen orientation.  We do this in ZombiePasser since it exists in
		//every screen. This is untested and needs to be tried in an apk. -Andrew
		//Screen.orientation = ScreenOrientation.Portrait; //never mind we can do this in player settings

		// extract level JSON file here:
		levelData = Camera.main.GetComponent<LevelReader>();
		levelsList = levelData.getLevels();

		// Generate Level Icons:
		GameObject levelSelection = GameObject.FindGameObjectWithTag("LevelSelection");
		GameObject IconRef = GameObject.FindGameObjectWithTag("LevelIcon");
		MainMenuManager mainMenu = GameObject.FindGameObjectWithTag ("mainCanvas").GetComponent<MainMenuManager> ();
		int xDiff = 30;
		int yDiff = 40;
		int yOffset = 0;
		int chapterFlag = 0;
		int chapterSeperationSpace = 90;
		int counter = 1;
		for (int i = 0; i < 10; i++) {
			int xOffset = 0;
			for (int j = 0; j < 5; j++) {
				if (!(i == 0 && j == 0)) {
					// create individual level icon with new positions, onclick functions, names.
					Vector3 newPosition = new Vector3 (IconRef.transform.position.x + xOffset, IconRef.transform.position.y+yOffset, IconRef.transform.position.z);
					GameObject newIconRef = Instantiate (IconRef, newPosition, Quaternion.identity, levelSelection.transform);
					newIconRef.name = counter.ToString ();
					int levelParameter = counter;
					newIconRef.GetComponent<Button> ().onClick.RemoveAllListeners ();
					newIconRef.GetComponent<Button>().onClick.AddListener( () => {
						mainMenu.updateLevelNum (levelParameter);
						mainMenu.loadScene("main");
					});
				}
				xOffset += xDiff;
				counter++;
			}
			// control the y offset spacing for levels by chapter
			chapterFlag++;
			if (chapterFlag == 2) {
				chapterFlag = 0;
				yOffset -= chapterSeperationSpace;
			} else {
				yOffset -= yDiff;
			}
			xOffset = xDiff;

		}
		// Binary Serialization Save System:
		//setDefaultData ();
		//Save ();
		Load();

		// keep zombie awake:
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
	}

	//------------------------------------------------------------------------------------------------
	// Set Functions
	//------------------------------------------------------------------------------------------------

	// sets level to be played through button
	// associated with that level
	public void setLevel(int newLevelNum){
		levelNum = newLevelNum;
	}
		
	// music toggle:
	public void setMusicToggle(){
		try {
			Toggle clickedToggle = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
			if(clickedToggle == null) {
				return;
			}
		} catch(System.Exception){
			return;
		}

		if (musicToggle == true) {
			musicToggle = false;
		} else {
			musicToggle = true;
		}
		//print ("set"+musicToggle);
	}
		
	// sfx toggle:
	public void setSFXToggle(){
		try {
			Toggle clickedToggle = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
			if(clickedToggle == null) {
				return;
			}
		} catch(System.Exception){
			return;
		}
		if (sfxToggle == true) {
			sfxToggle = false;
		} else {
			sfxToggle = true;
		}
	}

	// vibration toggle:
	public void setVibToggle(){
		try {
			Toggle clickedToggle = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
			if(clickedToggle == null) {
				return;
			}
		} catch(System.Exception){
			return;
		}
		if (vibToggle == true) {
			vibToggle = false;
		} else {
			vibToggle = true;
		}
	}

	public void setLockedLevelBool(int index){
		if (index > lockedLevels.Count-1 || index < 0) {
			Debug.Log ("error: index out of range");
			return;
		}
		lockedLevels [index] = false;
	}

	// set one of the 3 stars in the list for that level
	public void setStar(int level, int star)
    {
		if (star >= 3 || star < 0) {
			Debug.Log ("error: star array range is 0-2.");
			return;
		}
		//Debug.Log ("Level: " + level + " star number: " + star + " setting: " + starSetting);
		starRatings[level][star] = true;
    }

	//------------------------------------------------------------------------------------------------
	// Get Functions
	//------------------------------------------------------------------------------------------------


	// return the private level int
	public int getLevel(){
		return levelNum;
	}

	// return list of levels
	public List<Level> getLevels(){
		return levelsList;
	}

    // return a list of star values
	public List<bool> getStars(int level)
    {
		return starRatings[level];
    }



	public bool getLockedLevelBool(int index){
		if (index > lockedLevels.Count-1 || index < 0) {
			Debug.Log ("error: index out of range. The index put is: "+index +" in array of size: "+lockedLevels.Count);
		}
		return lockedLevels [index];
	}

	public bool getMusicToggle(){
		return musicToggle;
	}

	public bool getSFXToggle(){
		return sfxToggle;
	}

	public bool getVibToggle(){
		return vibToggle;
	}

	//------------------------------------------------------------------------------------------------
	// Save Data Functions
	//------------------------------------------------------------------------------------------------

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/GameData.dat");

		GameData data = new GameData ();
		data.settings = listToString(settings);
		data.lockedLevels = listToString (lockedLevels);
		data.starRatings = doubleListToString (starRatings);

		bf.Serialize (file, data);
		file.Close ();
	}

	public void Load() {
		if (File.Exists (Application.persistentDataPath + "/GameData.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/GameData.dat", FileMode.Open);

			GameData data = (GameData)bf.Deserialize (file);

			file.Close ();
			settings = listFromString (data.settings);
			lockedLevels = listFromString (data.lockedLevels);
			starRatings = doubleListFromString (data.starRatings);
		} else {
			setDefaultData ();
			Save ();
			Load ();
		}
	}

	public void setDefaultData(){
		settingsString = "ttt";
		lockedLevelsString = "fttttttttttttttttttttttttttttttttttttttttttttttttt";
		starRatingsString = "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
		settings = listFromString(settingsString);
		lockedLevels = listFromString(lockedLevelsString);
		starRatings = doubleListFromString (starRatingsString);
	}

	public string listToString(List<bool> dataList){
		string data = "";

		for (int i = 0; i < dataList.Count; i++) {
			if (dataList [i] == true) {
				data += 't';
			}
			if (dataList [i] == false) {
				data += 'f';
			}
		}
		return data;
	}

	public List<bool> listFromString(string dataString){
		List<bool> data = new List<bool> ();

		for (int i = 0; i < dataString.Length; i++) {
			if (dataString [i].Equals('t')) {
				data.Add(true);
			}
			if (dataString [i].Equals ('f')) {
				data.Add(false);
			}
		}

		return data;
	}

	public string doubleListToString(List<List<bool>> dataList){
		string data = "";

		for (int i = 0; i < dataList.Count; i++) {
			for(int j=0; j< dataList[i].Count; j++){
				List<bool> starList = dataList [i];
				if (starList[j] == true) {
					data += 't';
				}
				if (starList[j] == false) {
					data += 'f';
				}
			}
		}

		return data;
	}

	public List<List<bool>> doubleListFromString(string dataString){
		List<List<bool>> data = new List<List<bool>> ();

		for (int i = 0; i < dataString.Length; i+=3) {

			List<bool> starList = new List<bool> ();
			if (dataString [i].Equals('t')) {
				starList.Add(true);
			}
			if (dataString [i].Equals ('f')) {
				starList.Add(false);
			}
			if (dataString [i+1].Equals('t')) {
				starList.Add(true);
			}
			if (dataString [i+1].Equals ('f')) {
				starList.Add(false);
			}
			if (dataString [i+2].Equals('t')) {
				starList.Add(true);
			}
			if (dataString [i+2].Equals ('f')) {
				starList.Add(false);
			}
			data.Add (starList);
		}

		return data;
	}

}
