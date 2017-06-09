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
using GooglePlayGames;

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
            {"TankStandard", "CutScenes/tank"},
            {"EmilyHappy", "CutScenes/emilyHAPPY"},
			{"DadScared", "CutScenes/fatherSCARED"},
			{"DadHappy", "CutScenes/fatherHAPPY"}
        };
		backgrounds = new Dictionary<string, string> () {
			{"templeOutside", "CutScenes/cutscene_Outside"},
            {"templeInfront", "CutScenes/cutscene_1"},
            {"templeStairs", "CutScenes/cutscene_7"},
            {"templeCollapse", "CutScenes/cutscene_9"},
            {"templeInside", "CutScenes/cutscene_Inside"},
            {"foundJake", "CutScenes/cutscene_4"}, 
			{"emilyBegin", "Cutscenes/cutscene_Emily-alone"},
			{"tankBegin", "Cutscenes/cutscene_TankAlone"},
			{"emilyandtank", "Cutscenes/cutscene_EmilyandTank"},
            {"tuckersreunited", "Cutscenes/cutscene_Reunited"},
            {"tankPup", "Cutscenes/cutscene_TankPup"},
            {"tankGrownUp", "Cutscenes/cutscene_Family"},
            {"sadFamily", "Cutscenes/cutscene_SadFamily"},
            {"kidsReading", "Cutscenes/cutscene_Reading"},
            {"kidsGrownUp", "Cutscenes/cutscene_GrownUp"},
            {"familyPictures", "Cutscenes/cutscene_FamilyPictures"},
            {"blackScreen", "Cutscenes/black-screen"},
			{"johnTuckerFigure","Cutscenes/johnTuckerFigure"}
        };
		sounds = new Dictionary<string, string> () {
			{"roy_hmm", "CutScenes/Roy/Roy - Thinking"},
		//	{"roy_notsure", "CutScenes/Andre-ImNotSureAboutThis"},
		//	{"roy_confused", "CutScenes/Andre-WhatWasThat"}, 
			{"roy_relieved", "CutScenes/Roy/Roy - Relieved"},
			{"roy_spooked", "CutScenes/Roy/Roy - Shook"},
			{"roy_unsure", "CutScenes/Roy/Roy - Unsure"},
			{"roy_determined", "CutScenes/Roy/Roy - Determined"},
			{"roy_pumped", "CutScenes/Roy/Roy - EndLevel1"},
			{"emily_determined", "CutScenes/Emily/Emily - Determined"},
			{"emily_disturbed", "CutScenes/Emily/Emily - Disturbed"},
			{"emily_happylaugh", "CutScenes/Emily/Emily - HappyLaugh"},
			{"emily_relief", "CutScenes/Emily/Emily - Relief"},
			{"emily_spooked", "CutScenes/Emily/Emily - Shook"},
			{"emily_thinking", "CutScenes/Emily/Emily - Thinking"},
			{"jake_affirmative", "CutScenes/Jake/Jake - Affirmative"},
			{"jake_confused", "CutScenes/Jake/Jake - Confused"},
			{"jake_scared", "CutScenes/Jake/Jake - Really Scared or Death"},
			{"jake_relief", "CutScenes/Jake/Jake - Relieved"},
			{"jake_spooked", "CutScenes/Jake/Jake - Shook"},
			{"jake_unsure", "CutScenes/Jake/Jake - Unsure"},
			{"tank_cautious", "CutScenes/Tank/Tank - Cautious"},
			{"tank_concerned", "CutScenes/Tank/Tank - Concerned"},
			{"tank_distressed", "CutScenes/Tank/Tank - Distressed"},
			{"tank_happy", "CutScenes/Tank/Tank - Happy2"},
			{"tank_loudroar", "CutScenes/Tank/Tank - Death"},
			{"temple_rumble", "CutScenes/templerumbling"},
			{"temple_collapse", "CutScenes/Temple Falling"}

		};

		cutSceneFiles = new Dictionary<int, string>(){
			{1, "CutScene_Opening"}, //these cutscenes play before the level
            {11, "CutScene_Flashback1"},
            {21, "CutScene_JakeFound"},
			{26, "CutScene_Flashback2"},
            {31, "CutScene_EmilyAlone"},
            {35, "CutScene_TankAlone"},
            {37, "CutScene_EmilyAndTank"},
            {41, "CutScene_TuckerKidsReunited"},
            {51, "CutScene_Conclusion"}
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

        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            if (currLevel == 21)
            {
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_brotherly_love, 100.0f, (bool success) =>
                {
                    Debug.Log("Achievement Incremented: " + success);
                });
            }
            if (currLevel == 31)
            {
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_sister_sister, 100.0f, (bool success) =>
                {
                    Debug.Log("Achievement Incremented: " + success);
                });
            }
            if (currLevel == 35)
            {
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_bear_with_me, 100.0f, (bool success) =>
                {
                    Debug.Log("Achievement Incremented: " + success);
                });
            }
            if (currLevel == 41)
            {
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_family_reunion, 100.0f, (bool success) =>
                {
                    Debug.Log("Achievement Incremented: " + success);
                });
            }
            if (currLevel == 51)
            {
                PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_all_tuckered_out, 100.0f, (bool success) =>
                {
                    Debug.Log("Achievement Incremented: " + success);
                });
            }
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
    public void endCutScene()
    {
        loadingImage.SetActive(true);
        if (GameObject.Find("ZombiePasser").GetComponent<ZombiePasser>().getLevel() > 50)
        {
            SceneManager.LoadScene("mainMenu");
        }
        else
        {
            SceneManager.LoadScene("main");
        }
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
