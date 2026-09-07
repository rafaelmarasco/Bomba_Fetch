using UnityEngine;
using UnityEngine.InputSystem;

public class PropInteract : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform handsPos;
    [SerializeField] private float pushForce;
    private InputSystem_Actions inputActions;
    private Vector3 lastMoveDir;
    public bool hasItem { get; private set; }
    public bool hasBomb { get; private set; }
    private bool isBombInteracting = false;
    public GameObject heldItem { get; private set; }

    private void Awake()
    {
        hasItem = false;
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        inputActions.Player.Grab.performed += Grab_performed;
        inputActions.Player.Push.performed += Push_performed;
        inputActions.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        if (hasBomb && !isBombInteracting)
        {
            isBombInteracting = true;
            EventManager.Instance.BombInteracted();
        }
    }
    private void Push_performed(InputAction.CallbackContext obj)
    {
        PushProp();
    }
    private void Grab_performed(InputAction.CallbackContext obj)
    {
        if (!hasItem && CheckForProps(out GameObject prop))
        {
            Debug.Log("Pegou");
            PickUpProp(prop);
            hasBomb = prop.name == "Bomb"; // Trocar para script quando a bomba tiver um script
        }
        else if (hasItem)
        {
            DropProp();
            if (hasBomb)
            {
                hasBomb = false;
                EventManager.Instance.BombInteracted();
            }
        }
    }
    private void Update()
    {
        lastMoveDir = player.GetLastMoveDirection();
    }
    private bool CheckForProps(out GameObject prop) // Check if theres an object in front of the player
    {
        float interactDistance = .8f;
        bool canGrab = Physics.Raycast(handsPos.position, lastMoveDir, out RaycastHit hit, interactDistance);
        bool isProp = canGrab && hit.collider.gameObject.CompareTag("Prop");

        if (canGrab && isProp)
            prop = hit.collider.gameObject;
        else
            prop = null;

        return isProp;
    }
    private void PushProp()
    {
        if (!isBombInteracting)
        {
            EventManager.Instance.PropPush();
            if (!hasItem && CheckForProps(out GameObject prop))
            {
                prop.TryGetComponent<Rigidbody>(out Rigidbody propRb);
                propRb.linearVelocity = lastMoveDir * pushForce;
            }
            else if (hasItem)
            {
                heldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb);
                DropProp();
                propRb.linearVelocity = lastMoveDir * pushForce;
            }
        }
    }
    private void DropProp()
    {
        float zOffset = 0.4f;
        Vector3 offset = new Vector3(0f, 0f, zOffset);

        if (heldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb))
            propRb.isKinematic = false;

        heldItem.transform.localPosition += offset;
        heldItem.transform.SetParent(null);
        heldItem = null;

        hasItem = false;

        if (isBombInteracting)
            isBombInteracting = false;

        EventManager.Instance.ItemDroped();
    }
    private void PickUpProp(GameObject prop)
    {
        float zOffSet = .6f;
        float yOffSet = .8f;

        if (prop.TryGetComponent<Rigidbody>(out Rigidbody propRb))
            propRb.isKinematic = true;

        prop.transform.SetParent(handsPos);
        prop.transform.localPosition = new Vector3(0f, yOffSet, zOffSet);

        heldItem = prop;
        hasItem = true;

        EventManager.Instance.ItemPickedUp();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(handsPos.position, lastMoveDir);
    }
}
