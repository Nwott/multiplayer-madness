using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class Snowball : Projectile
{
    [Header("Snowball Settings")]
    [SerializeField] private float detectionRange = 1f; // range to detect player

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if(IsServer)
        {
            DetectPlayer();
        }
    }

    private void DetectPlayer()
    {
        if (!IsServer) return;

        if (targetPlayer == null) return;

        if(Vector3.Distance(transform.position, TargetPlayer.transform.position) <= detectionRange)
        {
            // player in range
            FreezePlayer();
            Despawn();
        }
    }

    private void FreezePlayer()
    {
        // freeze player here...
        // create script called PlayerFreeze in Player script folder
        print("Player frozen.");
        //targetPlayer.GetComponent<PlayerFreeze>().Freeze();

        FreezeClient(Player.Owner, Player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetPlayer != null) return;

        if (other.CompareTag("Player"))
        {
            // player in range
            FreezePlayer();
            Despawn();
        }
    }

    [TargetRpc]
    private void FreezeClient(NetworkConnection conn, ClientPlayer player)
    {
        player.Frozen = true;
    }
}
