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
	public List<string> Actors { get; set; }
	public int[] Stat { get; set; }
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
	public List<List<string>> tiles = new List<List<string>> ();

	// FILE PARSING
	void Start () {
		jsonString = File.ReadAllText(Application.dataPath + "/Resources/example.json");
		levelData = JsonMapper.ToObject(jsonString);
		for(int i = 1; i < levelData.Count+1; i++) {
			iterString = iterString + i.ToString();
			JsonData levelInfo = levelData[iterString];
			// get tile arrays into the right type format:
			JsonData tileArray = levelInfo[3];
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
			for (int l = 0; l < actors.Count; l++) {
				
				//print (actors.Keys[0]);
			}
			// debug code for tile list, actors:
			// foreach (List<string> l in tiles) foreach(string str in l) print(str);
			Level level = new Level {
				Name = (string)levelInfo[0],
				Rows = (int)levelInfo[1],
				Cols = (int)levelInfo[2],
				Tiles = tiles//,
				//Actors = levelInfo[4],
				//Stat = levelInfo[5],
			};
			levels.Add(level);
			iterString = "level";
		}
	}

	// lets GameMaster receive 
	// the level list
	public List<Level> getLevels(){
		return levels;
	}


}

// end of code
