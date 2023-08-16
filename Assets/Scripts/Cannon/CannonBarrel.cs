using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBarrel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firepoint;
    [SerializeField] private CannonAnimations animations;

    private BarrelData barrelData;

    public BarrelData BarrelData { get { return barrelData; } set { barrelData = value; } }

    public void PlayShootAnimation()
    {
        animations.PlayShootAnimation();   
    }

    public void Shoot(GameObject target)
    {
        GameObject cannonBall = GameManager.Instance.SpawnObject(projectile, firepoint.position, Quaternion.identity, null);
        Projectile projScript = cannonBall.GetComponent<Projectile>();
        projScript.Target = target.transform.position;
        projScript.TargetPlayer = target;
	    projScript.Callback = barrelData.Callback;
	    projScript.Player = target.GetComponent<ClientPlayer>();
    }
}
