using UnityEngine;

public class ReciveHarm : MonoBehaviour
{
    private Ragdoll ragdoll;
    private Rigidbody hipsRb;
    private PlayerEventManager playerEventManager;

    private void Awake()
    {
        ragdoll = GetComponent<Ragdoll>();
        hipsRb = GetComponentInChildren<Rigidbody>();
        playerEventManager = GetComponent<PlayerEventManager>();
    }

    private void OnEnable()
    {
        playerEventManager.OnEletrocuted += GetYonked;
    }
    private void GetYonked(Vector3 flyDirection, Vector3 propFlyDirection, float flyForce, float stunTime)
    {
        ragdoll.EnableRagDoll(out Rigidbody heldItem);

        if (heldItem != null)
        {
            float propFlyForce = heldItem.gameObject.GetComponent<Prop>().ThrowForce;
            heldItem.AddForce(propFlyDirection * propFlyForce, ForceMode.Impulse);
        }

        hipsRb.AddForce(flyDirection * flyForce, ForceMode.Impulse);
        playerEventManager.KnockedDown(stunTime);
    }
}
