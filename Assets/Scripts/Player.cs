using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
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
        Vector3 equationMovement = moveDir * moveSpeed * Time.fixedDeltaTime;
        playerCamera.position = new Vector3(playerCamera.position.x + equationMovement.x, playerCamera.position.y + equationMovement.y, playerCamera.position.z + equationMovement.z);
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
