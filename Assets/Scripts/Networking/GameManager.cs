using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;
using FishNet.Connection;
using FishNet.Component.Spawning;

public class GameManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Cannon cannon;
    [SerializeField] private List<GameObject> itemDrops = new();
    [SerializeField] private GameObject overheadUI;

    private PlayerSpawner playerSpawner;

    [Header("Settings")]
    [SerializeField] private int maxItems = 5; // max amount of items that can be on the map at once
    [SerializeField] private float updateInterval = 0.5f;

    private float updateTimer;

    public List<GameObject> ItemDrops { get { return itemDrops; } }

    public static GameManager Instance { get; private set; }

    [SyncObject] public readonly SyncList<ClientPlayer> players = new();
    [SyncObject] public readonly SyncList<ClientPlayer> playersSortedByTime = new();
    [SyncObject] public readonly SyncList<Item> items = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerSpawner = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PlayerSpawner>();
    }

    private void Update()
    {
        if (!IsServer) return;

        if(players.Count > 0)
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        updateTimer += Time.deltaTime;

        if(updateTimer >= updateInterval)
        {
            updateTimer = 0;
            UpdateLeaderboard();
        }
    }

    private void UpdateLeaderboard()
    {
        string msg = "";
        int index = -1;

        playersSortedByTime.Clear();

        foreach(ClientPlayer player in players.OrderByDescending(p => p.LongestTime))
        {
            index++;

            // detect if player has left
            if (player == null)
            {
                OnPlayerLeave(index);
                continue;
            }

            playersSortedByTime.Add(player);
        }
    }

    public void OnPlayerJoin(ClientPlayer player)
    {
        if (!IsServer) return;

        cannon.AddBarrel(player);
        players.Add(player);

        // spawn and setup overheadUI
        GameObject overhead = SpawnObject(overheadUI, player.OverheadUIPosition, player.OverheadUIRotation, player.transform);
        OverheadUI overheadScript = overhead.GetComponent<OverheadUI>();
        overheadScript.UpdateUsername(player.Username);
        overheadScript.InitializeOnClients(player.Username);
        overheadScript.ClientPlayer = player;

        for(int i = 0; i < players.Count; i++)
        {
            players[i].GetComponentInChildren<OverheadUI>().InitializeOnClients(players[i].Username);
        }
    }

    // if player leaves, then remove them from list
    public void OnPlayerLeave(int index)
    {
        players.RemoveAt(index);
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

    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerDeath(ClientPlayer player)
    {
        player.ResetHealth();
        player.Frozen = false;
        player.Movement.CanMove = true;

        // random spawn location
        Vector3 spawnLocation = playerSpawner.Spawns[Random.Range(0, playerSpawner.Spawns.Length - 1)].position;

        OnPlayerDeathClient(true, player.Movement, player, spawnLocation);
    }

    [ObserversRpc]
    private void OnPlayerDeathClient(bool enabled, PlayerMovement movement, ClientPlayer player, Vector3 spawnLocation)
    {
        movement.CanMove = enabled;
        player.Frozen = false;
        player.Movement.Controller.Move(spawnLocation - player.transform.position);
    }

    [ServerRpc]
    public void SpawnObjectRPC(GameObject obj, Vector3 position, Quaternion rotation)
    {
        GameObject go = Instantiate(obj, position, rotation);
        Spawn(go);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnlySpawnObjectRPC(GameObject obj)
    {
        Spawn(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnObjectRPC(GameObject obj)
    {
        Despawn(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnItemPickup(ClientPlayer player, Item item, GameObject holdObject)
    {
        item.transform.position = holdObject.transform.position;
        item.transform.parent = player.HoldObject.transform;
        item.ItemHeld = true;
        item.Player = player;
        OnItemPickupObservers(player, item, holdObject.transform.position);
        item.GiveOwnership(player.Owner);
    }

    [ObserversRpc]
    public void RefreshObjectActivity(GameObject obj, bool active)
    {
        obj.SetActive(active);
    }

    [ObserversRpc]
    public void OnItemPickupObservers(ClientPlayer player, Item item, Vector3 holdObject)
    {
        item.transform.position = holdObject;
        item.transform.parent = player.HoldObject.transform;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnProjectileRPC(GameObject obj, Vector3 position, Quaternion rotation, Vector3 targetPos)
    {
        GameObject projectile = Instantiate(obj, position, rotation);
        Spawn(projectile);

        Projectile proj = projectile.GetComponent<Projectile>();
        proj.Target = targetPos;
        proj.ThrownByPlayer = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddItemToList(Item item)
    {
        items.Add(item);

        if(items.Count > maxItems)
        {
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i] == null)
                {
                    continue;
                }

                if (items[i].ItemHeld)
                {
                    continue;
                }

                RemoveItemFromList(items[i]);
                Despawn(items[i].gameObject);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveItemFromList(Item item)
    {
        items.Remove(item);
    }
}
