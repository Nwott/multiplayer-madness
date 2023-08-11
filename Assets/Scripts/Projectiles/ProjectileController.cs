using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class ProjectileController : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float damageRange;
    private CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            MoveTowardsTarget(new Vector3(0, 5, 5));
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
