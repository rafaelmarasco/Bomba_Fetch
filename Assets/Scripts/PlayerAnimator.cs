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

    Coroutine pushCoroutine;

    private const string IS_WALKING = "isWalking";
    
    private void OnEnable()
    {
        EventManager.Instance.OnItemPickedUp += UpdateHands;
        EventManager.Instance.OnItemDroped += UpdateHands;
        EventManager.Instance.OnPropPush += HandlePropPush;
    }
    private void Update()
    {
        animator.SetBool(IS_WALKING, player.GetIsWalking());
    }
    private void HandlePropPush()
    {
        if (pushCoroutine != null)
            StopCoroutine(pushCoroutine);
        pushCoroutine = StartCoroutine(AnimatePush());
    }
    private void UpdateHands()
    {
        grabRig.weight = propInteract.hasItem ? 1 : 0;
    }
    private IEnumerator AnimatePush()
    {
        float duration = 0.15f;
        float timePassed = 0f;

        float animEnd = 1f;
        float animBegin = 0f;

        while (timePassed <= duration)
        {
            pushRig.weight = Mathf.Lerp(animBegin, animEnd, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }
        pushRig.weight = animEnd;

        timePassed = 0;

        while (timePassed <= duration)
        {
            pushRig.weight = Mathf.Lerp(animEnd, animBegin, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }
        pushRig.weight = animBegin;
    }
}
