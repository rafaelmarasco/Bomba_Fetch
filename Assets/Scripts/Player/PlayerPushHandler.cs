using System.Collections;
using UnityEngine;

public class PlayerPushHandler : MonoBehaviour
{

    [SerializeField] private float pushForce;

    public bool instantPlayerPushMode;
    private bool firstPush = false;
    private bool secondPush = false;
    private bool finalPush = false;

    private PlayerEventManager playerEventManager;
    private PropPickupHandler propPickupHandler;
    private PropInteract propInteract;

    private bool IsHoldingItem => propInteract.HasItem;

    private void Awake()
    {
        playerEventManager = GetComponent<PlayerEventManager>();
        propPickupHandler = GetComponent<PropPickupHandler>();
        propInteract = GetComponent<PropInteract>();
    }
    public void Push()
    {
        playerEventManager.PropPush();

        bool hasPropInRange = propInteract.CheckForProps(out GameObject prop);
        bool isInteractingWithBomb = propInteract.IsBombInteracting;
        bool hasPlayerInRange = propInteract.CheckForPlayer(out GameObject player);

        // atualmente o player nao consegue empurrar outro player se estiver carregando algum item

        if ((hasPropInRange || IsHoldingItem) || (!isInteractingWithBomb && hasPlayerInRange))
        {
            if (prop != null || IsHoldingItem)
                PushProp(prop);

            else if (player != null)
                PushPlayer(player);
        }
    }

    private void PushProp(GameObject propInRange)
    {
        Debug.Log(IsHoldingItem);
        if (!IsHoldingItem)
        {
            propInRange.TryGetComponent<Rigidbody>(out Rigidbody propRb);
            if (propRb == null) return;

            propRb.AddForce(pushForce * transform.forward, ForceMode.Impulse);
        }
        else
        {
            propPickupHandler.HeldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb);
            propPickupHandler.DropProp();
            propRb.AddForce(pushForce * transform.forward, ForceMode.Impulse);
        }
    }

    private void PushPlayer(GameObject player)
    {
        Rigidbody hipsRb = player.GetComponent<Player>().HipsRb;
        PlayerEventManager playerEventManager = player.GetComponent<PlayerEventManager>();

        if (player.GetComponent<PlayerPushHandler>().instantPlayerPushMode) // TROCAR SE FOR PERMANENTE
        {
            float knockdownTime = 2f;
            playerEventManager.KnockedDown(knockdownTime);
            hipsRb.AddForce(pushForce * transform.forward, ForceMode.Impulse);
        }

        else
        {
            float knockdownTime = 3f;
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (!firstPush)
            {
                playerRb.AddForce((pushForce) * transform.forward, ForceMode.Impulse);

                firstPush = true;
                StartCoroutine(ResetPushState(nameof(firstPush)));
            }
            else if (!secondPush)
            {
                playerRb.AddForce((pushForce * 1.5f) * transform.forward, ForceMode.Impulse);

                secondPush = true;
                StartCoroutine(ResetPushState(nameof(secondPush)));
            }
            else if (!finalPush)
            {
                playerEventManager.KnockedDown(knockdownTime);

                hipsRb.AddForce(pushForce * transform.forward, ForceMode.Impulse);

                finalPush = true;
                StartCoroutine(ResetPushState(nameof(finalPush)));
            }
        }
    }

    private IEnumerator ResetPushState(string stateName)
    {

        float cooldownTime = stateName switch
        {
            nameof(firstPush) => 6f,
            nameof(secondPush) => 4f,
            nameof(finalPush) => 3f,
            _ => 0f
        };

        yield return new WaitForSeconds(cooldownTime);

        switch (stateName)
        {
            case nameof(firstPush):
                firstPush = false;
                break;

            case nameof(secondPush):
                secondPush = false;
                break;

            case nameof(finalPush):
                finalPush = false;
                break;
        }
        /*float cooldownTime;

        switch (stateName)
        {
            case nameof(firstPush):
                cooldownTime = 4f;
                break;

            case nameof(secondPush):
                cooldownTime = 5f;
                break; 
            
            case nameof(finalPush):
                cooldownTime = 6f;
                break;

            default:
                cooldownTime = 0f;
                break;
        }

        yield return new WaitForSeconds(cooldownTime);
        */
    }
}
