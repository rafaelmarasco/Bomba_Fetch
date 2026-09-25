using UnityEngine;
using System.Collections;

public class PlayerHarmHandler : MonoBehaviour
{
    private Player player;
    private Ragdoll ragdoll;
    private PropPickupHandler propPickupHandler;
    private PlayerPushHandler playerPushHandler;
    private PlayerEventManager playerEventManager;

    private Rigidbody HipsRb => player.HipsRb;
    private Rigidbody playerRb;

    private bool isOnFireSequence = false;
    public bool CanGetPushed { get; private set; } = true;
    public bool IsOnFire { get; private set; } = false;

    private Vector3 initialRunTarget;
    private Vector3 dangerPos;

    [SerializeField] private float jumpTime;
    [SerializeField] private float onFireMoveSpeed;


    private void Awake()
    {
        playerEventManager = GetComponent<PlayerEventManager>();
        propPickupHandler = GetComponent<PropPickupHandler>();
        playerPushHandler = GetComponent<PlayerPushHandler>();
        player = GetComponent<Player>();

        ragdoll = GetComponent<Ragdoll>();
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
            RunOnFire(dangerPos);
    }
    private void GetYonked(Vector3 flyDirection, Vector3 propFlyDirection, float flyForce, float stunTime)
    {
        ragdoll.EnableRagDoll(out Rigidbody heldItem);

        if (heldItem != null)
        {
            float propFlyForce = heldItem.gameObject.GetComponent<Prop>().ThrowForce;
            heldItem.AddForce(propFlyDirection * propFlyForce, ForceMode.Impulse);
        }

        HipsRb.AddForce(flyDirection * flyForce, ForceMode.Impulse);
        playerEventManager.KnockedDown(stunTime);
    }
    public void StartKnockableColldownTimer(float duration)
    {
        CanGetPushed = false;
        CancelInvoke(nameof(ResetKnockableColldown));
        Invoke(nameof(ResetKnockableColldown), duration);
    }
    private void ResetKnockableColldown() => CanGetPushed = true;
    private void GetBurned(Vector3 jumpDirection, Vector3 runTarget, float jumpForce, float runningTime, Vector3 firePos, float stunTime)
    {
        dangerPos = firePos;

        if (isOnFireSequence)
            return;

        isOnFireSequence = true;
        StartCoroutine(OnFireSequence(jumpDirection, runTarget, jumpForce, runningTime, stunTime));
    }
    private IEnumerator OnFireSequence(Vector3 jumpDirection, Vector3 runTarget, float jumpForce, float runningTime, float stunTime)
    {
        this.initialRunTarget = runTarget;
        Vector3 directionToTarget = runTarget - playerRb.position;

        playerEventManager.StopInputingMovement(true);

        player.RotateOnMove(directionToTarget);
        playerRb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);

        if (propPickupHandler.HeldItem != null)
            playerPushHandler.Push();

        yield return new WaitForSeconds(jumpTime);

        IsOnFire = true;

        yield return new WaitForSeconds(runningTime);

        IsOnFire = false;
        isOnFireSequence = false;
        playerEventManager.KnockedDown(stunTime);
    }
    private void RunOnFire(Vector3 dangerPos)
    {
        Vector3 directionToTarget = initialRunTarget - playerRb.position;
        directionToTarget.y = 0f;

        float distanceFromTarget = directionToTarget.magnitude;

        if (distanceFromTarget <= .2f)
        {
            initialRunTarget = SelectRandonDirection();
            directionToTarget = initialRunTarget - playerRb.position;
            directionToTarget.y = 0f;
        }

        directionToTarget.Normalize();

        player.RotateOnMove(directionToTarget);
        playerRb.MovePosition(playerRb.position + onFireMoveSpeed * Time.fixedDeltaTime * directionToTarget);
    }

    private Vector3 SelectRandonDirection()
    {
        Vector3 randonDirection;
        Vector3 directionToDanger = (dangerPos - playerRb.position).normalized;
        float dot;

        do
        {
            float randonAngle = Random.Range(0f, 360f);
            randonDirection = Quaternion.Euler(0f, randonAngle, 0f) * Vector3.forward;

            dot = Vector3.Dot(randonDirection, directionToDanger);
        } while (dot > -.4f);

        float randonDistance = Random.Range(3f, 5f);

        return playerRb.position + randonDirection * randonDistance;
    }
}
