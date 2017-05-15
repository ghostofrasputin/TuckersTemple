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
	private Dictionary<int,List<bool>> starRatings = new Dictionary<int,List<bool>>();
    
	// Make this game object and all its transform children
	// survive when loading a new scene.
	private void Awake () {
		//set the screen orientation.  We do this in ZombiePasser since it exists in
		//every screen. This is untested and needs to be tried in an apk. -Andrew
		//Screen.orientation = ScreenOrientation.Portrait; //never mind we can do this in player settings

		// extract level JSON file here:
		levelData = Camera.main.GetComponent<LevelReader>();
		levelsList = levelData.getLevels();
		saveSystem = Camera.main.GetComponent<SaveSystem>();

		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
	}

	public void Update(){
		if (loaded == false) {
			//printDefaultStructuresToJson (saveSystem, levelsList.Count);
			// Load save data into proper data strutures:
			saveSystem.loadJsonFileList("settings.json",settings);
			saveSystem.loadJsonFileList("lockedLevels.json",lockedLevels);
			saveSystem.loadJsonFileDict("starRatings.json", starRatings);
			loaded = true;
		}

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
	public void setStar(int level, int star, bool starSetting)
    {
		starRatings[level][star] = starSetting;
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
		return starRatings[level];
    }
	public bool getLockedLevelBool(int index){
		if (index > lockedLevels.Count-1 || index < 0) {
			Debug.Log ("error: index out of range");
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
		List<bool> settings = new List<bool> ();
		settings.Add (true);
		settings.Add (true);
		settings.Add (true);
		List<bool> lockedLevels = new List<bool> ();

		// NOTE: key needs to be a string from JsonMapper to work...
		Dictionary<string,List<bool>> starRatings = new Dictionary<string,List<bool>>();

		//Debug.Log (numberOfLevels);
		for (int i = 0; i < numberOfLevels; i++) {
			lockedLevels.Add (false);
			List<bool> levelStars = new List<bool> ();
			for (int j = 0; j < 3; j++) {
				levelStars.Add (false);
			}
			starRatings.Add ((i + 1).ToString(), levelStars);
		}
		saveSys.writeJsonFile("lockedLevels.json", lockedLevels);
		saveSys.writeJsonFile ("starRatings.json", starRatings);
		saveSys.writeJsonFile ("settings.json", settings);
	}

}
