using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public delegate void ReceivedConnectionInfo(string info);

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
        ReceivedConnectionInfo callback = OnConnectionInfoReceived;
        HathoraManager.GetConnectionInfo(RoomID, callback);
    }

    private void OnConnectionInfoReceived(string info)
    {
        //print(info);
        string address = info.Split("host\":\"")[1];
        print(address);
    }
}
