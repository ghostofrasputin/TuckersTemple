using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class SocialPlatform : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if (UNITY_ANDROID)
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
#endif
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

    public void SignIn()
    {
#if (UNITY_ANDROID)
        PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
#endif
    }

    public void ShowAchievements()
    {
#if (UNITY_ANDROID)

        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            Debug.Log("Cannot Show Achievements, not logged in");
        }
#endif
    }

    public void AchievementProgress(string id, bool incremental, int numInc = 0)
    {
#if (UNITY_ANDROID)
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            if(incremental){
                PlayGamesPlatform.Instance.IncrementAchievement(id, numInc, (bool success) =>
                {
                    Debug.Log("Achievement Incremented: " + success);
                });
            } else {
                PlayGamesPlatform.Instance.ReportProgress(id, 100.0f, (bool success) => {
                    Debug.Log("Achievement Reported: " + success);
                });
            }
        }
#endif
    }
}
