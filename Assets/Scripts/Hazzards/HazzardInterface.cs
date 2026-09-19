using UnityEngine;
using UnityEngine.InputSystem;

public class HazzardInterface : MonoBehaviour
{
    [SerializeField] private Hazzard hazzard;

    public float radius;

    private InteractArea interactArea;

    private void OnEnable()
    {
        interactArea = GetComponentInChildren<InteractArea>();
        interactArea.OnReadyToInteract += ReadyToInteract;
        interactArea.OnNotInRangeToInteract += NotInRangeToInteract;
    }

    private void ReadyToInteract(PlayerInput input)
    {
        input.actions["Interact"].performed += TurnHazzardOff;
    }

    private void NotInRangeToInteract(PlayerInput input)
    {
        input.actions["Interact"].performed -= TurnHazzardOff;
    }

    private void TurnHazzardOff(InputAction.CallbackContext obj)
    {
        hazzard.isOn = !hazzard.isOn;
    }

    private void OnDrawGizmosSelected()
    {
        Transform hazzardTarget = hazzard.gameObject.transform;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(hazzardTarget.position + Vector3.up * .7f, radius);

        Gizmos.DrawLine(transform.position, hazzardTarget.position + Vector3.up * .7f);
    }
}
