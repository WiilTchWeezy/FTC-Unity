using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Records : MonoBehaviour
{
    public List<Text> RecordNames;
    public List<Text> RecordPoints;
    public RawImage img;

    private string a = "";
    // Use this for initialization
    IEnumerator Start()
    {
        foreach (var item in RecordNames)
            item.text = "";
        foreach (var item in RecordPoints)
            item.text = "";

        WWW www = new WWW("http://gamesapi.somee.com/api/game/score");
        yield return www;
        if (www.error == null)
        {
            Processjson(www.text);
            FB.Init();
            FB.API("/me/picture?redirect=false", HttpMethod.GET, ProfilePhotoCallback);
        }
        else
        {
            Debug.Log("ERROR: " + www.error);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Processjson(string jsonString)
    {
        Score[] scores;
        scores = JsonHelper.getJsonArray<Score>(jsonString);
        if (scores != null)
        {
            for (int i = 0; i < scores.Length; i++)
            {
                Debug.Log(scores[i].Name);
                Debug.Log(scores[i].ScorePoint);
                Debug.Log(scores[i].Password);
                RecordNames[i].text = scores[i].Name;
                RecordPoints[i].text = scores[i].ScorePoint.ToString();
                a = scores[i].Password;
            }
        }
    }


    private void ProfilePhotoCallback(IGraphResult result)
    {
        if (String.IsNullOrEmpty(result.Error) && !result.Cancelled)
        {
            IDictionary data = result.ResultDictionary["data"] as IDictionary;
            string photoURL = data["url"] as String;

            StartCoroutine(fetchProfilePic(photoURL));
        }
    }

    private IEnumerator fetchProfilePic(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        img.texture = www.texture;
    }
}

[Serializable]
public class Score
{
    public int ScorePoint;
    public string Name;
    public string Password;
}

public class JsonHelper
{
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

