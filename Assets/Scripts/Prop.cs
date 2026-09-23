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

    private Transform holdPoint;

    private FixedJoint joint;

    public void EnableReposition(Transform holdPointSmall, Transform holdPointMedium)
    {

        /*
        switch (propSize)
        {
            case Size.small:
                transform.SetParent(holdPointSmall);
                transform.localPosition = Vector3.zero;
                transform.rotation = holdPointSmall.rotation;
                break;
            case Size.medium:
                transform.SetParent(holdPointMedium);
                transform.localPosition = Vector3.zero;
                transform.rotation = holdPointMedium.rotation;
                break;
            case Size.large: // Mudar logica no futuro
                transform.SetParent(holdPointMedium);
                transform.localPosition = Vector3.zero;
                transform.rotation = holdPointMedium.rotation;
                break;
        }
        */

        DestroyJoints();

        holdPoint = PropSize switch
        {
            Size.small => holdPointSmall,
            Size.medium => holdPointMedium,
            Size.large => holdPointMedium,
            _ => holdPointMedium
        };

        transform.SetPositionAndRotation(holdPoint.position, holdPoint.rotation);

        Rigidbody holdPointRb = holdPoint.GetComponent<Rigidbody>();

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = holdPointRb;
        
        //isBeingHeld = true;
    }

    public void DisableReposition()
    {
        DestroyJoints();

        holdPoint = null;
        //isBeingHeld = false;
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
