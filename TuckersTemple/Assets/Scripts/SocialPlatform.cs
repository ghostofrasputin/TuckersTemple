using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class SocialPlatform : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SignInCallback(bool success)
    {
        if (success)
        {
            Debug.Log("(TuckersTemple) Signed in!");
        }
        else
        {
            Debug.Log("(TuckersTemple) Sign-in failed...");
        }
    }

    public void ShowAchievements()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        } else
        {
            Debug.Log("Cannot Show Achievements, not logged in");
        }
    }
}
