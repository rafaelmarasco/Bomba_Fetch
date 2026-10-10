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

    private bool isFlying = false;

    private const int PROP_IN_HAND_LAYER = 7;
    private const int DEFAULT_LAYER = 0;
    private const int FLOOR_LAYER = 10;

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

        Physics.IgnoreCollision(GetComponent<Collider>(), PlayerInteracting.GetComponentInChildren<Collider>());
    }
    private void RepositionSmallProp()
    {
        Transform holdPoint;

        holdPoint = PlayerInteracting.holdPointSmall;

        GetComponent<Rigidbody>().isKinematic = true;

        gameObject.layer = PROP_IN_HAND_LAYER;

        transform.SetParent(holdPoint);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    private void RepositionMediumProp()
    {
        Transform holdPoint;

        holdPoint = PlayerInteracting.holdPointMedium;

        transform.SetPositionAndRotation(holdPoint.position, holdPoint.rotation);

        Rigidbody holdPointRb = holdPoint.GetComponent<Rigidbody>();

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = holdPointRb;
    }
    private void RepositionLargeProp()
    {/*
        Vector3 playerToPropDirection = transform.position - PlayerInteracting.transform.position;
        playerToPropDirection.Normalize();

        float offSet = 1.5f;
        PlayerInteracting.transform.position = playerToPropDirection * offSet;
        */
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
    public void SetFlyingState(bool state) => isFlying = state;
    private void HandlePropToPlayerContact(Collision collision)
    {
        if (collision.gameObject.layer == FLOOR_LAYER)
            SetFlyingState(false);

        if (!collision.gameObject.GetComponent<Player>()) return;

        float stunTime = 2f;
        PlayerEventManager targetEventManager = collision.gameObject.GetComponentInParent<PlayerEventManager>();
        //PlayerHarmHandler target = collision.gameObject.GetComponentInParent<PlayerHarmHandler>();

        targetEventManager.KnockedDown(stunTime);
        SetFlyingState(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isFlying)
            HandlePropToPlayerContact(collision);
        else return;
    }
}
