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

    private const int WALL_LAYER = 6;

    public void EnableReposition(PropInteract playerInteracting)
    {
        this.playerInteracting = playerInteracting;

        Transform holdPoint;

        DestroyJoints();

        holdPoint = PropSize switch
        {
            Size.small => playerInteracting.holdPointSmall,
            Size.medium => playerInteracting.holdPointMedium,
            Size.large => playerInteracting.holdPointMedium,
            _ => playerInteracting.holdPointMedium
        };

        transform.SetPositionAndRotation(holdPoint.position, holdPoint.rotation);

        Rigidbody holdPointRb = holdPoint.GetComponent<Rigidbody>();

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = holdPointRb;

        isBeingHeld = true;
    }

    public void DisableReposition()
    {
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

    private void OnCollisionStay(Collision collision)
    {
        if (isBeingHeld && collision.gameObject.layer == WALL_LAYER)
            Debug.Log($"Tamo na parede {gameObject.name}");
    }
}
