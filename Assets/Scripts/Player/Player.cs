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

    [SerializeField] private float moveSpeed = 7f;

    private Vector3 rawInput;

    private Vector3 runTarget;
    public Vector3 MoveDir { get; private set; }
    public Vector3 LastMoveDir { get; private set; } = Vector3.forward;

    public bool IsMoving => MoveDir != Vector3.zero;
    public bool canGetPushed = true;
    public bool isOnFire = false;
    private bool stopMoving;
    private bool isOnFireSequence = false;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerEventManager = GetComponent<PlayerEventManager>();
    }

    private void OnEnable()
    {
        playerEventManager.OnStopedMoving += StopMoving;
        playerEventManager.OnEletrocuted += GetYonked;
        playerEventManager.OnBurned += GetBurned;
    }
    private void FixedUpdate()
    {
        if (!stopMoving)
        {
            ReadInput();
            RotateOnMove();
            BasicMove();
        }

        if (isOnFire)
            RunOnFire();

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
    private void RotateOnMove()
    {
        if (rawInput != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rawInput, Vector3.up);
    }
    private void StopMoving(bool stopMoving)
    {
        this.stopMoving = stopMoving;
    }
    private void GetYonked(Vector3 flyDirection, Vector3 propFlyDirection, float flyForce, float stunTime)
    {
        ragdoll.EnableRagDoll(out Rigidbody heldItem);

        if (heldItem != null)
        {
            float propFlyForce = heldItem.gameObject.GetComponent<Prop>().ThrowForce;
            heldItem.AddForce(propFlyDirection * propFlyForce, ForceMode.Impulse);
        }

        hipsRb.AddForce(flyDirection * flyForce, ForceMode.Impulse);
        playerEventManager.KnockedDown(stunTime);
    }
    private void GetBurned(Vector3 jumpDirection, Vector3 runTarget, float jumpForce, float runningTime)
    {
        Debug.Log($"jumpDirection: {jumpDirection}, jumpForce: {jumpForce}, runTarget: {runTarget}");
        if (isOnFireSequence)
            return;

        isOnFireSequence = true;
        StartCoroutine(OnFireSequence(jumpDirection, runTarget, jumpForce, runningTime));
    }

    private IEnumerator OnFireSequence(Vector3 jumpDirection, Vector3 runTarget, float jumpForce, float runningTime)
    {
        float jumpTime = 1f;
        this.runTarget = runTarget;

        playerEventManager.StopInputingMovement(true);

        Debug.Log($"isKinematic: {playerRb.isKinematic}, mass: {playerRb.mass}, drag: {playerRb.linearDamping}, constraints: {playerRb.constraints}, useGravity: {playerRb.useGravity}");

        playerRb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);

        yield return new WaitForSeconds(jumpTime);

        isOnFire = true;

        yield return new WaitForSeconds(runningTime);

        isOnFire = false;
        isOnFireSequence = false;
        playerEventManager.StopInputingMovement(false);
    }

    private void RunOnFire()
    {
        float onFireMoveSpeed = 7f;
        Vector3 directionToTarget = runTarget - playerRb.position;
        directionToTarget.y = 0f;
        directionToTarget.Normalize();

        playerRb.MovePosition(playerRb.position + onFireMoveSpeed * Time.fixedDeltaTime * directionToTarget);
    }

    public void StartKnockableColldownTimer(float duration)
    {
        canGetPushed = false;
        CancelInvoke(nameof(ResetKnockableColldown));
        Invoke(nameof(ResetKnockableColldown), duration);
    }
    private void ResetKnockableColldown() => canGetPushed = true;
}
