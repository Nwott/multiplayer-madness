using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FishNet;

public class NetworkMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField portInputField;

    public void Host()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();

        Setup();
    }

    public void Connect()
    {
        string ip = ipInputField.text;

        if(ip == "")
        {
            ip = "localhost";
        }

        InstanceFinder.ClientManager.StartConnection(ip, ushort.Parse(portInputField.text));

        Setup();
    }

    private void Setup()
    {
        SetUsername();
    }

    private void SetUsername()
    {
        string username = usernameInputField.text;

        if(username == "")
        {
            username = "Player" + Random.Range(1000, 9999);
        }

        PlayerPrefs.SetString("Username", username);
    }
}
