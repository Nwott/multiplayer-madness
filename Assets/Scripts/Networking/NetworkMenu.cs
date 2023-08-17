using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FishNet;

public class NetworkMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject devMenu; // menu with host and connect buttons
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject lobbyMenu;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField portInputField;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
        {
            // toggle between dev menu and main menu
            ToggleMenu();
        }
    }

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

    private void ToggleMenu()
    {
        devMenu.SetActive(!devMenu.activeSelf);
        mainMenu.SetActive(!mainMenu.activeSelf);
    }

    public void OpenLobbyMenu()
    {
        devMenu.SetActive(false);
        mainMenu.SetActive(false);
        lobbyMenu.SetActive(true);

        lobbyMenu.GetComponent<LobbyMenu>().RefreshJoinMenu();
    }

    public void CloseLobbyMenu()
    {
        devMenu.SetActive(false);
        lobbyMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
