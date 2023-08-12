using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Item : NetworkBehaviour
{
    [Header("Item References")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private ItemScriptableObject itemSO;

    private void PickUp()
    {

    }
}
