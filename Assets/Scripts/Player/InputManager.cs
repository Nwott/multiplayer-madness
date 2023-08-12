using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class InputManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private ClientPlayer clientPlayer;

    Controls controls;
    Controls.MovementActions movement;
    Controls.InteractionActions interaction;
    Vector2 hInput;
    PlayerMovement playerController;

    private void Awake()
    {
        controls = new Controls();
        playerController = gameObject.GetComponent<PlayerMovement>();
        movement = controls.Movement;
        interaction = controls.Interaction;

        movement.HMovement.performed += ctx => hInput = ctx.ReadValue<Vector2>();
        interaction.PickUp.performed += _ => clientPlayer.PickUp();
        //movement.Eat.performed += ctx => playerController.eat();
        

        //uiActions.Debug.performed += ctx => GetComponentInChildren<DeathScreenManager>().SetGameOverScreenActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            playerController.ReceiveInputs(hInput);
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
