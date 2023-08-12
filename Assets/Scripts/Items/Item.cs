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
        Vector3 targetPos = new Vector3(0, transform.position.y, 0);

        // get target position from mouse position
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            targetPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
        }

        GameManager.Instance.SpawnProjectileRPC(projectile, Firepoint.position, Quaternion.identity, targetPos);

        IsDone();
    }

    protected virtual void IsDone()
    {

    }
}
