using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Projectile : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] protected CharacterController controller;
    [SerializeField] private DestroyAfterTime destroyScript;

    [Header("Settings")]
    [SerializeField] protected float speed = 15;
    [SerializeField] protected float damage;
    [SerializeField] protected float despawnRange = 0.3f;

    protected ClientPlayer player;

    public ClientPlayer Player { get { return player; } set { player = value; } }

    private bool thrownByPlayer;

    public bool ThrownByPlayer
    {
        get { return thrownByPlayer; }
        set
        {
            thrownByPlayer = value;

            if (thrownByPlayer)
            {
                destroyScript.enabled = true;
            }
        }
    }

    private Vector3 target;
    protected GameObject targetPlayer;

    public Vector3 Target { get { return target; } set { target = value; } }
    public GameObject TargetPlayer { get { return targetPlayer; } set { targetPlayer = value; } }

    protected virtual void OnStart()
    {

    }

    protected virtual void OnUpdate()
    {
        if (IsServer)
        {
            if(ThrownByPlayer)
            {
                MoveInDirection(Target);
            }
            else
            {
                MoveTowardsTarget(Target);
            }
        }
    }

    protected void MoveTowardsTarget(Vector3 target)
    {
        transform.LookAt(target);
        controller.Move(transform.forward * speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) <= despawnRange)
        {
            Despawn();
        }
    }

    protected void MoveInDirection(Vector3 direction)
    {
        controller.Move(direction.normalized * speed * Time.deltaTime);
    }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        OnUpdate();
    }
}
