using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    [SerializeField] private PropInteract propInteract;
    [SerializeField] private Rig grabRig;
    private const string IS_WALKING = "isWalking";

    private void Update()
    {
        animator.SetBool(IS_WALKING, player.GetIsWalking());

        if (propInteract.hasItem)
            grabRig.weight = 1f;
        else 
            grabRig.weight = 0f;
    }
}
