using UnityEngine;

public class GrabProps : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform propTestPos;
    private Vector3 lastMoveDir = Vector3.forward;
    private void Update()
    {
        float maxDistance = 1f;
        lastMoveDir = player.moveDir != Vector3.zero ? player.moveDir : lastMoveDir;

        //Check if theres an object in front of the player
        bool canGrab = Physics.Raycast(transform.position, lastMoveDir, out RaycastHit hit, maxDistance);

        if (canGrab && hit.collider.gameObject.CompareTag("Prop"))
        {
            GameObject prop = hit.collider.gameObject;
            if (Input.GetKeyDown(KeyCode.E))
            {
                prop.transform.SetParent(propTestPos);
                prop.transform.localPosition = Vector3.zero;
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, lastMoveDir);
    }
}
