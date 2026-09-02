using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float moveSpeed = 7f;
    private Rigidbody rb;
    private bool isWalking => moveDir != Vector3.zero;
    public Vector3 moveDir { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        BasicMove();
    }

    private void Update()
    {
        RotateOnMove();
    }
    private void BasicMove()
    {
        Vector2 playerInput = playerMovement.GetMovementVectorNormalized();
        moveDir = new Vector3(playerInput.x, 0f, playerInput.y);
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    private void RotateOnMove()
    {
        if (moveDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    }

    public bool GetIsWalking()
    {
        return isWalking;
    }


}
