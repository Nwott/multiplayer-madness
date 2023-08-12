using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;
using FishNet.Object.Synchronizing;

public class Item : NetworkBehaviour
{
    [Header("Item References")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private ItemScriptableObject itemSO;

    [SyncVar][HideInInspector] public GameObject spawnedProjectile;

    private Transform firepoint;

    private bool itemHeld;

    public bool ItemHeld { get { return itemHeld; } set { itemHeld = value; } }
    
    public Transform Firepoint { get { return firepoint; } set { firepoint = value; } }

    public virtual void Perform()
    {
        if(itemSO.throwable)
        {
            Throw();
        }
        else
        {
            Use();
        }
    }

    protected virtual void Use()
    {

    }

    protected virtual void Throw()
    { 
        GameManager.Instance.SpawnProjectileRPC(projectile, Firepoint.position, Quaternion.identity, transform.parent.forward);

        IsDone();
    }

    protected virtual void IsDone()
    {
        GameManager.Instance.RemoveItemFromList(this);
        GameManager.Instance.DespawnObjectRPC(gameObject);
    }
}
