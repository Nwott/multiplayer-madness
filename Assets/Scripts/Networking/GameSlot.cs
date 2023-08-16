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

    private void UpdateRoomID()
    {
        txtRoomID.text = "Room: " + RoomID;
    }

    private void UpdateRegion()
    {
        txtRegion.text = "Region: " + Region;
    }
}
