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
    [SerializeField] private InputField ipInputField;

    public void Host()
    {
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ClientManager.StartConnection();
    }

    public void Connect()
    {
        InstanceFinder.ClientManager.StartConnection(ipInputField.text, )
    }
}
