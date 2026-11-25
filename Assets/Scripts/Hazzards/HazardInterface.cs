using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HazardInterface : MonoBehaviour
{
    [SerializeField] private List<Hazard> hazards;
    [SerializeField] private VFXAudio vfxAudio;

    public float radius;

    private InteractArea interactArea;

    private void Awake()
    {
        interactArea = GetComponentInChildren<InteractArea>();
        
    }

    private void OnEnable()
    {
        interactArea.OnReadyToInteract += ReadyToInteract;
        interactArea.OnNotInRangeToInteract += NotInRangeToInteract;
    }

    private void ReadyToInteract(PlayerInput input)
    {
        input.actions["Interact"].performed += TurnHazzardsOff;
    }

    private void NotInRangeToInteract(PlayerInput input)
    {
        input.actions["Interact"].performed -= TurnHazzardsOff;
    }

    private void TurnHazzardsOff(InputAction.CallbackContext obj)
    {
        vfxAudio.PlayVFX();
        foreach (var hazard in hazards)
        {
            hazard.isOn = !hazard.isOn;
            if (hazard.isOn)
                hazard.Glow.TurnOn();
            else
                hazard.Glow.TurnOff();
        }
    }

    private void OnDisable()
    {
        interactArea.OnReadyToInteract -= ReadyToInteract;
        interactArea.OnNotInRangeToInteract -= NotInRangeToInteract;
    }

    /*
    private void OnDrawGizmosSelected()
    {
        Transform hazzardTarget = hazard.gameObject.transform;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(hazzardTarget.position + Vector3.up * .7f, radius);

        Gizmos.DrawLine(transform.position, hazzardTarget.position + Vector3.up * .7f);
    }
    */
}
