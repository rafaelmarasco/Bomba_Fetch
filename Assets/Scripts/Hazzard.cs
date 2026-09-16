using UnityEngine;

public class Hazzard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "GFX")
        {
            PlayerEventManager playerEventManager = other.GetComponentInParent<PlayerEventManager>();
            playerEventManager.Eletrocute();

        }
    }
}
