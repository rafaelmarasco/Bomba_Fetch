using UnityEngine;

public class PropPickupHandler : MonoBehaviour
{
    private PropInteract propInteract;
    private PlayerEventManager playerEventManager;
    public GameObject HeldItem { get; private set; } = null;
    public bool HasItem => HeldItem != null;
    public bool HasBomb { get; private set; } = false;
    public bool IsBombInteracting { get; private set; } = false;

    private void OnEnable()
    {
        propInteract.OnPropDropped += SetBombState;
    }

    private void Awake()
    {
        propInteract = GetComponent<PropInteract>();
        playerEventManager = GetComponent<PlayerEventManager>();
    }

    public void PickUpProp(GameObject prop)
    {
        const int PROP_IN_HAND_LAYER = 7;

        HeldItem = prop;

        Collider gfxCollider = GetComponentInChildren<Collider>();
        Collider propCollider = prop.GetComponent<Collider>();

        prop.TryGetComponent<Prop>(out Prop propInfo);

        HasBomb = prop.name == "Bomb"; // Trocar para script quando a bomba tiver um script

        // if (prop.TryGetComponent<Rigidbody>(out Rigidbody propRb))
        //    propRb.isKinematic = true;

        prop.layer = PROP_IN_HAND_LAYER;

        //Physics.IgnoreCollision(gfxCollider, propCollider);

        propInfo.EnableReposition(propInteract.holdPointSmall, propInteract.holdPointMedium);
        playerEventManager.ItemPickedUp(propInfo);
    }
    public void DropProp()
    {
        const int DEFAULT_LAYER = 0;

        Collider gfxCollider = GetComponentInChildren<Collider>();
        Collider propCollider = HeldItem.GetComponent<Collider>();

        Prop propInfo = HeldItem.GetComponent<Prop>();

        float zOffset = .2f;
        float yOffset = .3f;

        Vector3 offset = new Vector3(0f, yOffset, zOffset);

        // if (HeldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb))
        //    propRb.isKinematic = false;

        HeldItem.layer = DEFAULT_LAYER;

        HeldItem.transform.localPosition += offset;
        HeldItem.transform.SetParent(null);
        HeldItem = null;

        if (IsBombInteracting)
        {
            propInteract.minigameCanvas.gameObject.SetActive(false);
            playerEventManager.BombDroped();
            IsBombInteracting = false;
        }

        //Physics.IgnoreCollision(gfxCollider, propCollider, false);

        propInfo.DisableReposition();
        playerEventManager.ItemDroped(propInfo);
    }

    private void SetBombState()
    {
        if (HasBomb)
        {
            HasBomb = false;
            playerEventManager.BombDroped();
        }
    }
}
