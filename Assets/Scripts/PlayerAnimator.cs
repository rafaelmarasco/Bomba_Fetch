using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    [SerializeField] private PropInteract propInteract;

    [Header("Ragdoll Field")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform playerRagDollTransform;
    [SerializeField] private float knockdownTime;

    [Header("Rig Field")]
    [SerializeField] private Rig grabRig;
    [SerializeField] private Rig pushRig;

    private PlayerEventManager playerEventManager;

    private Rigidbody playerRb;

    private readonly int upperBody = 1;

    private const string IS_WALKING = "isWalking";
    private const string IS_PUSHING = "IsPushing";

    private bool isRagDoll;

    private void OnEnable()
    {
        playerEventManager = GetComponentInParent<PlayerEventManager>();
        playerRb = GetComponentInParent<Rigidbody>();

        playerEventManager.OnItemPickedUp += UpdateGrabWeigth;
        playerEventManager.OnItemDroped += UpdateGrabWeigth;
        playerEventManager.OnPropPush += AnimatePush;
        playerEventManager.OnBombInteracted += BringBombUp;
        playerEventManager.OnKnockDown += AnimateKnockdown;
    }
    private void Update()
    {
        animator.SetBool(IS_WALKING, player.GetIsWalking());

        if (Input.GetKeyDown(KeyCode.T) && !isRagDoll)
            player.EnableRagDoll();
        else if (Input.GetKeyDown(KeyCode.T) && isRagDoll)
            player.DisableRagDoll();
    }
    private void AnimatePush()
    {
        StartCoroutine(PushAnimation());
    }
    private IEnumerator PushAnimation()
    {
        float animDuration = 0.35f;

        animator.SetTrigger(IS_PUSHING);
        pushRig.weight = 1f;
        animator.SetLayerWeight(upperBody, 1f);

        yield return new WaitForSeconds(animDuration);

        pushRig.weight = 0f;
        animator.SetLayerWeight(upperBody, 0f);
    }
    private void UpdateGrabWeigth(Prop propInfo)
    {
        Size propSize = propInfo.PropSize;

        if (propSize == Size.medium || propSize == Size.large)
            grabRig.weight = propInteract.hasItem ? 1f : 0f;
        else
            grabRig.weight = 0f;

    }
    private void BringBombUp(Transform cameraPos, GameObject bomb)
    {
        grabRig.weight = 1f;

        //Vector3 playerDir = cameraPos.position - bomb.transform.position;

        bomb.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    private void AnimateKnockdown()
    {
        playerEventManager.StopMoving(true);
        StartCoroutine(KnockdownAnimation());   
    }

    private IEnumerator KnockdownAnimation()
    {
        yield return new WaitForSeconds(.2f);
        player.EnableRagDoll();
        yield return new WaitForSeconds(knockdownTime);
        player.DisableRagDoll();
    }
}
