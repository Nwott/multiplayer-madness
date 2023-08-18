using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet;

public class GameSlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI txtRoomID;
    [SerializeField] private TextMeshProUGUI txtRegion;

    private string roomID;
    private string region;

    public string RoomID { get { return roomID; } set { roomID = value; UpdateRoomID(); } }
    public string Region { get { return region; } set { region = value; UpdateRegion(); } }
    public HathoraManager HathoraManager { get; set; }
    public TMP_InputField UsernameInputField { get; set; }
    public LobbyMenu LobbyMenu { get; set; }

    public delegate void ReceivedConnectionInfo(string info, bool isHost);

    private void UpdateRoomID()
    {
        txtRoomID.text = "Room: " + RoomID;
    }

    private void UpdateRegion()
    {
        txtRegion.text = "Region: " + Region;
    }

    public void JoinGame()
    {
        PlayerPrefs.SetString("RoomID", roomID);
        ReceivedConnectionInfo callback = LobbyMenu.OnConnectionInfoReceived;
        HathoraManager.GetConnectionInfo(RoomID, callback, false);
    }
}
