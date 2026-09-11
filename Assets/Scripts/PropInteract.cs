using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PropInteract : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform CheckPos;
    [SerializeField] private Transform headPos;
    [SerializeField] private float pushForce;
    private InputSystem_Actions inputActions;
    private Vector3 lastMoveDir;
    public bool hasItem => heldItem != null;
    public bool hasBomb { get; private set; } = false;
    public bool isBombInteracting { get; private set; } = false;
    public GameObject heldItem { get; private set; }

    [SerializeField] private Transform holdPointSmall;
    [SerializeField] private Transform holdPointMedium;
    [SerializeField] private Transform holdPointLarge;
    [SerializeField] private Transform holdPointInteract;

    public Vector3 halfExtends = new Vector3(.5f, 0.1f, .4f);

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
        inputActions.Player.Grab.performed += Grab_performed;
        inputActions.Player.Push.performed += Push_performed;
        inputActions.Player.Interact.performed += Interact_performed;
    }
    private void Update()
    {
        lastMoveDir = player.GetLastMoveDirection();
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        if (hasBomb && !isBombInteracting)
        {
            heldItem.transform.SetParent(holdPointInteract);

            isBombInteracting = true;
            EventManager.Instance.BombInteracted(headPos, heldItem);
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
        }
        else if (hasItem)
        {
            DropProp();
            if (hasBomb)
            {
                hasBomb = false;
                EventManager.Instance.BombDroped();
            }
        }
    }

    private bool CheckForProps(out GameObject prop) // Check if theres an object in front of the player
    {
        float interactDistance = .5f;
        //Vector3 halfExtends = new Vector3(.5f, 0.1f, .4f);
        bool canGrab = Physics.BoxCast(CheckPos.position - lastMoveDir * .2f, halfExtends, lastMoveDir, out RaycastHit hit, CheckPos.rotation, interactDistance);
        bool isProp = canGrab && hit.collider.gameObject.TryGetComponent<Prop>(out Prop propComponent);

        if (isProp)
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
    private void PickUpProp(GameObject prop)
    {
        //float zOffSet = .6f;
        //float yOffSet = .8f;

        heldItem = prop;
        prop.TryGetComponent<Prop>(out Prop propInfo);

        hasBomb = prop.name == "Bomb"; // Trocar para script quando a bomba tiver um script

        if (prop.TryGetComponent<Rigidbody>(out Rigidbody propRb))
            propRb.isKinematic = true;

        PickUpReposition(propInfo, prop.transform);

        EventManager.Instance.ItemPickedUp(propInfo);
    }
    private void DropProp()
    {
        float zOffset = .2f;
        float yOffset = .3f;
        Vector3 offset = new Vector3(0f, yOffset, zOffset);
        GameObject droppedProp = heldItem;

        if (heldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb))
            propRb.isKinematic = false;

        heldItem.transform.localPosition += offset;
        heldItem.transform.SetParent(null);
        heldItem = null;

        if (isBombInteracting)
        {
            EventManager.Instance.BombDroped();
            isBombInteracting = false;
        }

        EventManager.Instance.ItemDroped(droppedProp.GetComponent<Prop>());
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
                EventManager.Instance.ItemPickedUp(propInfo);
                break;
            case Size.large:
                break;
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawRay(CheckPos.position, lastMoveDir);
        Gizmos.DrawCube(transform.position + lastMoveDir * .8f , halfExtends * 2);
    }
}
