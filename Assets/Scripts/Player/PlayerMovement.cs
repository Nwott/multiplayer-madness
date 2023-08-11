using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 hInput;
    CharacterController controller;
    [SerializeField] private float moveSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        controller.Move(new Vector3(hInput.x * moveSpeed * Time.deltaTime, 0, hInput.y * moveSpeed * Time.deltaTime));
    }

    public void receiveInput(Vector2 input)
    {
        hInput = input;
    }
}
