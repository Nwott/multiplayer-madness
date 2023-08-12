using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Item : NetworkBehaviour
{
    [Header("Item References")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private ItemScriptableObject itemSO;

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
        GameObject obj = Instantiate(projectile, Firepoint.position, Quaternion.identity);
        GameManager.Instance.OnlySpawnObjectRPC(obj);
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Projectile proj = obj.GetComponent<Projectile>();
        proj.Target = targetPos;

        IsDone();
    }

    protected virtual void IsDone()
    {

    }
}
