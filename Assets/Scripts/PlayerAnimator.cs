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
        EventManager.Instance.OnItemPickedUp += UpdateHands;
        EventManager.Instance.OnItemDroped += UpdateHands;
        EventManager.Instance.OnPropPush += AnimateHands;
    }
    private void Update()
    {
        animator.SetBool(IS_WALKING, player.GetIsWalking());
    }

    private void AnimateHands()
    {
        StartCoroutine(HandsAnimation());
    }
    private IEnumerator HandsAnimation()
    {
        float animDuration = 0.35f;
        
        animator.SetTrigger(IS_PUSHING);
        pushRig.weight = 1f;
        animator.SetLayerWeight(upperBody, 1f);

        yield return new WaitForSeconds(animDuration);

        pushRig.weight = 0f;
        animator.SetLayerWeight(upperBody, 0f);
    }
    private void UpdateHands()
    {
        grabRig.weight = propInteract.hasItem ? 1 : 0;
    }

}
