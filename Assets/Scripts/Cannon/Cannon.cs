using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class Cannon : NetworkBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject cannonBarrel;
    [SerializeField] private GameObject barrelSpawnpoint;

    [Header("Settings")]
    [SerializeField] private float waitTime = 2f; // time between barrel active and shoot
    [SerializeField] private float cycleInterval = 5f; // time between shoot cycles
    [SerializeField] private float afterShootTime = 1f; // time between shoot and barrel inactive

    private float waitTimer;
    private bool startWaitTimer;

    private float cycleTimer;
    private bool startCycleTimer = true;

    private float afterShootTimer;
    private bool startAfterShootTimer;

    private List<BarrelData> barrelData = new();

    [HideInInspector] private Item tempItem;

    private void Update()
    {
        if (!IsServer) return;

        if (startWaitTimer)
        {
            WaitTimer();
        }

        if (startCycleTimer)
        {
            CycleTimer();
        }

        if(startAfterShootTimer)
        {
            AfterShootTimer();
        }
    }

    private void CycleTimer()
    {
        cycleTimer += Time.deltaTime;

        if(cycleTimer > cycleInterval)
        {
            cycleTimer = 0;
            startCycleTimer = false;
            Wait();
        }
    }

    private void WaitTimer()
    {
        waitTimer += Time.deltaTime;

        if(waitTimer > waitTime)
        {
            waitTimer = 0;
            startWaitTimer = false;
            Shoot();
        }
    }

    private void AfterShootTimer()
    {
        afterShootTimer += Time.deltaTime;

        if(afterShootTimer > afterShootTime)
        {
            afterShootTimer = 0;
            startAfterShootTimer = false;
            AfterShoot();
        }
    }

    // enables barrels and points them towards player
    private void Wait()
    {
        startWaitTimer = true;
        startAfterShootTimer = true;

        // rotates barrels towards players
        for (int i = 0; i < barrelData.Count; i++)
        {
            barrelData[i].Barrel.gameObject.SetActive(true);
            RefreshBarrelActive(barrelData[i].Barrel.gameObject, true);

            Vector3 target = barrelData[i].Player.gameObject.transform.position;
            target = new Vector3(target.x, 0, target.z);

            barrelData[i].Target = barrelData[i].Player.gameObject;
            barrelData[i].Barrel.transform.LookAt(target);
        }
    }

    private void Shoot()
    {
        startCycleTimer = true;

        // call shoot on each barrel
        for(int i = 0; i < barrelData.Count; i++)
        {
            barrelData[i].Barrel.Shoot(barrelData[i].Target);
        }
    }

    private void AfterShoot()
    {
        for(int i = 0; i < barrelData.Count; i++)
        {
            barrelData[i].Barrel.gameObject.SetActive(false);
            RefreshBarrelActive(barrelData[i].Barrel.gameObject, false);
        }
    }

    // called if player dodges cannon ball
    protected void DropItem(Vector3 position)
    {
        if (GameManager.Instance.ItemDrops.Count <= 0) return;

        GameObject randomItem = GameManager.Instance.ItemDrops[Random.Range(0, GameManager.Instance.ItemDrops.Count - 1)];

        GameObject itemObj = GameManager.Instance.SpawnObject(randomItem, position, Quaternion.identity, null);
        GameManager.Instance.AddItemToList(itemObj.GetComponent<Item>());
    }

    public void AddBarrel(ClientPlayer player)
    {
        if (IsServer)
        {
            GameObject barrel = GameManager.Instance.SpawnObject(cannonBarrel, barrelSpawnpoint.transform.position, Quaternion.identity, gameObject.transform);
            BarrelData data = new BarrelData(barrel.GetComponent<CannonBarrel>(), player, DropItem);
            barrelData.Add(data);
            barrel.GetComponent<CannonBarrel>().BarrelData = data;
            barrel.SetActive(false);
            RefreshBarrelActive(barrel, false);
        }
    }

    [ObserversRpc]
    private void RefreshBarrelActive(GameObject barrel, bool active)
    {
        barrel.SetActive(active);
    }
}
