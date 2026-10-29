using UnityEngine;

public class PropPickupHandler : MonoBehaviour
{
    private PropInteract propInteract;
    private PlayerEventManager playerEventManager;
    public GameObject HeldItem { get; private set; } = null;
    public bool HasItem => HeldItem != null;
    public bool IsBombInteracting { get; private set; } = false;

    [SerializeField] private Transform dropPoint;

    private void Awake()
    {
        propInteract = GetComponent<PropInteract>();
        playerEventManager = GetComponent<PlayerEventManager>();
    }

    public void PickUpProp(GameObject prop)
    {
        HeldItem = prop;

        //Collider gfxCollider = GetComponentInChildren<Collider>();
        //Collider propCollider = prop.GetComponent<Collider>();

        prop.TryGetComponent<Prop>(out Prop propInfo);

        propInfo.EnableReposition(propInteract);
        playerEventManager.ItemPickedUp(propInfo);
    }
    public void DropProp()
    {
        Prop propInfo = HeldItem.GetComponent<Prop>();
        
        if (propInfo.PropSize == Size.small)
        {
            HeldItem.transform.SetParent(dropPoint);
            HeldItem.transform.SetLocalPositionAndRotation(Vector3.zero, HeldItem.transform.rotation);
        }

        HeldItem.transform.SetParent(null);
        HeldItem = null;

        if (IsBombInteracting)
        {
            propInteract.minigameCanvas.gameObject.SetActive(false);
            playerEventManager.BombDroped();
            IsBombInteracting = false;
        }

        propInfo.DisableReposition();
        playerEventManager.ItemDroped(propInfo);
    }

}
