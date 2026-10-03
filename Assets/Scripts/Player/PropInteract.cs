using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PropInteract : MonoBehaviour
{
    private PlayerEventManager playerEventManager;
    private PropPickupHandler propPickupHandler;
    private PlayerPushHandler playerPushHandler;
    private PlayerInput playerInput;
    private Player player;

    [SerializeField] private Transform checkPos;
    [SerializeField] private Transform headPos;
    public bool IsBombInteracting { get; private set; } = false;

    private Vector3 lastMoveDir;
    private GameObject HeldItem => propPickupHandler.HeldItem;

    private IHeldTool toolInHand;
    public bool HasItem => HeldItem != null;

    [Header("Prop Holding Points")]
    [SerializeField] public Transform holdPointSmall;
    [SerializeField] public Transform holdPointMedium;
    [SerializeField] private Transform holdPointLarge;
    [SerializeField] public Transform holdPointInteract;

    [Header("BoxCastConfigs")]
    [SerializeField] private Vector3 halfExtends = new(.5f, 0.1f, .4f);
    [SerializeField] private float interactDistance = .8f;

    [Header("Minigame")]
    public Canvas minigameCanvas { get; private set; }

    public event Action OnPropDropped;


    private void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        propPickupHandler = GetComponent<PropPickupHandler>();
        playerPushHandler = GetComponent<PlayerPushHandler>();
        playerEventManager = GetComponent<PlayerEventManager>();

        playerInput.actions["Grab"].performed += Grab_performed;
        playerInput.actions["Push"].performed += Push_performed;
        playerInput.actions["Interact"].performed += Interact_performed;
        playerInput.actions["Interact"].canceled += Interact_canceled;
    }



    private void Update()
    {
        lastMoveDir = player.LastMoveDir;
    }
    private void Interact_performed(InputAction.CallbackContext obj) => toolInHand?.Use();

    /*
{

    if (propPickupHandler.HasBomb && !IsBombInteracting)
    {
        //HeldItem.transform.SetParent(holdPointInteract);
        // In this function mean that player has bomb and he is holding it
        minigameCanvas.gameObject.SetActive(true);
        IsBombInteracting = true;
        playerEventManager.BombInteracted(headPos, HeldItem);
    }
}
    */
    private void Interact_canceled(InputAction.CallbackContext context) => toolInHand?.StopUsing();
    private void Push_performed(InputAction.CallbackContext obj) => playerPushHandler.Push();
    private void Grab_performed(InputAction.CallbackContext obj)
    {
        if (!HasItem && CheckForProps(out GameObject prop))
        {
            Debug.Log("Pegou");
            propPickupHandler.PickUpProp(prop);
            SetToolInHand(prop);
        }
        else if (HasItem)
        {
            toolInHand?.Drop();
            toolInHand = null;

            propPickupHandler.DropProp();
            OnPropDropped?.Invoke();
        }
    }
    private void SetToolInHand(GameObject prop)
    {
        if (!prop.TryGetComponent(out IHeldTool tool)) return;

        Debug.Log("Item pego!!");

        toolInHand = tool;
        toolInHand.Pickup();
    }
    public bool CheckForPlayer(out GameObject player)
    {
        player = null;

        bool foundAnyObject = Physics.BoxCast
            (checkPos.position - lastMoveDir * .2f, halfExtends, lastMoveDir, out RaycastHit hit, checkPos.rotation, interactDistance);

        if (!foundAnyObject) return false;

        Player playerFound = hit.collider.gameObject.GetComponentInParent<Player>();

        if (playerFound == null)
            return false;

        player = playerFound.gameObject;

        return true;
    }
    public bool CheckForProps(out GameObject prop) // Check if theres an object in front of the player
    {
        GameObject objectInRange = null;

        bool findAnyObject = Physics.BoxCast
            (checkPos.position - lastMoveDir * .2f, halfExtends, lastMoveDir, out RaycastHit hit, checkPos.rotation, interactDistance);

        bool isProp = findAnyObject && hit.collider.gameObject.TryGetComponent<Prop>(out _);

        if (isProp)
            objectInRange = hit.collider.gameObject;

        prop = objectInRange;

        return isProp;
    }
    private void OnDestroy()
    {
        playerInput.actions["Grab"].performed -= Grab_performed;
        playerInput.actions["Push"].performed -= Push_performed;
        playerInput.actions["Interact"].performed -= Interact_performed;
        playerInput.actions["Interact"].canceled -= Interact_canceled;
    }
}
