using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class Player : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PropInteract propInteract;
    [SerializeField] private Ragdoll ragdoll;
    private PlayerEventManager playerEventManager;

    [SerializeField] private Animator animator;

    private Rigidbody playerRb;

    [SerializeField] private Rigidbody hipsRb;
    public Rigidbody HipsRb => hipsRb;

    [SerializeField] private float moveSpeed = 7f;

    private Vector3 rawInput;

    public Vector3 MoveDir { get; private set; }
    public Vector3 LastMoveDir { get; private set; } = Vector3.forward;

    public bool IsMoving => MoveDir != Vector3.zero;
    private bool stopMoving;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerEventManager = GetComponent<PlayerEventManager>();
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
        playerRb.MovePosition(playerRb.position + moveSpeed * Time.fixedDeltaTime * MoveDir);
    }
    public void RotateOnMove(Vector3 input)
    {
        if (input != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(input, Vector3.up);
    }
    private void StopMoving(bool stopMoving)
    {
        this.stopMoving = stopMoving;
    }
}
