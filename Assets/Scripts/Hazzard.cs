using UnityEngine;

public class Hazzard : MonoBehaviour
{
    [SerializeField] private Vector3 flyDirection = Vector3.left;
    [SerializeField] private float flyForce = 10f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "GFX")
        {
            PlayerEventManager playerEventManager = other.GetComponentInParent<PlayerEventManager>();
            playerEventManager.Eletrocute(flyDirection,flyForce);

        }
    }
}
