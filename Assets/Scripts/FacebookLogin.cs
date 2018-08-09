using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        FB.LogInWithReadPermissions(permission, (fbResult) => {
            Debug.Log(fbResult.ResultDictionary.ToJson());
        });
    }
}
