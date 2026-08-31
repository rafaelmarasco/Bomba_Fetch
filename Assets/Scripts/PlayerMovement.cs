using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private InputSystem_Actions playerInputs;
    private Rigidbody rb;
    private Vector2 movementInputs;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputs = new InputSystem_Actions();
        playerInputs.Player.Move.Enable();
    }

    public Vector2 GetMovementVectorNormalized()
    {
      movementInputs = playerInputs.Player.Move.ReadValue<Vector2>().normalized;
      return movementInputs;
    }


}
