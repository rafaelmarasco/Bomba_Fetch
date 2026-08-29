using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    [SerializeField] private PropInteract propInteract;
    private const string IS_WALKING = "isWalking";
    //private const string IS_CARRYING = "isCarrying";

    private void Update()
    {
        animator.SetBool(IS_WALKING, player.GetIsWalking());

      //  animator.SetBool(IS_CARRYING, propInteract.hasItem);
    }
}
