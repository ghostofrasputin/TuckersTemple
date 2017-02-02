/**************************************
 * Reader.cs                          *
 *                                    *
 * reads in json levels file into     *
 * a list of level objects            *
 *                                    *
 **************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;

//---------------------------------------------------------
// Level class - holds level data
//---------------------------------------------------------
public class Level {
	public string Name { get; set; }
	public int Rows { get; set; }
	public int Cols { get; set; }
	public List<List<string>> Tiles { get; set; }
	public Dictionary<string,List<int>> Actors { get; set; }
	public Dictionary<string,List<int>> StaticObjects { get; set; } // traps, goals, etc.
}

//---------------------------------------------------------
// LevelReader class - handles file reading and parsing
//---------------------------------------------------------
public class LevelReader : MonoBehaviour {

	// private:
	private string jsonString;
	private JsonData levelData;
	private string iterString = "level";
	private List<Level> levels = new List<Level>();
	private List<List<string>> tiles; 
	private Dictionary<string,List<int>> acts;
	private Dictionary<string,List<int>> stats; 

	// FILE PARSING
	void Start () {
		jsonString = File.ReadAllText(Application.dataPath + "/Resources/levels.json");
		levelData = JsonMapper.ToObject(jsonString);
		for(int i = 1; i < levelData.Count+1; i++) {
			iterString = iterString + i.ToString();
			JsonData levelInfo = levelData[iterString];

			// get tile arrays into the right type format:
			JsonData tileArray = levelInfo[3];
			// new List for each level
			tiles = new List<List<string>> ();
			for(int j=0; j<tileArray.Count; j++){
				List<string> row = new List<string> ();
				for (int k = 0; k < tileArray[j].Count; k++) {
					row.Add((string)tileArray[j][k]);
				}
				tiles.Add (row);
			}

			// get character data into the right type format:
			JsonData actors = levelInfo [4];
			string[] keys = new string[actors.Count];
			actors.Keys.CopyTo(keys, 0);
			// new Dict for each level
			acts = new Dictionary<string, List<int>>();
			for (int l = 0; l < keys.Length; l++) {
				string key = keys[l];
				List<int> actorInfo = new List<int> (); 
				for (int m = 0; m < actors[key].Count; m++) {
					int num = (int)actors[key][m];
					actorInfo.Add (num);
				}
				acts.Add(key,actorInfo);
			}

			// get static object data into the right type format:
			JsonData staticO = levelInfo [5];
			string[] skeys = new string[staticO.Count];
			staticO.Keys.CopyTo(skeys, 0);
			// new Dict for each level
			stats = new Dictionary<string, List<int>>();
			for (int n = 0; n < skeys.Length; n++) {
				string key = skeys[n];
				List<int> staticObjectInfo = new List<int> (); 
				for (int o = 0; o < staticO[key].Count; o++) {
					int num = (int)staticO[key][o];
					staticObjectInfo.Add (num);
				}
				stats.Add(key,staticObjectInfo);
			}

			// Create a new level:
			Level level = new Level {
				Name = (string)levelInfo[0],
				Rows = (int)levelInfo[1],
				Cols = (int)levelInfo[2],
				Tiles = tiles,
				Actors = acts,
				StaticObjects = stats
			};

			// add level to levels list:
			levels.Add(level);
			iterString = "level";
		}

		// DEBUGGING:
		printLevelsList();
		printLevel(1);
	}

	// let's GameMaster receive 
	// the level list. giggity.
	public List<Level> getLevels(){
		return levels;
	}

	// DEBUGGING TOOLS BELOW --------------------------

	// prints out each level from
 	// the level list for debugging
	// purposes
	public void printLevelsList(){
		foreach (Level level in levels) {
			Debug.Log("Level Name: " + level.Name);
			Debug.Log("Level Rows: " + level.Rows);
			Debug.Log("Level Cols: " + level.Cols);
			string tileString = "";
			foreach (List<string> l in tiles) {
				tileString = tileString + "[";
				foreach (string str in l) {
					tileString= tileString+ " " + str;
				}
				tileString = tileString + " ]";
			}
			Debug.Log("Level Tiles: " + tileString);
			Debug.Log("Level Actors:");
			printDict (level.Actors);
			Debug.Log("Level Static Objects:");
			printDict (level.StaticObjects);
		}
	}

	// same as before, but the user can select which
	// level they wish to see the contents of
	public void printLevel(int index){
		// in case someone puts in 
		// a dumb level, like level -3 
		if ((index-1) < 0 || (index-1)>=levels.Count) {
			Debug.Log ("Level " + index + " does not exist");
			return;
		}
		Level level = levels[index-1];
		Debug.Log("Level Name: " + level.Name);
		Debug.Log("Level Rows: " + level.Rows);
		Debug.Log("Level Cols: " + level.Cols);
		string tileString = "";
		foreach (List<string> l in tiles) {
			tileString = tileString + "[";
			foreach (string str in l) {
				tileString= tileString+ " " + str;
			}
			tileString = tileString + " ]";
		}
		Debug.Log("Level Tiles: " + tileString);
		Debug.Log("Level Actors:");
		printDict (level.Actors);
		Debug.Log("Level Static Objects:");
		printDict (level.StaticObjects);
	}

	// private resuable function for printing out
	// character and static object data
	private void printDict(Dictionary<string,List<int>> dict){
		foreach (KeyValuePair<string, List<int>> kvp in dict) {
			string actorString = "    ";
			string key = kvp.Key;
			actorString = actorString + key + ":  [";
			List<int> value = kvp.Value;
			foreach (int num in value) {
				actorString = actorString + " " + num;
			}
			actorString = actorString +" ]";
			Debug.Log(actorString);
		}
	}

}

// end of code
