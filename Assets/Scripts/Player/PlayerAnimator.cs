using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerEventManager playerEventManager;
    [SerializeField] private PropPickupHandler propPickupHandler;
    [SerializeField] private Player player;
    private Animator animator;
    private PropInteract propInteract;

    [Header("Ragdoll Field")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform playerRagDollTransform;
    [SerializeField] private Ragdoll ragdoll;

    [Header("Rig Field")]
    [SerializeField] private Rig grabRig;
    [SerializeField] private Rig pushRig;

    [SerializeField] private Transform headCamPos;

    private readonly int upperBody = 1;

    private const string IS_WALKING = "isWalking";
    private const string IS_PUSHING = "IsPushing";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        propInteract = GetComponentInParent<PropInteract>();
    }

    private void OnEnable()
    {
        playerEventManager.OnBombInteracted += BringBombUp;
        playerEventManager.OnItemPickedUp += UpdateGrabWeigth;
        playerEventManager.OnItemDroped += UpdateGrabWeigth;
        playerEventManager.OnKnockDown += AnimateKnockdown;
        playerEventManager.OnPropPush += AnimatePush;
    }
    private void Update()
    {
        animator.SetBool(IS_WALKING, player.IsMoving);
    }
    public void AnimatePush()
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
            grabRig.weight = propPickupHandler.HasItem ? 1f : 0f;

        else
            grabRig.weight = 0f;
    }
    private void BringBombUp(GameObject bomb)
    {
        grabRig.weight = 1f;
        bomb.transform.SetParent(propInteract.holdPointInteract);

        bomb.transform.localPosition = Vector3.zero;

        Vector3 toFaceDirecetion = headCamPos.position - bomb.transform.position;

        bomb.transform.localRotation = Quaternion.Euler(50.0000114f, 0f, 0f);

        // Quaternion.LookRotation(-toFaceDirecetion, Vector3.up) * 

    }
    private void AnimateKnockdown(float stunTime)
    {
        Debug.Log($"Animaçao de cair iniciada, com duraçao de {stunTime}");
        playerEventManager.StopInputingMovement(true);
        StartCoroutine(KnockdownAnimation(stunTime));
    }
    private IEnumerator KnockdownAnimation(float knockdownTime)
    {
        ragdoll.EnableRagDoll(out _);
        yield return new WaitForSeconds(knockdownTime);
        Debug.Log("Animaçao de cair finalizada");
        ragdoll.DisableRagDoll();
    }
}
