using UnityEngine;

public class FireExtinguisherSmoke : MonoBehaviour
{
    private CapsuleCollider smokeArea;
    public bool IsSpewing => smokeArea.enabled;

    private void Awake()
    {
        smokeArea = GetComponent<CapsuleCollider>();
        smokeArea.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out Hazard hazard);

        if (hazard == null || hazard.Type != HazardType.fire) return;

        Destroy(hazard.gameObject);
    }

    public void StartSmoke()
    {
        smokeArea.enabled = true;
        Debug.Log($"Estado da fumaçao {smokeArea.enabled}");
    }

    public void StopSmoke()
    {
        smokeArea.enabled = false;
    }
}