using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;

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
        print("test");

        GameObject obj = Instantiate(projectile, Firepoint.position, Quaternion.identity);
        GameManager.Instance.OnlySpawnObjectRPC(obj);
        Projectile proj = obj.GetComponent<Projectile>();

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

        proj.Target = targetPos;



        IsDone();
    }

    protected virtual void IsDone()
    {

    }
}
