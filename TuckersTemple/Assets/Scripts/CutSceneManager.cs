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
	public AudioClip soundByte { get; set; }
}

public class CutScene {
	public string Image { get; set; }
	public Vector2 StartLocation { get; set; }
	public Vector2 MoveOffset { get; set; }
	public int MoveSpeed { get; set; }
	public List<CharacterLine> Lines { get; set; }
}

public class CutSceneManager : MonoBehaviour {

	//Old variables for Elliot's code
    Vector3 goalPos;
    float speed;
	public AudioSource sound;
	public GameObject background;
	public GameObject textBox;
	public GameObject loadingImage;
	public DialogueWriter dialogueWriter;

	//These hold the JSON file information
	private string jsonString;
	private JsonData cutSceneData;
	//This is what we search for to find scenes and lines
	private string iterString = "scene";
	private string iterString2 = "line";
	//The master list of the cutscene
	private List<CutScene> cutScenes;
	//Variables to keep track of where we are in the cutscene
	private int sceneIndex;
	private int lineIndex;

	Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip> ();
	public AudioClip royHmm;

	// Use this for initialization
	void Start () {
		//fill the sound dictionary
		sounds.Add("roy_hmm", (AudioClip)Resources.Load("CutScenes/Andre-Hmm"));
		sounds.Add("roy_notsure", (AudioClip)Resources.Load("CutScenes/Andre-ImNotSureAboutThis"));
		sounds.Add("roy_confused", (AudioClip)Resources.Load("CutScenes/Andre-WhatWasThat"));
		//intialize GameObjects
		background = transform.Find("Background").gameObject;
		textBox = transform.Find ("TextBox").gameObject;
		loadingImage = transform.Find ("LoadingImage").gameObject;
		dialogueWriter = textBox.transform.Find("Text").GetComponent<DialogueWriter> ();
		sceneIndex = 0;
		lineIndex = -1; //line index gets set to -1 since there is currently no line displayed at all
		//load in the JSON file, the same way as levelReader
		TextAsset cutSceneFile = Resources.Load("CutScene1") as TextAsset;
		jsonString = cutSceneFile.ToString();
		cutSceneData = JsonMapper.ToObject(jsonString);
		cutScenes = new List<CutScene> ();

		//iterate through all the 'scenes', by adding an int to the end of the identifier
		int i = 1;
		while(cutSceneData.Keys.Contains(iterString + i.ToString())) {
			JsonData sceneInfo = cutSceneData[iterString + i.ToString()];
			CutScene cutScene = new CutScene {
				Image = (string)sceneInfo ["image"],
				//StartLocation = (Vector2)sceneInfo["start"],
				//MoveOffset = (Vector2)sceneInfo["offset"],
				//MoveSpeed = (int)sceneInfo["s"],
				Lines = new List<CharacterLine>()
			};

			int j = 1;
			while (sceneInfo.Keys.Contains(iterString2 + j.ToString())) {
				JsonData lineInfo = sceneInfo [iterString2 + j.ToString()];
				CharacterLine line = new CharacterLine {
					Image = (string)lineInfo["image"],
					LineText = (string)lineInfo["text"]
				};
				string soundName = (string)lineInfo ["sound"];
				if(sounds.ContainsKey(soundName)){
					line.soundByte = sounds[soundName];
				}
				cutScene.Lines.Add (line);
				j++;
			}

			// add level to levels list:
			cutScenes.Add(cutScene);
			i++;
		}

		printCutScenes (); //test print
		//set background to first image, and make the text box invisible
		background.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("CutScenes/" + cutScenes [0].Image);
		textBox.GetComponent<CanvasGroup>().alpha = 0f;
		//transforms, speed, etc.  This info should eventually be stored in the json file and read from there.
		goalPos = background.transform.position;
        goalPos.x -= 5;
        speed = 0.05f;
    }
	
	// Update is called once per frame
	void Update () {
		//first we check if the touch screen has just been touched
		bool isTouchBegan = false;
		if (Input.touchCount > 0) {
			isTouchBegan = (Input.GetTouch (0).phase == TouchPhase.Began);
		}
		if (Input.GetMouseButtonDown(0) || isTouchBegan) //or if the mouse was pressed down
        {
			//if the background isn't there yet, move it there.
			//TODO: this should also display the text box, as well as after a second when the move finishes
			if (background.transform.position != goalPos) { 
				background.transform.position = goalPos;
				nextLine ();
			}
			//first check if the current line is done typing
			else if (!dialogueWriter.currentText.Equals (dialogueWriter.fullText)) {
				dialogueWriter.skipTyping ();
			}
			//if the background is in place, check if there are more lines to read
			else if(lineIndex < cutScenes[sceneIndex].Lines.Count - 1) { 
				nextLine (); 
			}
			//if no more lines, check if there is another scene
			else if(sceneIndex < cutScenes.Count - 1) { 
				nextScene (); 
			}
			//otherwise load up the game
            else
            {
				endCutScene ();
            }
        }
		//move the background image along.
		background.transform.position = Vector2.MoveTowards(background.transform.position, goalPos, speed);
    }

	//ends the cut scene and loads in the game
	public void endCutScene(){
		loadingImage.SetActive (true);
		SceneManager.LoadScene ("main");
	}

	//Dubugging tool to see what the current cutscene is
	void printCutScenes(){
		print ("Loaded Cutscene:");
		foreach (CutScene cs in cutScenes) {
			print ("Background: " + cs.Image);
			foreach (CharacterLine cl in cs.Lines) {
				print (cl.Image + ": '" + cl.LineText + "'");
			}
		}
	}

	//goes to the next scene
	void nextScene (){
		sceneIndex++;
		lineIndex = -1; //reset line index
		//load in the new image
		background.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("CutScenes/" + cutScenes [sceneIndex].Image);
		//turn off the text box
		textBox.GetComponent<CanvasGroup>().alpha = 0f;
	}

	//display the next line
	void nextLine (){
		//if this is the first line to be shown, display the text box
		if (lineIndex < 0) {
			textBox.GetComponent<CanvasGroup>().alpha = 1f;
		}
		lineIndex++;
		//set the appropriate text for the text box
		//textBox.transform.Find ("Text").GetComponent<Text> ().text =
			//cutScenes [sceneIndex].Lines [lineIndex].LineText;
		textBox.transform.Find("Text").GetComponent<DialogueWriter>().writeDialogue(cutScenes [sceneIndex].Lines [lineIndex].LineText);
		if (cutScenes [sceneIndex].Lines [lineIndex].soundByte != null) {
			sound.PlayOneShot (cutScenes [sceneIndex].Lines [lineIndex].soundByte);
		}
		//set the appropriate image for the text box
		textBox.GetComponent<Image>().sprite = Resources.Load<Sprite> ("CutScenes/" + cutScenes [sceneIndex].Lines [lineIndex].Image);
	}
}
