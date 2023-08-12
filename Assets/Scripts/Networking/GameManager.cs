using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class GameManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Cannon cannon;

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

    public void OnPlayerJoin(ClientPlayer player)
    {
        if (!IsServer) return;

        cannon.AddBarrel(player);
    }

    [ObserversRpc]
    public void DebugToClients(string message)
    {
        if (IsServer) return;

        Debug.Log(message);
    }

    public GameObject SpawnObject(GameObject obj, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject go = Instantiate(obj, position, rotation, parent);
        Spawn(go);

        return go;
    }

    [ServerRpc]
    public void SpawnObjectRPC(GameObject obj, Vector3 position, Quaternion rotation)
    {
        GameObject go = Instantiate(obj, position, rotation);
        Spawn(go);
    }

    [ServerRpc]
    public void OnlySpawnObjectRPC(GameObject obj)
    {
        Spawn(obj);
    }

    [ServerRpc]
    public void DespawnObjectRPC(GameObject obj)
    {
        Despawn(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnItemPickup(ClientPlayer player, Item item, Vector3 holdObject)
    {
        item.transform.position = holdObject;
        item.transform.parent = player.transform;
        item.GiveOwnership(player.Owner);
    }

    [ObserversRpc]
    public void RefreshObjectActivity(GameObject obj, bool active)
    {
        obj.SetActive(active);
    }
}
