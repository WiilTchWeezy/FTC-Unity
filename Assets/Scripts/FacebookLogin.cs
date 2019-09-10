using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class FacebookLogin : MonoBehaviour
{

    private void Awake()
    {
        if (FB.IsInitialized == false)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Erro");
            });
        }
        else
            FB.ActivateApp();
    }

    public void ExecuteLogin()
    {
        var permission = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(permission, (fbResult) =>
        {
            Debug.Log(fbResult.ResultDictionary.ToJson());
            FB.API("/me?fields=id,name,email", HttpMethod.GET, GetFacebookInfo, new Dictionary<string, string>() { });
        });
    }

    public void GetFacebookInfo(IResult result)
    {
        if (result.Error == null)
        {
            Debug.Log(result.ResultDictionary["id"].ToString());
            Debug.Log(result.ResultDictionary["name"].ToString());
            Debug.Log(result.ResultDictionary["email"].ToString());
            PostBestScore(result.ResultDictionary["email"].ToString(), result.ResultDictionary["id"].ToString(), result.ResultDictionary["name"].ToString());
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    private void PostBestScore(string email, string facebookId, string name)
    {
        var request = new GameApiRequest { Email = email, Name = name, Password = facebookId };
        var jsonStrinRequest = JsonUtility.ToJson(request);
        byte[] bytes =  System.Text.Encoding.UTF8.GetBytes(jsonStrinRequest);
        int bestScore = 0;
        bestScore = LoadGameBestScore();
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        WWW www = new WWW("http://gamesapi.somee.com/api/game/user?bestScore=" + bestScore, bytes, headers);
        StartCoroutine(WaitForRequest(www));
    }

    private int LoadGameBestScore()
    {
        if (PlayerPrefs.HasKey("bestScore"))
            return PlayerPrefs.GetInt("bestScore", 0);
        else
            return 0;
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            //Print server response
            Debug.Log(www.text);
            SceneManager.LoadScene(2);
        }
        else
        {
            //Something goes wrong, print the error response
            Debug.Log(www.error);
        }
    }

}

[Serializable]
public class GameApiRequest
{
    public string Email;
    public string Name;
    public string Password;
}
