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
    [SerializeField] private Size propSize;
    public Size PropSize => propSize;
    public PropInteract PlayerInteracting { get; private set; }
    public float ThrowForce
    {
        get
        {
            return PropSize switch
            {
                Size.small => 8f,
                Size.medium => 10f,
                Size.large => 15f,
                _ => ThrowForce,
            };
        }
    }

    private FixedJoint joint;

    private Rigidbody rb;

    private bool isBeingPushed = false;

    [SerializeField] private float defaultLinearDampingValue;

    private const int PROP_IN_HAND_LAYER = 7;
    private const int DEFAULT_LAYER = 0;
    private const int FLOOR_LAYER = 10;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rb.position.y > 0.0f)
        {
            rb.linearDamping = 0;
        }
        else
        {
            rb.linearDamping = defaultLinearDampingValue;
        }
    }

    public void EnableReposition(PropInteract playerInteracting)
    {
        this.PlayerInteracting = playerInteracting;

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

        //Physics.IgnoreCollision(GetComponent<Collider>(), PlayerInteracting.GetComponentInChildren<Collider>());
    }
    private void RepositionSmallProp()
    {
        Transform holdPoint;

        holdPoint = PlayerInteracting.holdPointSmall;

        GetComponent<Rigidbody>().isKinematic = true;

        gameObject.layer = PROP_IN_HAND_LAYER;
        
        Physics.IgnoreCollision(GetComponent<Collider>(), PlayerInteracting.GetComponentInChildren<Collider>());

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
        gameObject.layer = DEFAULT_LAYER;

        GetComponent<Rigidbody>().isKinematic = false;

        DestroyJoints();
        Physics.IgnoreCollision(GetComponent<Collider>(), PlayerInteracting.GetComponentInChildren<Collider>(), false);
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
    public void SetPushingState(bool state) => isBeingPushed = state;
    private void HandlePropToPlayerContact(Collision collision)
    {
        if (collision.gameObject.layer == FLOOR_LAYER)
            SetPushingState(false);

        if (!collision.gameObject.GetComponent<Player>()) return;

        float stunTime = 2f;
        PlayerEventManager targetEventManager = collision.gameObject.GetComponentInParent<PlayerEventManager>();
        //PlayerHarmHandler target = collision.gameObject.GetComponentInParent<PlayerHarmHandler>();

        targetEventManager.KnockedDown(stunTime);
        SetPushingState(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isBeingPushed)
            HandlePropToPlayerContact(collision);
        else return;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == FLOOR_LAYER && rb.position.y != 0)
        {
            transform.position = new(rb.position.x, 0, rb.position.z);
        }
        else return; 
    }
}
