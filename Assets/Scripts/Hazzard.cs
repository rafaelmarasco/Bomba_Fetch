using UnityEngine;

public class Hazzard : MonoBehaviour
{
    [SerializeField] private Vector3 flyDirection;
    [SerializeField] private float flyForce;
    [SerializeField] private float stunTime;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "GFX")
        {
            PlayerEventManager playerEventManager = other.GetComponentInParent<PlayerEventManager>();
            playerEventManager.Eletrocute(flyDirection, flyForce , stunTime);

        }
    }
}
