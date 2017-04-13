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

	// NOTE: eventually this data will be loaded from save data:
	private bool musicToggle = true;
	private bool sfxToggle = true;
	private bool vibToggle = true;
	private bool[] lockedLevels = {false,false,false,false,false,false};
    private Dictionary<int,int> starRating = new Dictionary<int,int>();
    
	// Make this game object and all its transform children
	// survive when loading a new scene.
	private void Awake () {
		// extract level JSON file here:
		levelData = Camera.main.GetComponent<LevelReader>();
		levelsList = levelData.getLevels();
        for(int i = 0; i< lockedLevels.Length;i++)
        {
            starRating.Add(i + 1, 0);
        }
		DontDestroyOnLoad(this);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
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
		if (index > lockedLevels.Length-1 || index < 0) {
			Debug.Log ("error: index out of range");
		}
		lockedLevels [index] = false;
	}
    public void setStars(int level, int star)
    {
        starRating[level] = star;
    }
	// return the private level int
	public int getLevel(){
		return levelNum;
	}

	// return list of levels
	public List<Level> getLevels(){
		return levelsList;
	}
    // return a star value from dictionary
    public int getStars(int level)
    {
        return starRating[level];
    }
	public bool getLockedLevelBool(int index){
		if (index > lockedLevels.Length-1 || index < 0) {
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

}
