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

class Level {
	public string Name { get; set; }
	public int Rows { get; set; }
	public int Cols { get; set; }
	public List<string> Tiles { get; set; }
	public List<string> Actors { get; set; }
	public int[] Stat { get; set; }
}

public class LevelReader : MonoBehaviour {

	// public:

	// private:
	private string jsonString;
	private JsonData levelData;
	private string iterString = "level";
	private List<Level> levels;

	// Use this for initialization
	void Start () {
		jsonString = File.ReadAllText(Application.dataPath + "/Resources/example.json");
		levelData = JsonMapper.ToObject(jsonString);
		/**for(int i = 1; i < levelData.Count+1; i++) {
			iterString = iterString + i.ToString();
			JsonData levelInfo = levelData[iterString];
			print (levelInfo [0].ToString());
			Level level = new Level {
				name = levelInfo[0],
				rows = levelInfo[1],
				cols = levelInfo[2],
				tiles = levelInfo[3],
				actors = levelInfo[4],
				stat = levelInfo[5],
			};
			levels.Add(level);
			iterString = "level";
		}**/
	}
}

// end of code
