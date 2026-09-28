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
    [SerializeField] private PropInteract playerInteracting;
    public Size PropSize => propSize;
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

    private bool isBeingHeld;

    private const int PROP_IN_HAND_LAYER = 7;
    private const int DEFAULT_LAYER = 0;

    public void EnableReposition(PropInteract playerInteracting)
    {
        this.playerInteracting = playerInteracting;

        switch (PropSize)
        {
            case Size.small:
                RepositionSmallProp();
                break;
            case Size.medium:
                DestroyJoints();
                RepositionMediumProp();
                break;
        }
        isBeingHeld = true;
    }
    private void RepositionSmallProp()
    {
        Transform holdPoint;

        holdPoint = playerInteracting.holdPointSmall;

        GetComponent<Rigidbody>().isKinematic = true;

        gameObject.layer = PROP_IN_HAND_LAYER;

        transform.SetParent(holdPoint);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void RepositionMediumProp()
    {
        Transform holdPoint;

        holdPoint = playerInteracting.holdPointMedium;

        transform.SetPositionAndRotation(holdPoint.position, holdPoint.rotation);

        Rigidbody holdPointRb = holdPoint.GetComponent<Rigidbody>();

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = holdPointRb;
    }
    public void DisableReposition()
    {
        gameObject.layer = DEFAULT_LAYER;

        GetComponent<Rigidbody>().isKinematic = false;

        DestroyJoints();
        playerInteracting = null;
        isBeingHeld = false;
    }

    private void DestroyJoints()
    {
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }
    }
}
