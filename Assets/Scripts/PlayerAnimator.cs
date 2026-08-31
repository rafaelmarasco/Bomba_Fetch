using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    private const string IS_WALKING = "isWalking";

    private void Update()
    {
        animator.SetBool(IS_WALKING, player.GetIsWalking());
    }
}
