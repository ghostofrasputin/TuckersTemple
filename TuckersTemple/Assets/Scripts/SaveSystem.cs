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
using System.IO;

public class SaveSystem : MonoBehaviour {

	// private:
	private string jsonString;
	private JsonData saveData;

	// write data structure to json file
	public void writeJsonFile(string filename, object dataStruct){
		saveData = JsonMapper.ToJson (dataStruct);
		string pathName = Path.Combine (Application.persistentDataPath, filename);
		File.WriteAllText (pathName, saveData.ToString ());
		//Debug.Log (filename);
	}

	// load save data from json file to dictionary: starRatings
	public Dictionary<int,List<bool>> loadJsonFileDict(string filename, Dictionary<int,List<bool>> starRatings){
		string pathName = Path.Combine(Application.persistentDataPath,filename);
		jsonString = File.ReadAllText (pathName);
		//Debug.Log(jsonString); 
		saveData = JsonMapper.ToObject(jsonString);
		starRatings = new Dictionary<int,List<bool>> ();
		for (int i = 1; i < saveData.Count+1; i++) {
			List<bool> stars = new List<bool> ();
			for (int j = 0; j < saveData [i.ToString ()].Count; j++) {
				bool flag = (bool)saveData [i.ToString ()] [j];
				stars.Add(flag);
			}
			starRatings.Add(i,stars);
		}
		return starRatings;
	}

	// load save data from json file to dictionary: settings or lockedLevels
	public List<bool> loadJsonFileList(string filename, List<bool> dataList){
		string pathName = Path.Combine(Application.persistentDataPath,filename);
		jsonString = File.ReadAllText (pathName);
		//Debug.Log(jsonString); 
		saveData = JsonMapper.ToObject(jsonString);
		// build settings or lockedLevels:
		dataList = new List<bool>();
		for (int i = 0; i < saveData.Count; i++) {
			bool flag = (bool)saveData [i];
			dataList.Add (flag);
		}
		return dataList;
	}

}
