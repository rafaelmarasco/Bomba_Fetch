using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PropInteract propInteract;
    [SerializeField] private float moveSpeed = 7f;
    private Rigidbody rb;
    private bool isWalking => moveDir != Vector3.zero;
    public Vector3 moveDir { get; private set; }
    private Vector3 lastMoveDir = Vector3.forward;

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
        lastMoveDir = moveDir != Vector3.zero ? moveDir : lastMoveDir;
        RotateOnMove();
    }
    private void BasicMove()
    {
        Vector2 playerInput = playerMovement.GetMovementVectorNormalized();
        moveDir = new Vector3(playerInput.x, 0f, playerInput.y);

        if (CanMove())
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

    public Vector3 GetLastMoveDirection()
    {
        return lastMoveDir;
    }


    private bool CanMove()
    {
        float checkDistance = .8f;
        Vector3 rayCastOrigin = transform.position;

        if (propInteract.hasItem)
        {
            rayCastOrigin = propInteract.heldItem.transform.position;
            rayCastOrigin.y = transform.position.y;
        }
        
        bool canMove = !Physics.Raycast(rayCastOrigin, moveDir, checkDistance, LayerMask.GetMask("Walls"));
        return canMove;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(transform.position, lastMoveDir);
    }

}
