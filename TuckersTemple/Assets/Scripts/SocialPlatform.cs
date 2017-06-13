using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class SocialPlatform : MonoBehaviour {

	Dictionary<string, string> iosAchievements = new Dictionary<string, string>(){ 
		{GPGSIds.achievement_all_star, "all_star"},
		{GPGSIds.achievement_all_tuckered_out, "all_tuckered_out"},
		{GPGSIds.achievement_bear_with_me, "bear_with_me"},
		{GPGSIds.achievement_brotherly_love, "brotherly_love"},
		{GPGSIds.achievement_family_reunion, "family_reunion"},
		{GPGSIds.achievement_getting_started, "getting_started"},
		{GPGSIds.achievement_going_bearzerk, "going_bearzerk"},
		{GPGSIds.achievement_gr8_b8_m8, "great_bait_mate"},
		{GPGSIds.achievement_hello_darkness, "hello_darkness"},
		{GPGSIds.achievement_its_a_trap, "its_a_trap"},
		{GPGSIds.achievement_lasers_and_feelings, "lasers_and_feelings"},
		{GPGSIds.achievement_legend_of_the_hidden_temple, "legend_hidden_temple"},
		{GPGSIds.achievement_mulligan, "mulligan_"},
		{GPGSIds.achievement_sister_sister, "sister_sister"},
		{GPGSIds.achievement_temple_of_doom, "temple_doom"},
		{GPGSIds.achievement_these_belong_in_a_museum, "belong_in_museum"}
	};

	// Use this for initialization
	void Start () {
/*#if (UNITY_ANDROID)
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
#endif*/

#if (UNITY_IPHONE)
		GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
		//Social.localUser.Authenticate(SignInCallback);
#endif
		Social.localUser.Authenticate (SignInCallback);
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
/*#if (UNITY_ANDROID)

        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            Debug.Log("Cannot Show Achievements, not logged in");
        }
#endif*/
#if (UNITY_IPHONE)
		if(Social.localUser.authenticated){
			Social.ShowAchievementsUI();
		}
#endif

    }

    public void AchievementProgress(string id, bool incremental, int numInc = 0)
    {
/*#if (UNITY_ANDROID)
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
#endif*/

#if (UNITY_IPHONE)
		if(Social.localUser.authenticated){
			try {
				if(incremental){
					Social.ReportProgress(iosAchievements[id], numInc, (bool success) =>
						{
							Debug.Log("Achievement Incremented: " + success);
						});
				} else {
					Social.ReportProgress(iosAchievements[id], 100.0f, (bool success) => {
						Debug.Log("Achievement Reported: " + success);
					});
				}
			}
			catch {
				Debug.Log("Achievement Failed to report: " + id);
			}
		}
#endif

    }
}
