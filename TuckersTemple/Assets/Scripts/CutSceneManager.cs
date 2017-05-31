/***
 * CutSceneManager.cs
 * 
 * Reads in from CutScene json files to create and display cutscenes.
 * 
 * Andrew Cousins
 * Last Modified: 5/31/17
 * Version: 2.0
 ***/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using LitJson;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Class CharacterLine holds information about a single line
 * characterImage is the location of the sprite of the character for this line
 * LineText is the string to display
 * soundByte is the sound effect to play
 */
public class CharacterLine {
	public string characterImage { get; set; }
	public string LineText { get; set; }
	public string soundByte { get; set; }
}

/* Class CutScene holds information about a 'scene'
 * BackgroundImage is location the image for this scene
 * Lines is a list of the CharacterLines, in order, to be shown
 */
public class CutScene {
	public string BackgroundImage { get; set; }
	public List<CharacterLine> Lines { get; set; }
}

public class CutSceneManager : MonoBehaviour {

	//References to objects we need
	public AudioSource sound;
	public GameObject background;
	public GameObject textBox;
	public GameObject loadingImage;
	public DialogueWriter dialogueWriter;
	public GameObject tapPrompt;

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
	private float promptTimer;

	Dictionary<int, string> cutSceneFiles; //holds the list of cutscene jsons

	Dictionary<string, string> sounds;
	Dictionary<string, string> backgrounds;
	Dictionary<string, string> actorImages;

	// Use this for initialization
	void Start () {
		//fill the dictionaries
		actorImages = new Dictionary<string, string> () {
            {"blank", "CutScenes/blank"},
            {"RoyStandard", "CutScenes/royNEUTRAL_cutscene_2"},
            {"RoyDetermined", "CutScenes/royDETERMINEDL_cutscene_5"},
            {"RoyScared", "CutScenes/roySCARED_cutscene_10"},
            {"RoyHappy", "CutScenes/royHAPPY_cutscene_3"},
            {"EmilyCurious", "CutScenes/emilyCURIOUS_cutscene_3"},
            {"EmilyScared", "CutScenes/emilySCARED_cutscene_8"},
            {"JakeScared", "CutScenes/jakeSCAREDcutscene_8"},
            {"JakeCurious", "CutScenes/jakeCURIOUScutscene_4"},
            {"JakeHappy", "CutScenes/jakeHAPPYScutscene_2"},
            {"TankStandard", "CutScenes/tank"}
        };
		backgrounds = new Dictionary<string, string> () {
			{"templeOutside", "CutScenes/temple_outside"},
            {"templeInfront", "CutScenes/cutscene_1"},
            {"templeStairs", "CutScenes/cutscene_7"},
            {"templeCollapse", "CutScenes/cutscene_9"},
            {"templeInside", "CutScenes/template_episode_2"},
            {"foundJake", "CutScenes/cutscene_4"}, 
			{"emilyBegin", "Cutscenes/cutscene_Emily-alone"},
			{"tankBegin", "Cutscenes/cutscene_Tank-alone"},
			{"emilyandtank", "Cutscenes/cutscene_EmilyandTank"}
        };
		sounds = new Dictionary<string, string> () {
			{"roy_hmm", "CutScenes/Andre-Hmm"},
			{"roy_notsure", "CutScenes/Andre-ImNotSureAboutThis"},
			{"roy_confused", "CutScenes/Andre-WhatWasThat"}
		};

		cutSceneFiles = new Dictionary<int, string>(){
			//{1, "CutScene_Opening"}, //these cutscenes play before the level
			//{15, "CutScene_EmilyAlone"},
            //{17, "CutScene_AlmostToJake" }
        };

		//intialize GameObjects
		background = transform.Find("Background").gameObject;
		textBox = transform.Find ("TextBox").gameObject;
		//loadingImage = GameObject.Find ("LoadingImage").gameObject; //set manually
		dialogueWriter = transform.Find("Text").GetComponent<DialogueWriter> ();
		tapPrompt = transform.Find ("TapPrompt").gameObject;
		sceneIndex = 0;
		lineIndex = -1; //line index gets set to -1 since there is currently no line displayed at all
		promptTimer = 0;

		//get the current level
		int currLevel = GameObject.Find("ZombiePasser").GetComponent<ZombiePasser>().getLevel();

		//check if the current level has a cutscene
		if (!cutSceneFiles.ContainsKey (currLevel)) {
			Debug.Log ("No cutscene for level " + currLevel);
			endCutScene ();
            return;
		}

		//load in the JSON file, the same way as levelReader
		TextAsset cutSceneFile = Resources.Load(cutSceneFiles[currLevel]) as TextAsset;
		jsonString = cutSceneFile.ToString();
		cutSceneData = JsonMapper.ToObject(jsonString);
		cutScenes = new List<CutScene> ();

		//iterate through all the 'scenes', by adding an int to the end of the identifier
		int i = 1;
		while(cutSceneData.Keys.Contains(iterString + i.ToString())) {
			JsonData sceneInfo = cutSceneData[iterString + i.ToString()];
			CutScene cutScene = new CutScene {
				BackgroundImage = backgrounds[(string)sceneInfo ["image"]],
				Lines = new List<CharacterLine>()
			};

			int j = 1;
			while (sceneInfo.Keys.Contains(iterString2 + j.ToString())) {
				JsonData lineInfo = sceneInfo [iterString2 + j.ToString()];
				CharacterLine line = new CharacterLine {
					characterImage = actorImages[(string)lineInfo["image"]],
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

		//set background to first image, and make the text box invisible
		background.GetComponent<Image> ().sprite = Resources.Load<Sprite>(cutScenes[0].BackgroundImage);
		textBox.GetComponent<CanvasGroup>().alpha = 0f;
		nextLine ();
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
			//If the textbox isn't displayed, call nextLine
			if (textBox.GetComponent<CanvasGroup>().alpha == 0) { 
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
		if (dialogueWriter.currentText.Equals (dialogueWriter.fullText)) {
			tapPrompt.SetActive (true);
		}
		//blink prompt
		promptTimer += Time.deltaTime * 2;
		tapPrompt.GetComponent<CanvasGroup> ().alpha = (1 + Mathf.Sin (promptTimer)) / 2;
    }

	//ends the cut scene and loads in the game
	public void endCutScene(){
		loadingImage.SetActive (true);
		SceneManager.LoadScene ("main");
	}
		

	//goes to the next scene
	void nextScene (){
		sceneIndex++;
		lineIndex = -1; //reset line index
		//load in the new image
		background.GetComponent<Image> ().sprite = Resources.Load<Sprite>(cutScenes [sceneIndex].BackgroundImage);
		//turn off the text box
		textBox.GetComponent<CanvasGroup>().alpha = 0f;
		tapPrompt.SetActive (true);
        transform.Find("Text").GetComponent<Text>().text = "";
		nextLine ();
    }

	//display the next line
	void nextLine (){
		tapPrompt.SetActive (false);
		//if this is the first line to be shown, display the text box
		if (lineIndex < 0) {
			textBox.GetComponent<CanvasGroup>().alpha = 1f;
		}
		lineIndex++;
		//set the appropriate text for the text box
		//textBox.transform.Find ("Text").GetComponent<Text> ().text =
			//cutScenes [sceneIndex].Lines [lineIndex].LineText;
		transform.Find("Text").GetComponent<DialogueWriter>().writeDialogue(cutScenes [sceneIndex].Lines [lineIndex].LineText);
		if (cutScenes [sceneIndex].Lines [lineIndex].soundByte != null) {
			sound.PlayOneShot (Resources.Load<AudioClip>(cutScenes [sceneIndex].Lines [lineIndex].soundByte));
		}
		//set the appropriate image for the text box
		textBox.GetComponent<Image>().sprite = Resources.Load<Sprite>(cutScenes [sceneIndex].Lines [lineIndex].characterImage);
	}
}
