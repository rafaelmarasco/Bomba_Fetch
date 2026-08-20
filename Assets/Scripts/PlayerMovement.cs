using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputSystem_Actions playerInputs;
    private Rigidbody rb;
    private Vector2 movementInputs;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputs.Player.Enable();
    }

    public Vector2 GetMovementVectorNormalized()
    {
      return movementInputs = playerInputs.Player.Move.ReadValue<Vector2>();
    }


}
