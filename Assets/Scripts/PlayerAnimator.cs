using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    [SerializeField] private PropInteract propInteract;

    [Header("Rig Field")]
    [SerializeField] private Rig grabRig;
    [SerializeField] private Rig pushRig;

    private int upperBody = 1;

    private const string IS_WALKING = "isWalking";
    private const string IS_PUSHING = "IsPushing";

    private void OnEnable()
    {
        EventManager.Instance.OnItemPickedUp += UpdateGrabWeigth;
        EventManager.Instance.OnItemDroped += UpdateGrabWeigth;
        EventManager.Instance.OnPropPush += AnimatePush;
        EventManager.Instance.OnBombInteracted += BringBombUp;
    }
    private void Update()
    {
        animator.SetBool(IS_WALKING, player.GetIsWalking());
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

        Vector3 playerDir = cameraPos.position - bomb.transform.position;

        bomb.transform.localPosition = Vector3.zero;
        bomb.transform.localRotation = Quaternion.identity;

        EventManager.Instance.BombRepositionated();
    }

}
