using UnityEngine;
public class Player : MonoBehaviour
{

    private PlayerMovement playerMovement;

    private PlayerEventManager playerEventManager;

    [SerializeField] private Animator animator;

    private Rigidbody playerRb;

    [SerializeField] private Rigidbody hipsRb;
    public Rigidbody HipsRb => hipsRb;


    private Vector3 rawInput;
    public Vector3 MoveDir { get; private set; }
    public Vector3 LastMoveDir { get; private set; } = Vector3.forward;

    public bool IsMoving => MoveDir != Vector3.zero;
    private bool stopMoving;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float maxForce = 1000f;

    private void Awake()
    {
        playerEventManager = GetComponent<PlayerEventManager>();
        playerMovement = GetComponent<PlayerMovement>();

        playerRb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerEventManager.OnStopedMoving += StopMoving;
    }
    private void FixedUpdate()
    {
        if (!stopMoving)
        {
            ReadInput();
            RotateOnMove(rawInput);
            BasicMove();
        }
    }
    private void Update()
    {
        LastMoveDir = MoveDir != Vector3.zero ? MoveDir : LastMoveDir;
    }
    private void ReadInput()
    {
        Vector2 playerInput = playerMovement.GetMovementVectorNormalized();
        rawInput = new Vector3(playerInput.x, 0f, playerInput.y);
    }
    private void BasicMove()
    {
        MoveDir = rawInput;
        Vector3 nextVelocity = moveSpeed * MoveDir;
        Vector3 currentVelocity = playerRb.linearVelocity;

        Vector3 velocityDiff = nextVelocity - new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        Vector3 walkingForce = Vector3.ClampMagnitude(velocityDiff * acceleration * playerRb.mass, maxForce);

        playerRb.AddForce(walkingForce, ForceMode.Force);

    }
    public void RotateOnMove(Vector3 input)
    {
        if (input == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(input, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
    private void StopMoving(bool stopMoving)
    {
        this.stopMoving = stopMoving;
    }
}
