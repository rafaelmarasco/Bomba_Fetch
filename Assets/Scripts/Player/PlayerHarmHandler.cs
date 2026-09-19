using UnityEngine;
using System.Collections;

public class PlayerHarmHandler : MonoBehaviour
{
    private Ragdoll ragdoll;
    private Rigidbody hipsRb;
    private Rigidbody playerRb;
    private PlayerEventManager playerEventManager;

    public bool canGetPushed = true;
    public bool isOnFire = false;
    private bool isOnFireSequence = false;

    private Vector3 runTarget;

    private void FixedUpdate()
    {
        if (isOnFire)
            RunOnFire();
    }
    private void Awake()
    {
        ragdoll = GetComponent<Ragdoll>();
        hipsRb = GetComponentInChildren<Rigidbody>();
        playerRb = GetComponent<Rigidbody>();
        playerEventManager = GetComponent<PlayerEventManager>();
    }

    private void OnEnable()
    {
        playerEventManager.OnEletrocuted += GetYonked;
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
        canGetPushed = false;
        CancelInvoke(nameof(ResetKnockableColldown));
        Invoke(nameof(ResetKnockableColldown), duration);
    }
    private void ResetKnockableColldown() => canGetPushed = true;

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
}
