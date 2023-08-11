using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Cannon : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject cannonBarrel;
    [SerializeField] private GameObject center;

    [Header("Settings")]
    [SerializeField] private float waitTime = 1f; // time between barrel active and shoot
    [SerializeField] private float shootInterval = 5f; // time between shoot cycles

    
}
