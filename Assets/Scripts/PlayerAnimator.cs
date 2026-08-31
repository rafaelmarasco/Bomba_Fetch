using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    [SerializeField] private PropInteract propInteract;
    [SerializeField] private Rig grabRig;
    private InputSystem_Actions inputActions;
    private const string IS_WALKING = "isWalking";

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += Interact_performed;
    }
    private void Interact_performed(InputAction.CallbackContext obj)
    {
        UpdateHands();
    }
    private void Update()
    {
        animator.SetBool(IS_WALKING, player.GetIsWalking());
    }

    private void UpdateHands()
    {
        if (propInteract.hasItem)
            grabRig.weight = 1f;
        else
            grabRig.weight = 0f;
    }
}
