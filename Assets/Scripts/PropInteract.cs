using UnityEngine;
using UnityEngine.InputSystem;

public class PropInteract : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform handsPos;
    private InputSystem_Actions inputActions;
    private Vector3 lastMoveDir = Vector3.forward;
    public bool hasItem { get; private set; }
    private GameObject heldItem;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Interact.Enable();
        inputActions.Player.Interact.performed += Interact_performed;
        hasItem = false;
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        if (!hasItem && CheckForProps(out GameObject prop)) // If they dont have any item they pick up an item
            PickUpProp(prop);
        else if (hasItem) // Drop the item if they have any item in hand
            DropProp();

    }

    private void Update()
    {
        lastMoveDir = player.moveDir != Vector3.zero ? player.moveDir : lastMoveDir;
    }

    private bool CheckForProps(out GameObject prop) // Check if theres an object in front of the player
    {
        float grabDistance = 1f;
        bool canGrab = Physics.Raycast(handsPos.position, lastMoveDir, out RaycastHit hit, grabDistance);
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
        float offSet = .5f;

        if (prop.TryGetComponent<Rigidbody>(out Rigidbody propRb))
            propRb.isKinematic = true;

        prop.transform.SetParent(handsPos);
        prop.transform.localPosition = new Vector3(0f, 0f, 0f + offSet);

        heldItem = prop;
        hasItem = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, lastMoveDir);
    }
}
