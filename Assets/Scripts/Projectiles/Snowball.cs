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
            FreezePlayer(Player);
            Despawn();
        }
    }

    private void FreezePlayer(ClientPlayer client)
    {
        // freeze player here...
        // create script called PlayerFreeze in Player script folder
        print("Player frozen.");
        //targetPlayer.GetComponent<PlayerFreeze>().Freeze();

        FreezeClient(client);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetPlayer != null) return;

        if (other.CompareTag("Player") && other != Player)
        {
            // player in range
            FreezePlayer(other.GetComponent<ClientPlayer>());
            Despawn();
        }
    }

    private void FreezeClient(ClientPlayer player)
    {
        player.Frozen = true;
    }
}
