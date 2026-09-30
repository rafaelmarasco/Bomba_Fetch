using UnityEngine;
using UnityEngine.InputSystem;

public class Extinguisher : MonoBehaviour
{
    private Prop extinguisherProp;
    private PropInteract PlayerEquiped => extinguisherProp.PlayerInteracting;

    private PlayerInput playerInput;

    private FireExtinguisherSmoke fireExtinguisherSmoke;

    [SerializeField] private bool isEquiped;
    private void Awake()
    {
        extinguisherProp = GetComponent<Prop>();
        fireExtinguisherSmoke = GetComponentInChildren<FireExtinguisherSmoke>();
    }

    private void Update()
    {
        if (!isEquiped)
        {
            playerInput = null;

            if (fireExtinguisherSmoke.IsSpewing)
                fireExtinguisherSmoke.StopSmoke();

            return;
        }

        if (playerInput == null) 
            playerInput = PlayerEquiped.GetComponent<PlayerInput>();

        HandleSmokeState();
    }

    private void HandleSmokeState()
    {

        if (playerInput.actions["interact"].IsPressed() && !fireExtinguisherSmoke.IsSpewing)
        {
            fireExtinguisherSmoke.StartSmoke();
            transform.SetParent(PlayerEquiped.holdPointInteract);
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.LookRotation(-PlayerEquiped.transform.forward));
        }
        else if (playerInput.actions["interact"].WasReleasedThisFrame())
        {
            fireExtinguisherSmoke.StopSmoke();
            extinguisherProp.EnableReposition(PlayerEquiped);
        }
    }

    public void SetEquipedState(bool state) => isEquiped = state;
}
