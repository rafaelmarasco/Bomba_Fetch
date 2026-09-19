using UnityEngine;
using System.Collections;

public class PlayerHarmHandler : MonoBehaviour
{
    private Player player;
    private Ragdoll ragdoll;
    private Rigidbody hipsRb;
    private Rigidbody playerRb;
    private PlayerEventManager playerEventManager;

    private bool isOnFireSequence = false;
    public bool CanGetPushed { get; private set; } = true;
    public bool IsOnFire { get; private set; } = false;

    private Vector3 runTarget;

    [SerializeField] private float jumpTime;
    [SerializeField] private float onFireMoveSpeed;

    private void Awake()
    {
        player = gameObject.GetComponent<Player>();
        playerEventManager = GetComponent<PlayerEventManager>();

        ragdoll = GetComponent<Ragdoll>();
        hipsRb = GetComponentInChildren<Rigidbody>();
        playerRb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerEventManager.OnEletrocuted += GetYonked;
        playerEventManager.OnBurned += GetBurned;
    }
    private void FixedUpdate()
    {
        if (IsOnFire)
            RunOnFire();
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
    public void StartKnockableColldownTimer(float duration)
    {
        CanGetPushed = false;
        CancelInvoke(nameof(ResetKnockableColldown));
        Invoke(nameof(ResetKnockableColldown), duration);
    }
    private void ResetKnockableColldown() => CanGetPushed = true;
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
        this.runTarget = runTarget;
        Vector3 directionToTarget = runTarget - playerRb.position;

        playerEventManager.StopInputingMovement(true);

        player.RotateOnMove(directionToTarget);
        playerRb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);

        yield return new WaitForSeconds(jumpTime);

        IsOnFire = true;

        yield return new WaitForSeconds(runningTime);

        IsOnFire = false;
        isOnFireSequence = false;
        playerEventManager.StopInputingMovement(false);
    }
    private void RunOnFire()
    {
        Vector3 directionToTarget = runTarget - playerRb.position;

        directionToTarget.y = 0f;
        directionToTarget.Normalize();

        playerRb.MovePosition(playerRb.position + onFireMoveSpeed * Time.fixedDeltaTime * directionToTarget);
    }
}
