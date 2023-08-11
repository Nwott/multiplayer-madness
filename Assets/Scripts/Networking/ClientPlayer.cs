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
    [SyncVar] private float longestTime; // amount of time in seconds that the player has been alive for the longest

    public string Username { get { return username; } }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);

        if(IsOwner)
        {
            Initialize();
        }
    }

    private void Update()
    {
        if(IsOwner)
        {
            Timer();

            if(Input.GetKeyDown(KeyCode.U))
            {
                Death();
            }
        }
    }

    private void Timer()
    {
        currentTime += Time.deltaTime;

        if(currentTime > longestTime)
        {
            SetLongestTime(currentTime);
        }
    }

    private void Death()
    {
        currentTime = 0;
    }

    // what happens when the player joins the server
    [ServerRpc]
    private void Initialize()
    {
        username = PlayerPrefs.GetString("Username");

        string message = username + " has joined the game.";
        Debug.Log(message);
        GameManager.Instance.DebugToClients(message);
    }

    [ServerRpc]
    public void ChangeHealth(int change)
    {
        health += change;
    }

    [ServerRpc]
    public void SetLongestTime(float time)
    {
        if(time > longestTime)
        {
            longestTime = time;
        }
    }
}
