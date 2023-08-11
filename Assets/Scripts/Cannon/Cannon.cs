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

    private float waitTimer;
    private bool startWaitTimer;

    private float cycleTimer;
    private bool startCycleTimer = true;

    private List<BarrelData> barrelData = new();

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
    }

    private void CycleTimer()
    {
        cycleTimer += Time.deltaTime;

        if(cycleTimer > cycleInterval)
        {
            cycleTimer = 0;
            startCycleTimer = false;
            startWaitTimer = true;
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
        }
    }

    // enables barrels and points them towards player
    private void Wait()
    {
        for(int i = 0; i < barrelData.Count; i++)
        {
            barrelData[i].Barrel.SetActive(true);

            Vector3 target = barrelData[i].Player.gameObject.transform.position;

            barrelData[i].Barrel.transform.LookAt(new Vector3(target.x, 0, target.z));
        }
    }

    private void Shoot()
    {

    }

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
