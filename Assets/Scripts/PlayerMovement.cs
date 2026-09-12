using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction movementInput;
    private Rigidbody rb;
    private Vector2 movementInputs;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        movementInput = playerInput.actions["Move"];
    }

    public Vector2 GetMovementVectorNormalized()
    {
      movementInputs = movementInput.ReadValue<Vector2>().normalized;
      return movementInputs;
    }


}
