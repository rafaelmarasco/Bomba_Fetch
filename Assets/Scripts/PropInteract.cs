using UnityEngine;

public class PropInteract : MonoBehaviour 
{
    [SerializeField] private Player player;
    [SerializeField] private Transform propTestPos;
    private Vector3 lastMoveDir = Vector3.forward;
    private bool hasItem = false;
    private GameObject heldItem;

    private void Update()
    {
        lastMoveDir = player.moveDir != Vector3.zero ? player.moveDir : lastMoveDir;
        // If they dont have any item they pick up an item
        if (!hasItem && CheckForProps(out GameObject obj) && obj.CompareTag("Prop"))
        {
            if (Input.GetKeyDown(KeyCode.E))
                PickUpProp(obj);
        }

        // Drop the item if they have any item in hand
        else if (hasItem)
        {
            if (Input.GetKeyDown(KeyCode.E))
                DropProp();
        }
    }

    private bool CheckForProps(out GameObject obj) // Check if theres an object in front of the player
    {
        float maxDistance = 1f;
        bool canGrab = Physics.Raycast(transform.position, lastMoveDir, out RaycastHit hit, maxDistance);
        obj = hit.collider.gameObject;
        return canGrab;
    }

    private void DropProp()
    {
        if (heldItem.TryGetComponent<Rigidbody>(out Rigidbody propRb))
            propRb.isKinematic = false;

        heldItem.transform.SetParent(null);
        hasItem = false;
        heldItem = null;
    }

    private void PickUpProp(GameObject prop)
    {
        float offSet = .5f;

        if (prop.TryGetComponent<Rigidbody>(out Rigidbody propRb))
            propRb.isKinematic = true;

        prop.transform.SetParent(propTestPos);
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
