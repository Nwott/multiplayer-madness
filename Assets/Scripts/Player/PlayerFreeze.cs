using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.UI;
using FishNet.Connection;

public class PlayerFreeze : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private ClientPlayer clientPlayer;
    [SerializeField] private GameObject frozenUI;
    [SerializeField] private Slider thermometerSlider;

    [Header("Settings")]
    [SerializeField] private int unfreezeNumber;
    [SerializeField] private int damage;
    [SerializeField] private float damageDelay;

    private float damageTimer;
    private int unfreezeCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (clientPlayer.Frozen && IsOwner)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageDelay)
            {
                damageTimer = 0;
                clientPlayer.ChangeHealth(-damage);
            }
        }
    }

    public void Freeze()
    {
        unfreezeCount = 0;
        playerMovement.CanMove = false;
        damageTimer = 0;

        if(IsOwner)
        {
            frozenUI.SetActive(true);
        }
    }

    public void PartialUnfreeze()
    {
        if (clientPlayer.Frozen && IsOwner)
        {
            unfreezeCount++;

            thermometerSlider.value = (float)unfreezeCount / (float)unfreezeNumber;

            if (unfreezeCount >= unfreezeNumber)
            {
                UnfreezeOnServer(clientPlayer);
            }
        }
    }

    public void Unfreeze()
    {
        playerMovement.CanMove = true;

        if(IsOwner)
        {
            thermometerSlider.value = 0;
            frozenUI.SetActive(false);
        }
    }

    [ServerRpc]
    private void UnfreezeOnServer(ClientPlayer player)
    {
        player.Frozen = false;
    }
}
