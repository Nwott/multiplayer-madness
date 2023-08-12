using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Fish : Item
{
    [Header("Fish Settings")]
    [SerializeField] private int amountToHeal = 10;

    protected override void Use()
    {
        if (!IsOwner) return;

        base.Use();

        Player.ChangeHealth(amountToHeal);
    }
}
