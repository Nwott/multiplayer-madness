using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class Snowball : Projectile
{
    [Header("Snowball References")]
    [SerializeField] private AudioSource srcSnowballHit;

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
        if (!IsServer) return;
        if (targetPlayer != null) return;

        print(Player.Username);

        if (other.CompareTag("Player") && other != Player)
        {
            // player in range
            PlaySnowballHit();
            FreezePlayer(other.GetComponent<ClientPlayer>());
            Despawn();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!IsServer) return;

        if (hit.gameObject.layer == LayerMask.NameToLayer("Environment") && !ThrownByPlayer)
        {
            Callback(transform.position);
            PlaySnowballHit();
            Despawn();
        }
        else if(hit.gameObject.layer == LayerMask.NameToLayer("Environment") && ThrownByPlayer)
        {
            PlaySnowballHit();
            Despawn();
        }
    }

    private void FreezeClient(ClientPlayer player)
    {
        player.Frozen = true;
    }

    private void PlaySnowballHit()
    {
        if(IsServer)
        {
            //PlaySnowballHitServer();
        }
    }

    [ServerRpc]
    private void PlaySnowballHitServer()
    {
        srcSnowballHit.Play();

        PlaySnowballHitClients();
    }

    [ObserversRpc]
    private void PlaySnowballHitClients()
    {
        if (IsServer) return;

        srcSnowballHit.Play();
    }
}
