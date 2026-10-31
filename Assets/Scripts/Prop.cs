using UnityEngine;

public enum Size
{
    small,
    medium,
    large,
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class Prop : MonoBehaviour
{
    private PropManager manager;
    [SerializeField] private Size propSize;
    public Size PropSize => propSize;
    public PropInteract PlayerInteracting { get; private set; }
    public float ThrowForce
    {
        get
        {
            return PropSize switch
            {
                Size.small => 10f,
                Size.medium => 6f,
                Size.large => 4f,
                _ => 0f,
            };
        }
    }

    private FixedJoint joint;

    private Rigidbody rb;
    private Collider propCollider;
    private Collider throwerCollider;

    private bool isBeingPushed = false;
    public bool IsHiddingSpot { get; private set; } = false;

    private PhysicsMaterial CarryMaterialFix => manager.PhysicsMaterial;
    public IHideable HiddenProp { get; private set; }

    private const int PROP_IN_HAND_LAYER = 7;
    private const int DEFAULT_LAYER = 0;
    private const int FLOOR_LAYER = 10;

    private void Start()
    {
        //manager = transform.Find("PropManager").GetComponent<PropManager>();
        manager = FindFirstObjectByType<PropManager>();
        propCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        rb.maxDepenetrationVelocity = 2f;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        SetPropMass();
        SetPhysicsMaterial();
    }

    public void EnableReposition(PropInteract playerInteracting)
    {
        this.PlayerInteracting = playerInteracting;
        throwerCollider = PlayerInteracting.GetComponentInChildren<Collider>();

        switch (PropSize)
        {
            case Size.small:
                RepositionSmallProp();
                break;
            case Size.medium:
                DestroyJoints();
                RepositionMediumProp();
                break;
            case Size.large:
                DestroyJoints();
                RepositionLargeProp();
                break;
        }

    }
    private void RepositionSmallProp()
    {
        Transform holdPoint;

        holdPoint = PlayerInteracting.holdPointSmall;

        GetComponent<Rigidbody>().isKinematic = true;

        gameObject.layer = PROP_IN_HAND_LAYER;

        Physics.IgnoreCollision(propCollider, throwerCollider);

        transform.SetParent(holdPoint);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

    }
    private void RepositionMediumProp()
    {
        Transform holdPoint;

        holdPoint = PlayerInteracting.holdPointMedium;

        gameObject.layer = PROP_IN_HAND_LAYER;

        transform.SetPositionAndRotation(holdPoint.position, holdPoint.rotation);

        Rigidbody holdPointRb = holdPoint.GetComponent<Rigidbody>();

        rb.mass = 2f;

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = holdPointRb;
        joint.enableCollision = true;
    }
    private void RepositionLargeProp()
    {
        Rigidbody playerRb = PlayerInteracting.GetComponent<Rigidbody>();


        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = playerRb;
    }
    public void DisableReposition()
    {
        SetPropMass();

        gameObject.layer = DEFAULT_LAYER;

        GetComponent<Rigidbody>().isKinematic = false;

        if(!isBeingPushed)
            Physics.IgnoreCollision(propCollider, throwerCollider, false);

        DestroyJoints();
        PlayerInteracting = null;
    }
    private void DestroyJoints()
    {
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }
    }
    public void SetPushingState(bool state)
    {
        isBeingPushed = state;
        if (state == false && throwerCollider != null)
        {
            Physics.IgnoreCollision(propCollider, throwerCollider, false);
            throwerCollider = null;
        }
    }


    private void HandlePropToPlayerContact(Collision collision)
    {
        if (collision.gameObject.layer == FLOOR_LAYER)
        {
            SetPushingState(false);
            return;
        }

        if (throwerCollider != null)
            Physics.IgnoreCollision(propCollider, throwerCollider);
        else
            Debug.Log("O colisor do jogador nao foi pego");

        if (!collision.gameObject.GetComponent<Player>()) return;

        PlayerEventManager targetEventManager = collision.gameObject.GetComponentInParent<PlayerEventManager>();
        if (targetEventManager == null) return;

        float stunTime = 2f;

        targetEventManager.KnockedDown(stunTime);
        SetPushingState(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!isBeingPushed) return;
        HandlePropToPlayerContact(collision);
    }

    private void SetPropMass()
    {
        switch (this.propSize)
        {
            case Size.small:
                rb.mass = 20;
                break;
            case Size.medium:
                rb.mass = 70;
                break;
            case Size.large:
                rb.mass = 100;
                break;
        }
    }

    private void SetPhysicsMaterial()
    {
        if (this.propSize != Size.medium) return;
        propCollider.material = CarryMaterialFix;
    }
    public void SetAsHiddingSpot(bool state, IHideable hideable)
    {
        IsHiddingSpot = state;
        HiddenProp = hideable;
    }

}
