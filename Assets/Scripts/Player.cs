using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float moveSpeed = 7f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        BasicMove();
    }
    private void BasicMove()
    {
        Vector2 playerInput = playerMovement.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(playerInput.x, 0f, playerInput.y);
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }
}
