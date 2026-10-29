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

    private Coroutine firstPushState;
    private Coroutine secondPushState;

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
        bool isInteractingWithBomb = propInteract.ToolInHand is Bomb; // --- Problema
        bool hasPlayerInRange = propInteract.CheckForPlayer(out GameObject player);

        // atualmente o player nao consegue empurrar outro player se estiver carregando algum item

        if ((hasPropInRange || IsHoldingItem) || (!isInteractingWithBomb && hasPlayerInRange))
        {
            if (IsHoldingItem)
                PushProp(propPickupHandler.HeldItem);
            
            else if (prop != null)
                PushProp(prop);

            else if (player != null)
                PushPlayer(player);
        }
    }

    private void PushProp(GameObject prop)
    {
        Debug.Log($"Acabo de arremessar um {prop.name}");

        Prop propBeingThrown;

        prop.TryGetComponent(out propBeingThrown);

        if (propBeingThrown == null) Debug.LogWarning($"somehow {propBeingThrown.name} is not a prop!");

        if (!IsHoldingItem)
        {
            prop.TryGetComponent<Rigidbody>(out Rigidbody propRb);
            if (propRb == null) return;

            propBeingThrown.SetPushingState(true);

            propRb.AddForce(pushForce * transform.forward, ForceMode.Impulse);
        }
        else
        {
            propPickupHandler.HeldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb);

            propBeingThrown.SetPushingState(true);

            propPickupHandler.DropProp();
            propRb.AddForce(pushForce * transform.forward, ForceMode.Impulse);
        }

        if (propInteract.ToolInHand is ICanvasTool canvasTool)
            canvasTool.DisableCanvas();

    }

    private void PushPlayer(GameObject player)
    {
        Rigidbody hipsRb = player.GetComponent<Player>().HipsRb;
        PlayerEventManager targetPlayerEventManager = player.GetComponent<PlayerEventManager>();

        if (player.GetComponent<PlayerPushHandler>().instantPlayerPushMode) // TROCAR SE FOR PERMANENTE
        {
            float knockdownTime = 2f;
            targetPlayerEventManager.KnockedDown(knockdownTime);
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
                firstPushState = StartCoroutine(ResetPushState(nameof(firstPush)));
            }
            else if (!secondPush)
            {
                playerRb.AddForce((pushForce * 1.5f) * transform.forward, ForceMode.Impulse);

                secondPush = true;
                secondPushState = StartCoroutine(ResetPushState(nameof(secondPush)));
            }
            else if (!finalPush)
            {
                targetPlayerEventManager.KnockedDown(knockdownTime);

                hipsRb.AddForce(pushForce * transform.forward, ForceMode.Impulse);

                finalPush = true;
                if (firstPushState != null) StopCoroutine(firstPushState);
                if (secondPushState != null) StopCoroutine(secondPushState);
                ResetPushStates();

            }
        }
    }

    private IEnumerator ResetPushState(string stateName)
    {

        float cooldownTime = stateName switch
        {
            nameof(firstPush) => 4f,
            nameof(secondPush) => 4.2f,
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
        }
    }

    private void ResetPushStates()
    {
        firstPush = false;
        secondPush = false;
        finalPush = false;
    }
}
