using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using UnityEngine.InputSystem;

public class ClientPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ownerObjects; // object to enable if the player is the owner of this object
    [SerializeField] private GameObject holdObject;
    [SerializeField] private GameObject firepoint;
    [SerializeField] private GameObject model;
    [SerializeField] private Camera cam;

    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float itemDetectionRadius = 4f;

    [SyncVar][HideInInspector] private string username;
    [SyncVar][HideInInspector] private int health;

    private float currentTime; // amount of time in seconds that the player has been alive for
    [SyncVar][HideInInspector] private float longestTime; // amount of time in seconds that the player has been alive for the longest

    private Item item;

    public Item Item { get { return item; } }

    public float LongestTime { get { return longestTime; } }

    public string Username { get { return username; } }

    public GameObject HoldObject { get { return holdObject; } }

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
            RotatePlayer();

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

    // for picking up items
    public void PickUp()
    {
        if (item != null) return; // if player is already holding item, don't run code

        Collider closestItem = GetClosestItem(Physics.OverlapSphere(transform.position, itemDetectionRadius));

        if(closestItem != null)
        {
            Item heldItem = closestItem.GetComponent<Item>();
            GameManager.Instance.OnItemPickup(this, heldItem, holdObject.transform.position);
            item = heldItem;
        }
    }

    public void UseItem()
    {
        item.Firepoint = firepoint.transform;
        item.Perform();
    }

    private Collider GetClosestItem(Collider[] colliders)
    {
        if (colliders.Length < 1) return null;

        Collider closestCollider = null;

        for(int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Item"))
            {
                if (closestCollider == null)
                {
                    closestCollider = colliders[i];
                }
                else
                {
                    if (Vector3.Distance(colliders[i].transform.position, transform.position) <
                        Vector3.Distance(closestCollider.transform.position, transform.position))
                    {
                        closestCollider = colliders[i];
                    }
                }
            }
        }

        return closestCollider;
    }

    private void RotatePlayer()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = cam.nearClipPlane;
        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            model.transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
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
