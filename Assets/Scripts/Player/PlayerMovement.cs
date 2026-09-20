using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction movementInput;
    private Vector2 movementInputs;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        movementInput = playerInput.actions["Move"];
    }
    public Vector2 GetMovementVectorNormalized()
    {
      movementInputs = movementInput.ReadValue<Vector2>().normalized;
      return movementInputs;
    }
}
