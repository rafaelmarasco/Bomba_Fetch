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
    public float throwForce
    {
        get
        {
            return PropSize switch
            {
                Size.small => 8f,
                Size.medium => 10f,
                Size.large => 18f,
                _ => throwForce,
            };
        }
    }
}
