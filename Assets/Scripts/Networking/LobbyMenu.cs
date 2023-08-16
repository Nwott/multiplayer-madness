using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HathoraManager hathoraManager;
    [SerializeField] private GameObject hostPanel;
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private TMP_Dropdown regionDropdown;
    [SerializeField] private GameObject gameSlot;
    [SerializeField] private GameObject gameSlotList;

    private List<GameObject> gameSlots = new();

    public delegate void ReceivedLobbies(string lobbyString);

    public void ShowHostPanel()
    {
        joinPanel.SetActive(false);
        hostPanel.SetActive(true);
    }

    public void ShowJoinPanel()
    {
        hostPanel.SetActive(false);
        joinPanel.SetActive(true);
    }

    public void HostGame()
    {
        string region = regionDropdown.options[regionDropdown.value].text;
        region = region.Replace(" ", "_");
        hathoraManager.CreateLobby(region);
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

        gameSlots.Add(gameSlotGO);
    }
}
