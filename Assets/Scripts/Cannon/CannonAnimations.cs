using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Component.Animating;

public class CannonAnimations : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private NetworkAnimator networkAnimator;

    public void PlayShootAnimation()
    {
        if(networkAnimator != null && IsServer)
        {
            networkAnimator.SetTrigger("Shoot");
        } 
    }
}
