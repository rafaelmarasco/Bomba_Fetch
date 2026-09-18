using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractArea : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField] private List<PlayerInput> playersInside = new();

    public event Action<PlayerInput> OnReadyToInteract;
    public event Action<PlayerInput> OnNotInRangeToInteract;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerAnimator>())
        {
            PlayerInput input = other.gameObject.GetComponentInParent<PlayerInput>();

            if (input != null && !playersInside.Contains(input))
            {
                playersInside.Add(input);
                OnReadyToInteract?.Invoke(input);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerAnimator>())
        {
            PlayerInput input = other.gameObject.GetComponentInParent<PlayerInput>();

            if (input != null && playersInside.Remove(input))
            {
                OnNotInRangeToInteract?.Invoke(input);
                input = null;
            }
        }
    }
}
