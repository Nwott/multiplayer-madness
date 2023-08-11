using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class ClientPlayer : NetworkBehaviour
{
    [SyncVar] private string username;
    [SyncVar] private int health;

    private float currentTime; // amount of time in seconds that the player has been alive for
    private float longestTime; // amount of time in seconds that the player has been alive for the longest

    public string Username { get { return username; } }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);

        if(IsOwner)
        {
            Initialize();
        }
    }

    // what happens when the player joins the server
    [ServerRpc]
    private void Initialize()
    {
        username = PlayerPrefs.GetString("Username");

        string message = username + " has joined the game.";
        Debug.Log(message);
        DebugToClients(message);
    }

    [ObserversRpc]
    private void DebugToClients(string message)
    {
        if (IsServer) return;

        Debug.Log(message);
    }

    [ServerRpc]
    public void ChangeHealth(int change)
    {
        health += change;
    }
}
