using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody playerRb;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PropInteract propInteract;
    [SerializeField] private float moveSpeed = 7f;

    private Rigidbody[] bonesRb;
    [SerializeField] private GameObject GFX;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody hipsRb;
    public bool isRagDoll { get; private set; } = false;
    private bool isWalking => moveDir != Vector3.zero;

    private PlayerEventManager playerEventManager;
    public Vector3 moveDir { get; private set; }
    private Vector3 lastMoveDir = Vector3.forward;
    private Vector3 rawInput;

    bool stopMoving;

    private void Awake()
    {
        bonesRb = GFX.GetComponentsInChildren<Rigidbody>();
        playerRb = GetComponent<Rigidbody>();

        playerEventManager = GetComponent<PlayerEventManager>();

        UntangleBones();
    }

    private void OnEnable()
    {
        playerEventManager.OnStopedMoving += StopMoving;
        playerEventManager.OnEletrocuted += GetYonked;
    }
    private void FixedUpdate()
    {
        if (!stopMoving)
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

        /*
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
        */
        //if (CanMove(moveDir))
        playerRb.MovePosition(playerRb.position + moveSpeed * Time.fixedDeltaTime * moveDir);
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
    private void StopMoving(bool stopMoving)
    {
        this.stopMoving = stopMoving;
    }
    public void GetYonked(Vector3 flyDirection, float flyForce, float stunTime)
    {
        EnableRagDoll();
        hipsRb.AddForce(flyDirection * flyForce, ForceMode.Impulse);
        playerEventManager.KnockedDown(stunTime);
    }
    public void EnableRagDoll()
    {
        if (propInteract.heldItem != null)
            propInteract.DropProp();

        foreach (Rigidbody bone in bonesRb)
        {
            bone.isKinematic = false;
            bone.useGravity = true;
        }

        animator.enabled = false;
        playerEventManager.StopMoving(true);
        isRagDoll = true;
    }
    public void DisableRagDoll()
    {
        Transform playerRagDollTransform = hipsRb.gameObject.transform;

        transform.position =
            new Vector3(playerRagDollTransform.position.x, transform.position.y, playerRagDollTransform.position.z);

        foreach (Rigidbody bone in bonesRb)
        {
            bone.isKinematic = true;
            bone.useGravity = false;
        }

        animator.enabled = true;
        playerEventManager.StopMoving(false);
        isRagDoll = false;
    }
    private void UntangleBones() // Makes de player collider and bone colliders ignore each other
    {
        Collider playerCollider = GetComponentInChildren<Collider>();

        foreach (Rigidbody bone in bonesRb)
        {
            if (bone.TryGetComponent<Collider>(out Collider boneCollider))
                Physics.IgnoreCollision(playerCollider, boneCollider);
        }

        DisableRagDoll();
    }
    public bool GetIsWalking() { return isWalking; }
    public Vector3 GetLastMoveDirection() { return lastMoveDir; }
}
