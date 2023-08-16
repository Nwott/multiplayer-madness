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
        string status = info.Split("status\":\"")[1];
        status = status.Split("\"")[0];

        if(status != "active")
        {
            print("Server still starting. Try again in a few seconds.");
            return;
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

        if(port == "")
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
        string username = UsernameInputField.text;

        if (username == "")
        {
            username = "Player" + Random.Range(1000, 9999);
        }

        PlayerPrefs.SetString("Username", username);
    }
}
