using UnityEngine;
using UnityEngine.InputSystem;

public class PropInteract : MonoBehaviour
{
    private PlayerEventManager playerEventManager;
    private PlayerInput playerInput;
    private Player player;

    [SerializeField] private Transform checkPos;
    [SerializeField] private Transform headPos;
    [SerializeField] private float pushForce;

    private Vector3 lastMoveDir;
    public bool HasItem => HeldItem != null;
    public bool HasBomb { get; private set; } = false;
    public bool IsBombInteracting { get; private set; } = false;
    public GameObject HeldItem { get; private set; }

    [Header("Prop Holding Points")]
    [SerializeField] private Transform holdPointSmall;
    [SerializeField] private Transform holdPointMedium;
    [SerializeField] private Transform holdPointLarge;
    [SerializeField] private Transform holdPointInteract;

    [Header("BoxCastConfigs")]
    [SerializeField] private Vector3 halfExtends = new Vector3(.5f, 0.1f, .4f);
    [SerializeField] private float interactDistance = .8f;

    [Header("Minigame")]
    [SerializeField] private Canvas minigameCanvas;


    private void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        playerEventManager = GetComponent<PlayerEventManager>();

        playerInput.actions["Grab"].performed += Grab_performed;
        playerInput.actions["Push"].performed += Push_performed;
        playerInput.actions["Interact"].performed += Interact_performed;
    }
    private void Update()
    {
        lastMoveDir = player.LastMoveDir;
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        if (HasBomb && !IsBombInteracting)
        {
            //heldItem.transform.SetParent(holdPointInteract);
            // In this function mean that player has bomb and he is holding it
            minigameCanvas.gameObject.SetActive(true);
            IsBombInteracting = true;
            playerEventManager.BombInteracted(headPos, HeldItem);
        }
    }
    private void Push_performed(InputAction.CallbackContext obj)
    {
        Push();
    }
    private void Grab_performed(InputAction.CallbackContext obj)
    {
        if (!HasItem && CheckForProps(out GameObject prop))
        {
            Debug.Log("Pegou");
            PickUpProp(prop);
        }
        else if (HasItem)
        {
            DropProp();
            if (HasBomb)
            {
                HasBomb = false;
                playerEventManager.BombDroped();
            }
        }
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
    private bool CheckForProps(out GameObject prop) // Check if theres an object in front of the player
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
    public void Push()
    {
        if (!IsBombInteracting) //REMOVER????
        {
            playerEventManager.PropPush();

            GameObject prop = null;
            GameObject player = null;

            if (!HasItem && (CheckForProps(out prop) || CheckForPlayer(out player)))
            {
                if (prop != null && prop.TryGetComponent<Rigidbody>(out Rigidbody propRb))
                    propRb.linearVelocity = lastMoveDir * pushForce;

                else if (player != null)
                {
                    float againstPlayerMutiplier = 20;
                    Rigidbody hipsRb = player.GetComponent<Player>().HipsRb;
                    player.GetComponent<Ragdoll>().EnableRagDoll(out _);
                    hipsRb.AddForce(againstPlayerMutiplier * pushForce * lastMoveDir, ForceMode.Impulse);
                }
            }
            else if (HasItem)
            {
                HeldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb);
                DropProp();
                propRb.linearVelocity = lastMoveDir * pushForce;
            }
        }
    }
    private void PickUpProp(GameObject prop)
    {
        const int PROP_IN_HAND_LAYER = 7;

        HeldItem = prop;
        prop.TryGetComponent<Prop>(out Prop propInfo);

        HasBomb = prop.name == "Bomb"; // Trocar para script quando a bomba tiver um script

        if (prop.TryGetComponent<Rigidbody>(out Rigidbody propRb))
            propRb.isKinematic = true;

        prop.layer = PROP_IN_HAND_LAYER;

        PickUpReposition(propInfo, prop.transform);

        playerEventManager.ItemPickedUp(propInfo);
    }
    public void DropProp()
    {
        const int DEFAULT_LAYER = 0;

        Prop propInfo = HeldItem.GetComponent<Prop>();
        float zOffset = .2f;
        float yOffset = .3f;
        Vector3 offset = new Vector3(0f, yOffset, zOffset);

        if (HeldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb))
            propRb.isKinematic = false;

        HeldItem.layer = DEFAULT_LAYER;

        HeldItem.transform.localPosition += offset;
        HeldItem.transform.SetParent(null);
        HeldItem = null;

        if (IsBombInteracting)
        {
            minigameCanvas.gameObject.SetActive(false);
            playerEventManager.BombDroped();
            IsBombInteracting = false;
        }

        playerEventManager.ItemDroped(propInfo);
    }
    public void PickUpReposition(Prop propInfo, Transform propPos)
    {
        Size propSize = propInfo.PropSize;

        switch (propSize)
        {
            case Size.small:
                propPos.SetParent(holdPointSmall);
                propPos.localPosition = Vector3.zero;
                propPos.rotation = holdPointSmall.rotation;
                break;
            case Size.medium:
                propPos.SetParent(holdPointMedium);
                propPos.localPosition = Vector3.zero;
                propPos.rotation = holdPointMedium.rotation;
                break;
            case Size.large: // Mudar logica no futuro
                propPos.SetParent(holdPointMedium);
                propPos.localPosition = Vector3.zero;
                propPos.rotation = holdPointMedium.rotation;
                break;
        }

    }
    private void BoxCastDebug(Vector3 origin, Vector3 halfExtends, Quaternion orientation)
    {
        DebugBoxCast.SimpleDrawBoxCast(origin, halfExtends, orientation, lastMoveDir, interactDistance, Color.aliceBlue);
    }
}
