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

    [Header("Settings")]
    [SerializeField] private ushort port = 7770;

    public void Host()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
    }

    public void Connect()
    {
        string ip = ipInputField.text;

        if(ip == "")
        {
            ip = "localhost";
        }

        InstanceFinder.ClientManager.StartConnection(ip, port);
    }
}
