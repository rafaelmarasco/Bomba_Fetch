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
    public GameObject heldItem {  get; private set; }

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Grab.Enable();
        inputActions.Player.Push.Enable();
        inputActions.Player.Grab.performed += Grab_performed;
        inputActions.Player.Push.performed += Push_performed;
        hasItem = false;
    }

    private void Push_performed(InputAction.CallbackContext obj)
    {
        PushProp();
    }

    private void Grab_performed(InputAction.CallbackContext obj)
    {
        if (!hasItem && CheckForProps(out GameObject prop)) // If they dont have any item they pick up an item
            PickUpProp(prop);
        else if (hasItem) // Drop the item if they have any item in hand
            DropProp();

    }

    private void Update()
    {
        lastMoveDir = player.GetLastMoveDirection();
    }

    private void PushProp()
    {
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

    private void DropProp()
    {
        float zOffset = 0.4f;
        Vector3 offset = new Vector3(0f, 0f, zOffset);

        if (heldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb))
            propRb.isKinematic = false;

        heldItem.transform.localPosition += offset;
        heldItem.transform.SetParent(null);
        hasItem = false;
        heldItem = null;
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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(handsPos.position, lastMoveDir);
    }
}
