using UnityEngine;

public class Hazzard : MonoBehaviour
{
    private enum Type
    {
        fire,
        eletricity,
    }

    [SerializeField] private Type hazzardType;
    [SerializeField] private Vector3 flyDirection;
    [SerializeField] private Vector3 propFlyDirection;
    [SerializeField] private float flyForce;
    [SerializeField] private float stunTime;
    [SerializeField] private float cooldown;

    [Header("Fire Hazzard")]
    [SerializeField] private Material onFireMaterial;
    [SerializeField] private Transform onFireRunDirection;
    [SerializeField] private Vector3 jumpDirection;
    [SerializeField] private float jumpForce;
    [SerializeField] private float onFireTime;

    public bool isOn;

    private void OnTriggerEnter(Collider other)
    {
        switch (hazzardType)
        {
            case Type.fire:
                SetOnFire(other);
                break;

            case Type.eletricity:
                Debug.Log("Algo detectado");
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

        Player player = other.gameObject.GetComponentInParent<Player>();

        if (player == null || !player.canGetPushed)
            return;

        PlayerEventManager playerEventManager = other.GetComponentInParent<PlayerEventManager>();

        if (playerEventManager == null)
            return;

        player.StartKnockableColldownTimer(cooldown);
        playerEventManager.Eletrocute(flyDirection, propFlyDirection, flyForce, stunTime);
    }

    private void SetOnFire(Collider other)
    {
        if (!isOn || !other.gameObject.GetComponent<PlayerAnimator>())
            return;

        Player player = other.GetComponentInParent<Player>();

        if (player == null || player.isOnFire)
            return;

        PlayerEventManager playerEventManager = other.GetComponentInParent<PlayerEventManager>();

        if (playerEventManager == null)
            return;

        playerEventManager.SetOnFire(jumpDirection, onFireRunDirection.position, jumpForce, onFireTime);
    }
    private void FireTest(Collider other)
    {
        SkinnedMeshRenderer skinnedMeshRenderer = other.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        Material[] mats = skinnedMeshRenderer.materials;
        mats[0] = onFireMaterial;

        Material[] originalMat = skinnedMeshRenderer.materials;

        skinnedMeshRenderer.materials = mats;
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
