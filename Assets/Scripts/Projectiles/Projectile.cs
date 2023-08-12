using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Projectile : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;

    [Header("Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float damageRange;

    private Vector3 target;

    public Vector3 Target { get { return target; } set { target = value; } }

    void Update()
    {
        if (IsServer)
        {
            MoveTowardsTarget(Target);
        }
    }

    void MoveTowardsTarget(Vector3 target)
    {
        transform.LookAt(target);
        controller.Move(transform.forward * speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) <= damageRange)
        {
            Despawn();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected void DestroyEntity()
    {
        Despawn();
    }
}
