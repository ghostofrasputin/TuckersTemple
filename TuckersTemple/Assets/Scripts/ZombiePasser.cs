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

public class ZombiePasser : MonoBehaviour {

	// public:

	// private:
	private int levelNum = 1;
	private LevelReader levelData;
	private List<Level> levelsList;

	// ALL Save Data for the game:
	bool loaded = false;
	SaveSystem saveSystem;
	// settings data:
	List<bool> settings = new List<bool> ();
	private bool musicToggle = true;
	private bool sfxToggle = true;
	private bool vibToggle = true;
	// locked level data:
	private List<bool> lockedLevels = new List<bool>();
	// star ratings for eaach level data:
	private Dictionary<string,List<bool>> starRatings = new Dictionary<string,List<bool>>();
    
	// Make this game object and all its transform children
	// survive when loading a new scene.
	private void Awake () {
		//set the screen orientation.  We do this in ZombiePasser since it exists in
		//every screen. This is untested and needs to be tried in an apk. -Andrew
		//Screen.orientation = ScreenOrientation.Portrait; //never mind we can do this in player settings

		// extract level JSON file here:
		levelData = Camera.main.GetComponent<LevelReader>();
		levelsList = levelData.getLevels();

		// SAVE SYSTEM:
		saveSystem = Camera.main.GetComponent<SaveSystem>();
		//printDefaultStructuresToJson (saveSystem, 17);
		// Load save data into proper data strutures:
		settings = saveSystem.loadJsonFileList("settings.json");
		musicToggle = settings[0];
		sfxToggle = settings[1];
		vibToggle = settings[2];
		lockedLevels = saveSystem.loadJsonFileList("lockedLevels.json");
		starRatings = saveSystem.loadJsonFileDict("starRatings.json");

		// keep zombie awake:
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
	}

	// saves the game by writing to json files
	public void saveGame(){
		saveSystem.writeJsonFile("lockedLevels.json", lockedLevels);
		saveSystem.writeJsonFile("starRatings.json", starRatings);
		saveSystem.writeJsonFile("settings.json", settings);
	}

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
	public void setStar(int level, int star, bool starSetting)
    {
		if (star >= 3 || star < 0) {
			Debug.Log ("error: star array range is 0-2.");
			return;
		}
		//Debug.Log ("Level: " + level + " star number: " + star + " setting: " + starSetting);
		string lev = level.ToString ();
		starRatings[lev][star] = starSetting;
    }

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
		string lev = level.ToString ();
		return starRatings[lev];
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

	// this is a one time needed helper
	// function to print out the initial state
	// of the save data. after these files are created
	// this function won't be needed, but will be left in
	// to create files if needed.
	public void printDefaultStructuresToJson(SaveSystem saveSys, int numberOfLevels){
		List<bool> s = new List<bool> ();
		s.Add (true);
		s.Add (true);
		s.Add (true);
		List<bool> ll = new List<bool> ();

		// NOTE: key needs to be a string from JsonMapper to work...
		Dictionary<string,List<bool>> sr = new Dictionary<string,List<bool>>();

		//Debug.Log (numberOfLevels);
		for (int i = 0; i < numberOfLevels; i++) {
			if (i == 0) {
				ll.Add (false);
			} else {
				ll.Add (true);
			}

			List<bool> levelStars = new List<bool> ();
			for (int j = 0; j < 3; j++) {
				levelStars.Add (true);
			}
			sr.Add ((i + 1).ToString(), levelStars);
		}
		saveSys.writeJsonFile("lockedLevels.json", ll);
		saveSys.writeJsonFile ("starRatings.json", sr);
		saveSys.writeJsonFile ("settings.json", s);
	}

}
