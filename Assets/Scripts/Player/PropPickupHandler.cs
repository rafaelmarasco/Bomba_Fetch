using UnityEngine;

public class PropPickupHandler : MonoBehaviour
{
    private PlayerInputHendler playerInputs;
    private PlayerEventManager playerEventManager;
    public GameObject HeldItem { get; private set; } = null;
    public bool HasItem => HeldItem != null;

    [SerializeField] private Transform dropPoint;

    private void Awake()
    {
        playerInputs = GetComponent<PlayerInputHendler>();
        playerEventManager = GetComponent<PlayerEventManager>();
    }
    public void TryPickUpProp(bool hasItem)
    {
        if (hasItem)
            DropProp();
        else
        {
            if (!playerInputs.CheckForProps(out GameObject prop)) return;
            PickUpProp();
        }
    }
    public void PickUpProp()
    {
        if (!playerInputs.CheckForProps(out GameObject prop)) return;

        HeldItem = prop;

        prop.TryGetComponent(out Prop propInfo);

        propInfo.EnableReposition(playerInputs);
        playerEventManager.ItemPickedUp(propInfo);

        playerInputs.SetToolInHand(prop);
    }
    public void DropProp()
    {
        playerInputs.ToolInHand?.Drop();
        playerInputs.SetToolInHand(null);

        Prop propInfo = HeldItem.GetComponent<Prop>();
        
        if (propInfo.PropSize == Size.small)
        {
            HeldItem.transform.SetParent(dropPoint);
            HeldItem.transform.localPosition = Vector3.zero;
        }

        HeldItem.transform.SetParent(null);
        HeldItem = null;

        if (playerInputs.ToolInHand is Bomb)
            playerEventManager.BombDroped();

        playerInputs.PropDropped();

        propInfo.DisableReposition();
        playerEventManager.ItemDroped(propInfo);
    }
}
