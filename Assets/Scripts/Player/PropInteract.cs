using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PropInteract : MonoBehaviour
{
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
    [SerializeField] public Transform holdPointInteract;

    [Header("BoxCastConfigs")]
    [SerializeField] private Vector3 halfExtends = new(.5f, 0.1f, .4f);
    [SerializeField] private float interactDistance = .8f;

    [Header("Minigame")]
    public Canvas minigameCanvas;
    private bool isBombInteracting = false;

    public event Action OnPropDropped;

    [Header("Password")]
    [SerializeField] private Canvas passwordCanvas;
    private bool isPasswordInteracting = false;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        propPickupHandler = GetComponent<PropPickupHandler>();
        playerPushHandler = GetComponent<PlayerPushHandler>();

        playerInput.actions["Grab"].performed += Grab_performed;
        playerInput.actions["Push"].performed += Push_performed;
        playerInput.actions["Interact"].performed += Interact_performed;
        playerInput.actions["Interact"].canceled += Interact_canceled;
    }

    private void Start()
    {
        passwordCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        lastMoveDir = player.LastMoveDir;
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        EnableCanvas();

        toolInHand?.Use();
    }

    private void Interact_canceled(InputAction.CallbackContext context) => toolInHand?.StopUsing();

    private void Push_performed(InputAction.CallbackContext obj) => playerPushHandler.Push();

    private void Grab_performed(InputAction.CallbackContext obj)
    {
        if (!HasItem && CheckForProps(out GameObject prop))
        {
            Debug.Log("Pegou");
            propPickupHandler.PickUpProp(prop);
            SetToolInHand(prop);
            CheckIsBombOrPassoword(prop);
        }
        else if (HasItem)
        {
            toolInHand?.Drop();
            toolInHand = null;

            DisableCanvas();

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

    private void CheckIsBombOrPassoword(GameObject prop)
    {
        isPasswordInteracting = prop.name == "Password";
        isBombInteracting = prop.name == "Bomb";
    }

    private void EnableCanvas()
    {
        if (isPasswordInteracting)
        {
            passwordCanvas.gameObject.SetActive(true);
            return;
        }
        else if (isBombInteracting)
        {
            minigameCanvas.gameObject.SetActive(true);
            return;
        }
    }

    public void DisableCanvas()
    {
        if (isPasswordInteracting)
        {
            passwordCanvas.gameObject.SetActive(false);
            isPasswordInteracting = false;
        }
        else if (isBombInteracting)
        {
            minigameCanvas.gameObject.SetActive(false);
            isBombInteracting = false;
        }
    }

    private void OnDestroy()
    {
        playerInput.actions["Grab"].performed -= Grab_performed;
        playerInput.actions["Push"].performed -= Push_performed;
        playerInput.actions["Interact"].performed -= Interact_performed;
        playerInput.actions["Interact"].canceled -= Interact_canceled;
    }
}