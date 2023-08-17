using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using UnityEngine.InputSystem;
using FishNet.Component.Spawning;
using FishNet;

public class ClientPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ownerObjects; // object to enable if the player is the owner of this object
    [SerializeField] private GameObject holdObject;
    [SerializeField] private GameObject firepoint;
    [SerializeField] private GameObject model;
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerFreeze freeze;
    [SerializeField] private UserInterface userInterface;
    [SerializeField] private OverheadUI overheadUI;
    [SerializeField] private Transform overheadUITransform;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerAnimations playerAnimations;
    [SerializeField] private GameObject iceBlockGO;

    private GameObject iceBlock;

    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float itemDetectionRadius = 4f;

    [SyncVar][HideInInspector] private string username;
    [SyncVar][HideInInspector] private int health;

    private float currentTime; // amount of time in seconds that the player has been alive for
    [SyncVar][HideInInspector] private float longestTime; // amount of time in seconds that the player has been alive for the longest

    private Item item;

    private bool paused;

    public bool Paused { get { return paused; } private set { paused = value; OnPauseChanged(); } }

    [SyncVar(OnChange = nameof(on_frozen), Channel = FishNet.Transporting.Channel.Reliable)][HideInInspector] private bool frozen;

    public bool Frozen 
    { 
        get { return frozen; } 
        set 
        {
            frozen = value;
        } 
    }

    public Item Item { get { return item; } }

    public float CurrentTime { get { return currentTime; } }
    public float LongestTime { get { return longestTime; } }

    public string Username { get { return username; } }

    public GameObject HoldObject { get { return holdObject; } }
    
    public int MaxHealth { get { return maxHealth; } }
    public int Health { get { return health; } }

    public OverheadUI Overhead { get { return overheadUI; } set { overheadUI = value; } }

    public Vector3 OverheadUIPosition { get { return overheadUITransform.position; } }
    public Quaternion OverheadUIRotation { get { return overheadUITransform.rotation; } }
    public PlayerMovement Movement { get { return movement; } }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);

        if(IsOwner)
        {
            GetComponent<InputManager>().enabled = true;
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
            CheckIfPlayerFellThroughGround();
        }
    }

    private void CheckIfPlayerFellThroughGround()
    {
        if(IsOwner)
        {
            if(transform.position.y <= -5)
            {
                transform.position = new Vector3(transform.position.x, 2.5f, transform.position.z);
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

    public void TogglePaused()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        Paused = !Paused;
    }

    public void LeaveGame()
    {
        if(IsOwner)
        {
            //InstanceFinder.ClientManager.StopConnection();
            InstanceFinder.ClientManager.StopConnection();

            if(IsServer)
            {
                InstanceFinder.ServerManager.StopConnection(false);
            }
        }
    }

    public void OnPauseChanged()
    {
        if(IsOwner)
        {
            GetComponent<InputManager>().enabled = !Paused;
            GetComponent<PlayerMovement>().enabled = !Paused;
        }
    }

    // run by server
    private void Death()
    {
        currentTime = 0;
        GameManager.Instance.OnPlayerDeath(this);

        if(IsServer)
        {
            ResetTime();
        }
    }

    [ObserversRpc]
    private void ResetTime()
    {
        if(IsOwner)
        {
            currentTime = 0;
        }
    }

    // for picking up items
    public void PickUp()
    {
        if (item != null) return; // if player is already holding item, don't run code

        if(IsOwner && !frozen)
        {
            Collider closestItem = GetClosestItem(Physics.OverlapSphere(transform.position, itemDetectionRadius));

            if (closestItem != null)
            {
                Item heldItem = closestItem.GetComponent<Item>();
                GameManager.Instance.OnItemPickup(this, heldItem, holdObject);
                item = heldItem;
                heldItem.Player = this;

                userInterface.UpdateItemSlot(heldItem.ItemSO);
            }
        }
    }

    public void UseItem()
    {
        if(IsOwner && !frozen)
        {
            item.Firepoint = firepoint.transform;
            item.Perform();
            userInterface.UpdateItemSlot(null);
        }
    }

    private void on_frozen(bool prev, bool next, bool asServer)
    {

        if(next == true)
        {
            if(asServer)
            {
                iceBlock = GameManager.Instance.SpawnObject(iceBlockGO, transform.position, Quaternion.identity, null);
            }

            freeze.Freeze();
        }
        else
        {
            if(asServer && iceBlock != null)
            {
                Despawn(iceBlock);
            }

            freeze.Unfreeze();
        }
    }

    public void SpawnPlayer(Vector3 position)
    {
        transform.localPosition = position;
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

        health = maxHealth;

        GameManager.Instance.OnPlayerJoin(this);
    }

    [ServerRpc]
    public void ChangeHealth(int change)
    {
        health += change;
        health = Mathf.Clamp(health, 0, maxHealth);
        userInterface.UpdateHealthBar(health, maxHealth);
        userInterface.UpdateHealthBarOnClients(health, maxHealth);

        if(health <= 0)
        {
            Death();
        }
    }

    public void ResetHealth()
    {
        if(IsServer)
        {
            health = MaxHealth;
            userInterface.UpdateHealthBar(Health, MaxHealth);
            userInterface.UpdateHealthBarOnClients(Health, MaxHealth);
        }
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

    public void Walking(bool walking)
    {
        playerAnimations.Walking = walking;
    }
}
