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
		//Debug.Log (saveData.ToString ()); //prints out json format
	}

	// load save data from json file to dictionary: starRatings
	public Dictionary<string,List<bool>> loadJsonFileDict(string filename){
		string pathName = Path.Combine(Application.persistentDataPath,filename);
		jsonString = File.ReadAllText (pathName);
		//Debug.Log(jsonString); 
		saveData = JsonMapper.ToObject(jsonString);
		Dictionary<string,List<bool>> starRatings = new Dictionary<string,List<bool>> ();
		for (int i = 1; i < saveData.Count+1; i++) {
			List<bool> stars = new List<bool> ();
			for (int j = 0; j < saveData [i.ToString ()].Count; j++) {
				bool flag = (bool)saveData [i.ToString ()] [j];
				stars.Add(flag);
			}
			starRatings.Add(i.ToString(),stars);
		}
		return starRatings;
	}

	// load save data from json file to list: settings or lockedLevels
	public List<bool> loadJsonFileList(string filename){
		string pathName = Path.Combine(Application.persistentDataPath,filename);
		jsonString = File.ReadAllText (pathName);
		//Debug.Log(jsonString); 
		saveData = JsonMapper.ToObject(jsonString);
		// build settings or lockedLevels:
		List<bool> dataList = new List<bool>();
		for (int i = 0; i < saveData.Count; i++) {
			bool flag = (bool)saveData [i];
			dataList.Add (flag);
		}
		return dataList;
	}
    
    public bool fileExists(string filename)
    {
        string pathName = Path.Combine(Application.persistentDataPath, filename);

        if (System.IO.File.Exists(pathName))
        {
            return true;
        }
        return false;
    }
}
