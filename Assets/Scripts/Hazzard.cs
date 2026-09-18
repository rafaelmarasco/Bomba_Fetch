using UnityEngine;

public class Hazzard : MonoBehaviour
{
    [SerializeField] private Vector3 flyDirection;
    [SerializeField] private float flyForce;
    [SerializeField] private float stunTime;
    public bool isOn;
    private void OnTriggerEnter(Collider other)
    {
        if (isOn && other.gameObject.name == "GFX")
        {
            PlayerEventManager playerEventManager = other.GetComponentInParent<PlayerEventManager>();
            playerEventManager.Eletrocute(flyDirection, flyForce , stunTime);
        }
    }
}
