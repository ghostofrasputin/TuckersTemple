using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterLine {
	public string Image { get; set; }
	public string LineText { get; set; }
}

public class CutScene {
	public string Image { get; set; }
	public List<CharacterLine> Lines { get; set; }
}

public class CutSceneManager : MonoBehaviour {

    Vector3 goalPos;
    float speed;
    public Sprite first;
    public Sprite second;
	public GameObject loadingImage;

	private string jsonString;
	private JsonData cutSceneData;
	private string iterString = "scene";
	private string iterString2 = "line";
	private List<CutScene> cutScenes;
	private int sceneIndex;
	private int lineIndex;

	// Use this for initialization
	void Start () {
		sceneIndex = 0;
		lineIndex = -1;
		TextAsset cutSceneFile = Resources.Load("CutScene1") as TextAsset;
		jsonString = cutSceneFile.ToString();
		cutSceneData = JsonMapper.ToObject(jsonString);
		cutScenes = new List<CutScene> ();
		for(int i = 1; i < cutSceneData.Count+1; i++) {
			JsonData sceneInfo = cutSceneData[iterString + i.ToString()];
			CutScene cutScene = new CutScene {
				Image = (string)sceneInfo [0],
				Lines = new List<CharacterLine>()
			};

			for (int j = 1; j < sceneInfo.Count + 1; j++) {
				JsonData lineInfo = sceneInfo [iterString2 + i.ToString ()];
				CharacterLine line = new CharacterLine {
					Image = (string)lineInfo[0],
					LineText = (string)lineInfo[1]
				};
				cutScene.Lines.Add (line);
			}

			// add level to levels list:
			cutScenes.Add(cutScene);
		}

		transform.Find("Background").GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("CutScenes/" + cutScenes [0].Image);
		transform.Find ("TextBox").GetComponent<CanvasGroup>().alpha = 0f;
		goalPos = transform.Find("Background").transform.position;
        goalPos.x -= 5;
        speed = 0.05f;
    }
	
	// Update is called once per frame
	void Update () {
		bool isTouchBegan = false;
		if (Input.touchCount > 0) {
			isTouchBegan = (Input.GetTouch (0).phase == TouchPhase.Began);
		}
		if (Input.GetMouseButtonDown(0) || isTouchBegan)
        {
			if (transform.Find("Background").transform.position != goalPos) { 
				transform.Find("Background").transform.position = goalPos; 
			}
			else if(lineIndex < cutScenes[sceneIndex].Lines.Count - 1) { 
				nextLine (); 
			}
			else if(sceneIndex < cutScenes.Count - 1) { 
				nextScene (); 
			}
            else
            {
				loadingImage.SetActive (true);
				SceneManager.LoadScene ("main");
            }
        }
		transform.Find("Background").transform.position = Vector2.MoveTowards(transform.Find("Background").transform.position, goalPos, speed);
    }

	void nextScene (){
		sceneIndex++;
		lineIndex = -1;
		GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("CutScenes/" + cutScenes [sceneIndex].Image);
		transform.Find ("TextBox").GetComponent<CanvasGroup>().alpha = 0f;
	}

	void nextLine (){
		if (lineIndex < 0) {
			transform.Find ("TextBox").GetComponent<CanvasGroup>().alpha = 1f;
		}
		lineIndex++;
		transform.Find ("TextBox").transform.Find ("Text").GetComponent<Text> ().text =
			cutScenes [sceneIndex].Lines [lineIndex].LineText;
		transform.Find("TextBox").GetComponent<Image>().sprite = Resources.Load<Sprite> ("CutScenes/" + cutScenes [sceneIndex].Lines [lineIndex].Image);
	}
}
