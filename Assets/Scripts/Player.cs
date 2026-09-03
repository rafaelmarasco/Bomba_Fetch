using UnityEngine;
using UnityEngine.EventSystems;

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
        RotateOnMove();
        BasicMove();
    }

    private void Update()
    {
        lastMoveDir = moveDir != Vector3.zero ? moveDir : lastMoveDir;

    }
    private void BasicMove()
    {
        Vector2 playerInput = playerMovement.GetMovementVectorNormalized();
        moveDir = new Vector3(playerInput.x, 0f, playerInput.y);

        if (!CanMove(moveDir))
        {
            Vector3 moveDirecetionX = new Vector3(moveDir.x, 0f, 0f);

            if (CanMove(moveDirecetionX))
            {
                moveDir = moveDirecetionX;
            }
            else
            {
                Vector3 moveDirecetionZ = new Vector3(0f, 0f, moveDir.z);
                if (CanMove(moveDirecetionZ))
                {
                    moveDir = moveDirecetionZ;
                }
            }
        }

        if (CanMove(moveDir))
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


    private bool CanMove(Vector3 moveDirection)
    {
        bool canMove;
        float checkDistance = .8f;
        Vector3 checkOrigin = transform.position;
        Quaternion targetRotation = moveDirection != Vector3.zero ? Quaternion.LookRotation(moveDirection) : transform.rotation;

        if (propInteract.hasItem)
        {
            checkOrigin = propInteract.heldItem.transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
            checkOrigin.y = transform.position.y;
            float propReach = Vector3.Distance(transform.position, checkOrigin);
            Vector3 halfExtends = new Vector3(.25f, .25f, .25f + propReach);

            canMove = !Physics.CheckBox(checkOrigin, halfExtends, targetRotation, LayerMask.GetMask("Walls"));
        }
        else
            canMove = !Physics.Raycast(checkOrigin, moveDirection, checkDistance, LayerMask.GetMask("Walls"));

        return canMove;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(transform.position, lastMoveDir);

        if (propInteract.hasItem)
            Gizmos.DrawCube(transform.position, new Vector3(4f, 4f, 4f));
    }

}
