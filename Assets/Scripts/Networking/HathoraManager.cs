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
        Login();
    }

    public void Login()
    {
        StartCoroutine(LoginCoroutine());
    }

    private IEnumerator LoginCoroutine()
    {
        string baseUrl = "https://api.hathora.dev/auth/v1/" + appID + "/login/anonymous";
        string content = "{ \"appId\": \"" + appID + "\"}";

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
            }
        }
    }

    public void CreateLobby()
    {
        StartCoroutine(CreateLobbyRequest());
    }

    private IEnumerator CreateLobbyRequest()
    {
        string baseUrl = "https://api.hathora.dev/lobby/v2/" + appID + "/create";
        string content = "{ \"visibility\": \"public\", \"initialConfig\": { }, \"region\": \"Washington_DC\" }";

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
}
