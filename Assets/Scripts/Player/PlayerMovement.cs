using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SyncVar][HideInInspector] private bool canMove = true;

    public bool CanMove { get { return canMove; } set { canMove = value; } }

    Vector2 hInput;
    CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Can Move: " + canMove);

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
