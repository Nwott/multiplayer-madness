using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Item : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] protected CharacterController controller;
    [SerializeField] protected GameObject firepoint;
    [SerializeField] protected GameObject projectile;

    [Header("Settings")]
    [SerializeField] protected string itemName;
    [SerializeField][TextArea] protected string description;
    [SerializeField] protected bool throwable;
    [SerializeField] protected int damage;
    [SerializeField] protected float speed;

    protected virtual void Perform()
    {
        if(throwable)
        {
            Throw();
        }
        else
        {
            Use();
        }
    }

    protected virtual void Throw()
    {
        GameManager.Instance.SpawnObject(projectile, firepoint.transform.position, Quaternion.identity, null);
    }

    protected virtual void Use()
    {

    }

    private void PickUp(ClientPlayer player)
    {
    }
}
