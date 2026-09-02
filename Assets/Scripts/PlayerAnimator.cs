using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    [SerializeField] private PropInteract propInteract;

    [Header("Rig Field")]
    [SerializeField] private Rig grabRig;
    [SerializeField] private Rig pushRig;

    private InputSystem_Actions inputActions;

    private const string IS_WALKING = "isWalking";

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        inputActions.Player.Grab.performed += Grab_performed;
        inputActions.Player.Push.performed += Push_performed;
    }

    private void Push_performed(InputAction.CallbackContext context)
    {
        StartCoroutine(AnimatePush());
        UpdateHands();
    }

    private void Grab_performed(InputAction.CallbackContext obj)
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

    private IEnumerator AnimatePush()
    {
        float duration = 0.15f;
        float timePassed = 0;

        while(timePassed <= duration)
        {
            pushRig.weight = Mathf.Lerp(0f, 1f, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }

        timePassed = 0;

        while(timePassed <= duration)
        {
            pushRig.weight = Mathf.Lerp(1f, 0f, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }
    }
}
