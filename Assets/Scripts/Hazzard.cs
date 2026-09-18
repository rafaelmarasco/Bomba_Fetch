using System.Collections;
using UnityEngine;

public class Hazzard : MonoBehaviour
{
    [SerializeField] private Vector3 flyDirection;
    [SerializeField] private float flyForce;
    [SerializeField] private float stunTime;
    [SerializeField] private float cooldown;

    public bool isOn;
    private void OnTriggerEnter(Collider other)
    {
        TryToEletrocute(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryToEletrocute(other);
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

        player.StartColldownTimer(cooldown);
        playerEventManager.Eletrocute(flyDirection, flyForce, stunTime);
    }
}
