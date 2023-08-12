using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class DestroyAfterTime : NetworkBehaviour
{
    [SerializeField] float timeUntilDestroy;
    [SerializeField] bool isNetworked = false;
    float timer;

    private void Update()
    {
        if (isNetworked && !IsServer) return;

        timer += Time.deltaTime;

        if (timer >= timeUntilDestroy)
        {
            if (isNetworked && IsServer)
            {
                Despawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}