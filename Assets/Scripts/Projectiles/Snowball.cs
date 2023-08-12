using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Snowball : Projectile
{
    [Header("Snowball Settings")]
    [SerializeField] private float detectionRange = 0.5f; // range to detect player

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
    }
}
