using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;

public class FacebookScript : MonoBehaviour {

    public Text FriendsText;
    
    
    private void Awake() {
        if (!FB.IsInitialized) {
            FB.Init(() => {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Couldn't initialized facebook");
            },
            isGameShown => {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }
        else {
            FB.ActivateApp();
        }
    }

    #region Login / Logout

    public void FacebookLogin() {
        var permissions = new List<string>() {"public_profile", "email", "user_friends"};
        FB.LogInWithReadPermissions(permissions);
    }
    
    public void FacebookLogout() {
        FB.LogOut();
    }
    #endregion
    

    #region Invites

    public void FacebookGameRequest() {
        FB.AppRequest("Come play CircleGame", title: "CircleGame");
    }
    
    #endregion
    
    
    public void FacebookShare() {
        FB.ShareLink(new System.Uri("https://github.com/austinkobayashi"), "UBC Cheats!", "UBC course stuffs", new System.Uri("https://avatars1.githubusercontent.com/u/14931192?s=460&v=4"));
    }


    public void GetFriendsPlayingThisGame() {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result => {
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>) dictionary["data"];
            FriendsText.text = "";
            foreach (var dict in friendsList)
                FriendsText.text += ((Dictionary<string, object>) dict)["name"];
        });
    }
}
