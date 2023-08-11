using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SyncObject] public readonly SyncList<ClientPlayer> players = new();

    ClientPlayer playerWLongestTime; // player with longest time

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(players.Count > 0)
        {
            GetLongestTime();
        }
    }

    private void GetLongestTime()
    {
        float longestTime = 0;

        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].LongestTime > longestTime)
            {
                playerWLongestTime = players[i];
                longestTime = players[i].LongestTime;
            }
        }
    }

    [ObserversRpc]
    public void DebugToClients(string message)
    {
        if (IsServer) return;

        Debug.Log(message);
    }
}
