using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBarrel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firepoint;

    public void Shoot(Vector3 target)
    {
        GameObject cannonBall = GameManager.Instance.SpawnObject(projectile, firepoint.position, Quaternion.identity, null);
        Projectile projScript = cannonBall.GetComponent<Projectile>();
        projScript.Target = target;
    }
}
