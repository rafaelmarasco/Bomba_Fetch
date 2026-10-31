using UnityEngine;
using UnityEngine.InputSystem;

public class Extinguisher : MonoBehaviour, IHeldTool
{
    private Prop extinguisherProp;

    private PlayerAnimator playerAnimator;
    private PlayerEventManager playerEventManager;
    private PropInteract Holder => extinguisherProp.PlayerInteracting;

    private PlayerInput HolderInput;

    private FireExtinguisherSmoke smoke;

    private bool isEquipped;

    [SerializeField] private Vector3 rotationOffset = new(0f, -90f, -10f);
    [SerializeField] private Vector3 positionOffset = new(0f, 0f, 0f);

    private void Awake()
    {
        extinguisherProp = GetComponent<Prop>();
        smoke = GetComponentInChildren<FireExtinguisherSmoke>();
    }
    public void Use()
    {
        if (!isEquipped || smoke.IsSpewing) return;

        smoke.StartSpewing();
        playerEventManager.ToolInteracted(this.gameObject);
        transform.SetParent(Holder.holdPointInteract);
        transform.SetLocalPositionAndRotation(Vector3.zero + positionOffset, Quaternion.Euler(rotationOffset));
    }
    public void StopUsing()
    {
        if (!smoke.IsSpewing) return;

        smoke.StopSpewing();
        playerAnimator.BringToolDown(this.gameObject);
        extinguisherProp.EnableReposition(Holder);
    }
    public void Pickup()
    {
        isEquipped = true;

        if (HolderInput != null)
        {
            Debug.LogWarning($"O player: {Holder.name} já esta equipado com esse item");    
            return;
        }

        HolderInput = Holder.GetComponent<PlayerInput>();
        playerAnimator = Holder.GetComponentInChildren<PlayerAnimator>();
        playerEventManager = Holder.GetComponent<PlayerEventManager>();
    }
    public void Drop()
    {
        if (smoke.IsSpewing)
            smoke.StopSpewing();

        isEquipped = false;
        HolderInput = null;
        playerAnimator = null;
        playerEventManager = null;
    }
}
