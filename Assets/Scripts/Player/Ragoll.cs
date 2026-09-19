using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PropInteract propInteract;
    [SerializeField] private PlayerEventManager playerEventManager;

    [Header("Player Model")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject GFX;
    [SerializeField] private Rigidbody hipsRb;
    public bool IsRagDoll { get; private set; } = false;

    private Rigidbody[] bonesRb;

    private CapsuleCollider gfxCapsuCollider;

    private void Awake()
    {
        gfxCapsuCollider = GFX.GetComponent<CapsuleCollider>();
        bonesRb = GFX.GetComponentsInChildren<Rigidbody>();
        playerEventManager = GetComponent<PlayerEventManager>();

        UntangleBones();
    }
    public void EnableRagDoll(out Rigidbody heldItemRb)
    {
        GameObject heldItem = propInteract.heldItem;

        heldItemRb = null;

        if (heldItem != null && heldItem.TryGetComponent(out heldItemRb))
            propInteract.DropProp();

        foreach (Rigidbody bone in bonesRb)
        {
            bone.isKinematic = false;
            bone.useGravity = true;
        }

        gfxCapsuCollider.enabled = false;
        animator.enabled = false;
        playerEventManager.StopInputingMovement(true);
        IsRagDoll = true;
    }
    public void DisableRagDoll()
    {
        Transform playerRagDollTransform = hipsRb.gameObject.transform;

        transform.position =
            new Vector3(playerRagDollTransform.position.x, transform.position.y, playerRagDollTransform.position.z);

        foreach (Rigidbody bone in bonesRb)
        {
            bone.isKinematic = true;
            bone.useGravity = false;
        }

        gfxCapsuCollider.enabled = true;
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
