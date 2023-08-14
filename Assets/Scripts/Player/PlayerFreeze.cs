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
        if (clientPlayer.Frozen)
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
        if (clientPlayer.Frozen) return;

        unfreezeCount = 0;
        playerMovement.canMove = false;
        damageTimer = 0;
    }

    public void PartialUnfreeze()
    {
        if (clientPlayer.Frozen)
        {
            unfreezeCount++;
            print(unfreezeCount+" e clicks and "+unfreezeNumber+" needed");
            if (unfreezeCount >= unfreezeNumber)
            {
                clientPlayer.Frozen = false;
                playerMovement.canMove = true;
            }
        }
    }
}
