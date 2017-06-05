/*
 * ZombiePasser.cs
 * 
 * Passes information between Unity scenes:
 * Stores level to be started in main
 * Controls musicToggle, sfx, vibration bools
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ZombiePasser : MonoBehaviour {

	// public:

	// default string data
	public const string settingsString = "ttf";
    public const string lockedLevelsString = "fttttttttttttttttttttttttttttttttttttttttttttttttt";
	public const string unlockedLevelsString="ffffffffffffffffffffffffffffffffffffffffffffffffff";
	public const string starRatingsString = "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";

	// data to save
	public SaveSystem saveSys;
	public bool musicToggle = true;
	public bool sfxToggle = true;
	public bool vibToggle = false;
	public List<bool> settings = new List<bool> ();
	public List<bool> lockedLevels = new List<bool>();
	public List<List<bool>> starRatings = new List<List<bool>>();

	// private:
	private int levelNum = 1;
	private int menuToggle = 0;
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
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Screen.SetResolution(640, 1136, false);
        }

        // extract level JSON file here:
        levelData = Camera.main.GetComponent<LevelReader>();
		levelsList = levelData.getLevels();

		// Generate Level Icons:
		GameObject levelSelection = GameObject.FindGameObjectWithTag("LevelSelection");
		GameObject IconRef = GameObject.FindGameObjectWithTag("LevelIcon");
		MainMenuManager mainMenu = GameObject.FindGameObjectWithTag ("mainCanvas").GetComponent<MainMenuManager> ();
		float scalarX = GameObject.FindGameObjectWithTag ("mainCanvas").GetComponent<RectTransform> ().localScale.x;
		float scalarY = GameObject.FindGameObjectWithTag ("mainCanvas").GetComponent<RectTransform> ().localScale.y;
		float iconWidth = IconRef.GetComponent<RectTransform> ().rect.width;
		//Debug.Log (iconWidth);
		float xDiff = scalarX*(iconWidth*1.05f);
		float yDiff = scalarY*(iconWidth*1.4f);
		float yOffset = 0.0f;
		float chapterFlag = 0.0f;
		float chapterSeperationSpace = scalarY*(iconWidth*3.4f);
		int counter = 1;
		for (int i = 0; i < 10; i++) {
			float xOffset = 0.0f;
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
				chapterFlag = 0.0f;
				yOffset -= chapterSeperationSpace;
			} else {
				yOffset -= yDiff;
			}
			xOffset = xDiff;

		}

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

	public void setMenuToggle(){
		if (menuToggle == 1) {
			menuToggle = 0;
		} else {
			menuToggle = 1;
		}
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

	public int getMenuToggle(){
		return menuToggle;
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
		settings [0] = musicToggle; 
		settings [1] = sfxToggle;
		settings [2] = vibToggle;
		PlayerPrefs.SetString("settings", listToString(settings));
        PlayerPrefs.Save();
        PlayerPrefs.SetString("locked", listToString(lockedLevels));
        PlayerPrefs.Save();
        PlayerPrefs.SetString("stars", matrixToString(starRatings));
        PlayerPrefs.Save();
    }

    public void Load() {
        settings = listFromString(PlayerPrefs.GetString("settings", settingsString));
		musicToggle = settings [0]; 
		sfxToggle = settings [1];
		vibToggle= settings [2];
		// !!!!!!!!!!
		// all levels are set to unlocked right now
		// use the commented out code below to use locked levels again
		lockedLevels = listFromString(unlockedLevelsString); //listFromString(PlayerPrefs.GetString("locked", lockedLevelsString));
        starRatings = matrixFromString(PlayerPrefs.GetString("stars", starRatingsString));
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

	public string matrixToString(List<List<bool>> dataList){
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

	public List<List<bool>> matrixFromString(string dataString){
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
