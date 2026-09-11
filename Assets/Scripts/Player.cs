using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PropInteract propInteract;
    [SerializeField] private float moveSpeed = 7f;
    private bool isWalking => moveDir != Vector3.zero;

    public Vector3 moveDir { get; private set; }
    private Vector3 lastMoveDir = Vector3.forward;
    private Vector3 rawInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (!propInteract.isBombInteracting)
        {
            ReadInput();
            RotateOnMove();
            BasicMove();
        }


    }
    private void Update()
    {
        lastMoveDir = moveDir != Vector3.zero ? moveDir : lastMoveDir;

    }
    private void ReadInput()
    {
        Vector2 playerInput = playerMovement.GetMovementVectorNormalized();
        rawInput = new Vector3(playerInput.x, 0f, playerInput.y);

    }
    private void BasicMove()
    {
        moveDir = rawInput;

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
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * moveDir);
    }
    private void RotateOnMove()
    {
        if (rawInput != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rawInput, Vector3.up);
    }
    private bool CanMove(Vector3 moveDirection)
    {
        bool canMove;
        float checkDistance = .8f;
        Vector3 checkOrigin = transform.position;
        Quaternion targetRotation = moveDirection != Vector3.zero ? Quaternion.LookRotation(moveDirection) : transform.rotation;
        GameObject heldItem = propInteract.heldItem;

        if (propInteract.hasItem && heldItem.GetComponent<Prop>().PropSize != Size.small)
        {
            checkOrigin = heldItem.transform.position + moveSpeed * Time.fixedDeltaTime * moveDirection;
            float propReach = Vector3.Distance(transform.position, heldItem.transform.position) / 2;
            Vector3 halfExtends = new(.1f, .25f, .1f + propReach);

            DebugCheckBox(checkOrigin, halfExtends, targetRotation);
            canMove = !Physics.CheckBox(checkOrigin, halfExtends, targetRotation, LayerMask.GetMask("Walls"));
        }

        //-----------------------------------------------------------------------------------------------------------//
        else
            canMove = !Physics.Raycast(checkOrigin, moveDirection, checkDistance, LayerMask.GetMask("Walls"));

        return canMove;
    }
    private void DebugCheckBox(Vector3 checkOrigin, Vector3 halfExtends, Quaternion targetRotation)
    {

        DebugBoxCast.SimpleDrawBox(checkOrigin, halfExtends, targetRotation, Color.antiqueWhite);
    }

    public bool GetIsWalking() { return isWalking; }
    public Vector3 GetLastMoveDirection() { return lastMoveDir; }
}
