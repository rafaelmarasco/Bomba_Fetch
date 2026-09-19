using UnityEngine;

public class Hazzard : MonoBehaviour
{
    private enum Type
    {
        fire,
        eletricity,
    }
    [SerializeField] private Type hazzardType;

    [Header("Eletricity Hazzard")]
    [SerializeField] private Vector3 flyDirection;
    [SerializeField] private Vector3 propFlyDirection;
    [SerializeField] private float flyForce;
    [SerializeField] private float cooldown;

    [Header("Fire Hazzard")]
    [SerializeField] private Transform onFireRunDirection;
    private Vector3 jumpDirection => onFireRunDirection.position.normalized + Vector3.up;
    [SerializeField] private float jumpForce;
    [SerializeField] private float onFireTime;

    [Header("Hazzard Field")]
    [SerializeField] private float stunTime;
    public bool isOn;

    private void OnTriggerEnter(Collider other)
    {
        switch (hazzardType)
        {
            case Type.fire:
                SetOnFire(other);
                break;

            case Type.eletricity:
                TryToEletrocute(other);
                break;

            default:
                Debug.LogWarning("Hazzard Type not defined");
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        switch (hazzardType)
        {
            case Type.fire:
                return;

            case Type.eletricity:
                TryToEletrocute(other);
                break;

            default:
                Debug.LogWarning("Hazzard Type not defined");
                break;
        }
    }

    private void TryToEletrocute(Collider other)
    {
        if (!isOn || !other.gameObject.GetComponent<PlayerAnimator>())
            return;

        PlayerHarmHandler playerHarmHandler = other.gameObject.GetComponentInParent<PlayerHarmHandler>();

        if (playerHarmHandler == null || !playerHarmHandler.CanGetPushed)
            return;

        PlayerEventManager playerEventManager = other.GetComponentInParent<PlayerEventManager>();

        if (playerEventManager == null)
            return;

        playerHarmHandler.StartKnockableColldownTimer(cooldown);
        playerEventManager.Eletrocute(flyDirection, propFlyDirection, flyForce, stunTime);
    }

    private void SetOnFire(Collider other)
    {
        if (!isOn || !other.gameObject.GetComponent<PlayerAnimator>())
            return;

        PlayerHarmHandler playerHarmHandler = other.gameObject.GetComponentInParent<PlayerHarmHandler>();

        if (playerHarmHandler == null || playerHarmHandler.IsOnFire)
            return;

        PlayerEventManager playerEventManager = other.GetComponentInParent<PlayerEventManager>();

        if (playerEventManager == null)
            return;

        playerEventManager.SetOnFire(jumpDirection, onFireRunDirection.position, jumpForce * 10, onFireTime, transform.position, stunTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (hazzardType == Type.fire)
        {
            Gizmos.color = Color.orangeRed;

            Gizmos.DrawLine(transform.position, onFireRunDirection.position);
        }
    }
}
