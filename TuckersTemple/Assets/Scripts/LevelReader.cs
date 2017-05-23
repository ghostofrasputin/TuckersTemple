/**************************************
 * LevelReader.cs                     *
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
    public int Moves { get; set; }
    public string Star { get; set; }
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
        TextAsset levelFile = Resources.Load("betaLevels") as TextAsset;
		jsonString = levelFile.ToString();
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
			acts = new Dictionary<string,List<int>> ();
			setDict(actors, acts);

			// get static object data into the right type format:
			JsonData staticO = levelInfo [5];
			stats = new Dictionary<string,List<int>> ();
			setDict(staticO, stats);

            //get star info
            int numMoves = 4; //default number of moves to 4 if none is specified.
            if (levelInfo.Keys.Contains("moves"))
            {
                numMoves = (int)levelInfo["moves"];
            }
            string starCriteria = ""; //if no special requirement, the star will be given for free.
            if (levelInfo.Keys.Contains("star"))
            {
                starCriteria = (string)levelInfo["star"];
            }

            // Create a new level:
            Level level = new Level {
                Name = (string)levelInfo[0],
                Rows = (int)levelInfo[1],
                Cols = (int)levelInfo[2],
                Moves = numMoves,
                Star = starCriteria,
				Tiles = tiles,
				Actors = acts,
				StaticObjects = stats
			};

			// add level to levels list:
			levels.Add(level);
			iterString = "level";
		}

		// DEBUGGING:
		//printLevelsList();
		//printLevel(1);
	}

	// let's GameMaster receive 
	// the level list. giggity.
	public List<Level> getLevels(){
		return levels;
	}

	// sets dictionaries for actors and static objects
	// yeee reusable code!!!
	private void setDict(JsonData data, Dictionary<string, List<int>> objectList){
		string[] keys = new string[data.Count];
		data.Keys.CopyTo(keys, 0);
		// new Dict for each level
		for (int l = 0; l < keys.Length; l++) {
			string key = keys[l];
			// numOfObjectTypes example: 1 roy, 2 shadows = 2 objects
			//                           1 goal, 5 traps = 2 objects
			int numOfObjectTypes = data[key].Count;
			for (int m = 0; m < numOfObjectTypes; m++) {
				List<int> objectInfo = new List<int> ();
				for (int inner= 0; inner < data[key][m].Count; inner++) {
					int num = (int)data[key][m][inner];
					objectInfo.Add (num);
				}
				// duplicate actors need their 
				// own specific key:
				string tempKey = key;
				if (numOfObjectTypes > 1) {
					key = key + m.ToString ();
				}
				objectList.Add(key,objectInfo);
				key = tempKey;
			}
		}
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
			foreach (List<string> l in level.Tiles) {
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
		foreach (List<string> l in level.Tiles) {
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

