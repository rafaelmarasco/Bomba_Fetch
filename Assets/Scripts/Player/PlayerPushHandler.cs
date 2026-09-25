using Unity.VisualScripting;
using UnityEngine;

public class PlayerPushHandler : MonoBehaviour
{

    [SerializeField] private float pushForce;

    PlayerEventManager playerEventManager;
    PropPickupHandler propPickupHandler;
    PropInteract propInteract;

    bool HasItem => propInteract.HasItem;

    private void Awake()
    {
        playerEventManager = GetComponent<PlayerEventManager>();
        propPickupHandler = GetComponent<PropPickupHandler>();
        propInteract = GetComponent<PropInteract>();
    }
    public void Push()
    {
        playerEventManager.PropPush();

        GameObject prop = null;
        GameObject player = null;

        if (!HasItem && (propInteract.CheckForProps(out prop) || 
            (!propInteract.IsBombInteracting && propInteract.CheckForPlayer(out player))))
        {
            if (prop != null && prop.TryGetComponent<Rigidbody>(out Rigidbody propRb))
                propRb.AddForce(pushForce * transform.forward, ForceMode.Impulse);

            else if (player != null)
            {
                float knockdownTime = 2f;

                Rigidbody hipsRb = player.GetComponent<Player>().HipsRb;

                player.GetComponent<PlayerEventManager>().KnockedDown(knockdownTime);
                hipsRb.AddForce(pushForce * transform.forward, ForceMode.Impulse);
            }
        }
        else if (HasItem)
        {
            propPickupHandler.HeldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb);
            propPickupHandler.DropProp();
            propRb.AddForce(pushForce * transform.forward, ForceMode.Impulse);
        }
    }
}
