using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.UI;
using FishNet.Connection;

public class PlayerFreeze : NetworkBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private int unfreezeNumber;
    [SerializeField] private ClientPlayer clientPlayer;
    [SerializeField] private int damage;
    [SerializeField] private float damageDelay;
    private float damageTimer;
    private int unfreezeCount;
    bool isFrozen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFrozen)
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
        if (isFrozen)
        {
            //do nothing
        }
        else
        {
            isFrozen = true;
            unfreezeCount = 0;
            playerMovement.canMove = false;
            damageTimer = 0;

        }
    }

    public void PartialUnfreeze()
    {
        if (isFrozen)
        {
            unfreezeCount++;
            print(unfreezeCount+" e clicks and "+unfreezeNumber+" needed");
            if (unfreezeCount >= unfreezeNumber)
            {
                isFrozen = false;
                playerMovement.canMove = true;
            }
        }
    }
}
