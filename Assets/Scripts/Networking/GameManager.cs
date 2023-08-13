using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Cannon cannon;
    [SerializeField] private List<GameObject> itemDrops = new();

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

        playersSortedByTime.Clear();

        foreach(ClientPlayer player in players.OrderByDescending(p => p.LongestTime))
        {
            playersSortedByTime.Add(player);
            //msg += player.Username + ": " + player.LongestTime.ToString() + " ";
        }
    }

    public void OnPlayerJoin(ClientPlayer player)
    {
        if (!IsServer) return;

        cannon.AddBarrel(player);
        players.Add(player);
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
