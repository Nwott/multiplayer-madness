using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerAnimations : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    private bool walking;

    public bool Walking 
    { 
        get { return walking; } 
        set 
        {
            if (walking == value) return;
            walking = value;
            OnWalkingChanged();
        } 
    }

    private void OnWalkingChanged()
    {
        if (!IsOwner) return;

        if(animator != null)
        {
            animator.SetBool("Walking", walking);
        }

        OnWalkingChangedServer(walking);
    }

    [ServerRpc]
    private void OnWalkingChangedServer(bool walking)
    {
        if(animator != null)
        {
            animator.SetBool("Walking", walking);
        }
    }

    [ObserversRpc]
    private void OnWalkingChangedClients(bool walking)
    {
        if (IsOwner) return;

        if(animator != null)
        {
            animator.SetBool("Walking", walking);
        }
    }
}
