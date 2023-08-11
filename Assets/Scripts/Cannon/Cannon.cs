using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Cannon : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject cannonBarrel;
    [SerializeField] private GameObject center;
    [SerializeField] private GameObject barrelSpawnpoint;

    [Header("Settings")]
    [SerializeField] private float waitTime = 1f; // time between barrel active and shoot
    [SerializeField] private float shootInterval = 5f; // time between shoot cycles

    private List<BarrelData> barrelData = new();

    public void AddBarrel(ClientPlayer player)
    {
        if (IsServer)
        {
            GameObject barrel = GameManager.Instance.SpawnObject(cannonBarrel, barrelSpawnpoint.transform.position, Quaternion.identity, gameObject.transform);
            barrelData.Add(new BarrelData(barrel, player));
            barrel.SetActive(false);
        }
    }
}
