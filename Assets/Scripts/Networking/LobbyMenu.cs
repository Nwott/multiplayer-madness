using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FishNet;

public class LobbyMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HathoraManager hathoraManager;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private TMP_Dropdown regionDropdown;
    [SerializeField] private GameObject gameSlot;
    [SerializeField] private GameObject gameSlotList;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private GameObject noGamesFoundTexts;

    private List<GameObject> gameSlots = new();

    public delegate void CreatedLobby(string lobbyString);
    public delegate void ReceivedLobbies(string lobbyString);
    public delegate void ReceivedConnectionInfo(string info);

    private void Update()
    {
        ShowText();
    }

    public void ShowHostPanel()
    {
        joinPanel.SetActive(false);
        hostPanel.SetActive(true);
        noGamesFoundTexts.SetActive(false);
    }

    public void ShowJoinPanel()
    {
        hostPanel.SetActive(false);
        joinPanel.SetActive(true);
    }

    // shows no game text
    private void ShowText()
    {
        if(gameSlots.Count <= 0)
        {
            if(!noGamesFoundTexts.activeSelf && joinPanel.activeSelf)
            {
                noGamesFoundTexts.SetActive(true);
            }
        }
        else if(gameSlots.Count > 0)
        {
            if(noGamesFoundTexts.activeSelf)
            {
                noGamesFoundTexts.SetActive(false);
            }
        }
    }

    public void HostGame()
    {
        string region = regionDropdown.options[regionDropdown.value].text;
        region = region.Replace(" ", "_");

        CreatedLobby callback = LobbyCreated;

        Prompt.Instance.ShowPrompt("Game is starting. Please wait...");

        hathoraManager.CreateLobby(region, callback);
    }

    private void LobbyCreated(string lobbyString)
    {
        string roomID = lobbyString.Split("roomId\":\"")[1];
        roomID = roomID.Split("\"")[0];
        PlayerPrefs.SetString("RoomID", roomID);

        GameSlot.ReceivedConnectionInfo callback = OnConnectionInfoReceived;
        hathoraManager.GetConnectionInfo(roomID, callback, true);
    }

    public void RefreshJoinMenu() 
    {
        for(int i = 0; i < gameSlots.Count; i++)
        {
            Destroy(gameSlots[i]);
        }

        ReceivedLobbies callback = OnGetPublicLobbies;
        hathoraManager.GetPublicLobbies(callback);
    }

    // gets called when hathora manager finishes getting the public lobbies
    private void OnGetPublicLobbies(string lobbyString)
    {
        string[] lobbyStrList = lobbyString.Split("roomId");

        for(int i = 0; i < lobbyStrList.Length; i++)
        {
            if (!lobbyStrList[i].Contains("region"))
            {
                continue;
            }

            string roomID = lobbyStrList[i].Substring(3);
            string region = roomID.Split("\"")[4];
            roomID = roomID.Split("\"")[0];

            CreateGameSlot(roomID, region);
        }
    }

    private void CreateGameSlot(string roomID, string region)
    {
        GameObject gameSlotGO = Instantiate(gameSlot, transform.position, Quaternion.identity, gameSlotList.transform);
        GameSlot gameSlotScript = gameSlotGO.GetComponent<GameSlot>();
        gameSlotScript.RoomID = roomID;
        gameSlotScript.Region = region;
        gameSlotScript.HathoraManager = hathoraManager;
        gameSlotScript.UsernameInputField = usernameInputField;
        gameSlotScript.LobbyMenu = this;

        gameSlots.Add(gameSlotGO);
    }

    public void OnConnectionInfoReceived(string info, bool isHost)
    {
        string status = info.Split("status\":\"")[1];
        status = status.Split("\"")[0];

        if (status != "active")
        {
            if(isHost)
            {
                LobbyCreated(info);
            }
            else
            {
                Prompt.Instance.ShowPrompt("Server still starting. Try again in a few seconds.");
                print("Server still starting. Try again in a few seconds.");
                return;
            }
        }

        string address = info.Split("host\":\"")[1];
        address = address.Split("\"")[0];
        string port = info.Split("port\":")[1];
        port = port.Split(",")[0];

        Connect(address, port);
    }

    public void Connect(string ip, string port)
    {
        if (ip == "")
        {
            ip = "localhost";
        }

        if (port == "")
        {
            port = "7770";
        }

        InstanceFinder.ClientManager.StartConnection(ip, ushort.Parse(port));

        Setup();
    }

    private void Setup()
    {
        SetUsername();
    }

    private void SetUsername()
    {
        string username = usernameInputField.text;

        if (username == "")
        {
            username = "Player" + Random.Range(1000, 9999);
        }

        PlayerPrefs.SetString("Username", username);
    }
}
