using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using UnityEngine.Networking;

public class HathoraManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string appID;

    string token;

    private void Start()
    {
        
    }

    public void Login()
    {

    }

    private IEnumerator LoginCoroutine(string region)
    {
        string baseUrl = "https://api.hathora.dev/auth/v1/" + appID + "/login/anonymous";
        string content = "";

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, content, "application/json"))
        {
            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                token = www.downloadHandler.text.Substring(10);
                token = token.Substring(0, token.Length - 2);
                Debug.Log("Token: " + token);

                StartCoroutine(CreateLobbyRequest(region));
            }
        }
    }

    public void CreateLobby(string region)
    {
        StartCoroutine(LoginCoroutine(region));
    }

    private IEnumerator CreateLobbyRequest(string region)
    {
        region = "\"" + region + "\"";
        string baseUrl = "https://api.hathora.dev/lobby/v2/" + appID + "/create";
        string content = "{ \"visibility\": \"public\", \"initialConfig\": { }, \"region\":" + region + " }";

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, content, "application/json"))
        {
            www.SetRequestHeader("Authorization", token);
            //www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                print(www.GetRequestHeader("Authorization"));
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    public void GetPublicLobbies(LobbyMenu.ReceivedLobbies callback)
    {
        StartCoroutine(GetPublicLobbiesRequest(callback));
    }

    private IEnumerator GetPublicLobbiesRequest(LobbyMenu.ReceivedLobbies callback)
    {
        string baseUrl = "https://api.hathora.dev/lobby/v2/" + appID + "/list/public";

        using (UnityWebRequest www = UnityWebRequest.Get(baseUrl))
        {
            yield return www.SendWebRequest();

            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }
    }
}
