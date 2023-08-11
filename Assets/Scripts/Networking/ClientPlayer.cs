using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class ClientPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ownerObjects; // object to enable if the player is the owner of this object

    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;

    [SyncVar][HideInInspector] private string username;
    [SyncVar][HideInInspector] private int health;

    private float currentTime; // amount of time in seconds that the player has been alive for
    [SyncVar][HideInInspector] private float longestTime; // amount of time in seconds that the player has been alive for the longest

    public float LongestTime { get { return longestTime; } }

    public string Username { get { return username; } }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);

        if(IsOwner)
        {
            Initialize(PlayerPrefs.GetString("Username"));
            ownerObjects.SetActive(true);
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


    public override void OnStopClient()
    {
        base.OnStopClient();

        if (IsOwner)
        {
            OnDisconnect();
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
    private void Initialize(string username)
    {
        // print username to console
        this.username = username;
        string message = username + " has joined the game.";
        Debug.Log(message);
        GameManager.Instance.DebugToClients(message);

        // add player to players list in GameManager
        GameManager.Instance.players.Add(this);

        health = maxHealth;

        GameManager.Instance.OnPlayerJoin(this);
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

    [ServerRpc(RequireOwnership = false)]
    private void OnDisconnect()
    {
        print("test");
    }
}
