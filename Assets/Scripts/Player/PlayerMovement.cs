using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    private bool canMove = true;

    public bool CanMove { get { return canMove; } set { canMove = value; } }

    Vector2 hInput;
    CharacterController controller;

    public CharacterController Controller { get { return controller; } }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOwner && canMove)
        {
            controller.Move(new Vector3(hInput.x * moveSpeed * Time.deltaTime, 0, hInput.y * moveSpeed * Time.deltaTime));
        }
    }

    public void ReceiveInputs(Vector2 input)
    {
        hInput = input;
    }
}
