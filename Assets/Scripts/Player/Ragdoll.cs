using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private PropPickupHandler propPickupHandler;
    private PlayerEventManager playerEventManager;

    [Header("Player Model")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject GFX;
    [SerializeField] private Rigidbody hipsRb;
    [SerializeField] private Rigidbody playerRb;
    public bool IsRagDoll { get; private set; } = false;

    private Rigidbody[] bonesRb;

    private void Awake()
    {
        playerEventManager = GetComponent<PlayerEventManager>();
        propPickupHandler = GetComponent<PropPickupHandler>();

        bonesRb = GFX.GetComponentsInChildren<Rigidbody>();

        UntangleBones();
    }
    public void EnableRagDoll(out Rigidbody heldItemRb)
    {
        const int RAGDOLL_LAYER = 8;

        GameObject heldItem = propPickupHandler.HeldItem;

        heldItemRb = null;

        if (heldItem != null && heldItem.TryGetComponent(out heldItemRb))
            propPickupHandler.DropProp();

        foreach (Rigidbody bone in bonesRb)
        {
            bone.isKinematic = false;
            bone.useGravity = true;
        }

        GFX.layer = RAGDOLL_LAYER;
        playerRb.useGravity = false;

        animator.enabled = false;

        playerEventManager.StopInputingMovement(true);

        IsRagDoll = true;
    }
    public void DisableRagDoll()
    {
        const int PLAYER_LAYER = 3;
        Transform playerRagDollTransform = hipsRb.gameObject.transform;

        transform.position =
            new Vector3(playerRagDollTransform.position.x, transform.position.y, playerRagDollTransform.position.z);

        foreach (Rigidbody bone in bonesRb)
        {
            bone.isKinematic = true;
            bone.useGravity = false;
        }

        GFX.layer = PLAYER_LAYER;
        playerRb.useGravity = true;
        animator.enabled = true;
        playerEventManager.StopInputingMovement(false);
        IsRagDoll = false;
    }
    private void UntangleBones() // Makes the player collider and bone colliders ignore each other
    {
        Collider playerCollider = GetComponentInChildren<Collider>();

        foreach (Rigidbody bone in bonesRb)
        {
            if (bone.TryGetComponent<Collider>(out Collider boneCollider))
                Physics.IgnoreCollision(playerCollider, boneCollider);
        }

        DisableRagDoll();
    }
}
