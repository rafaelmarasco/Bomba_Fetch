using UnityEngine;

public class FireExtinguisherSmoke : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem smokeParticule;

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

    public void StartSpewing()
    {
        smokeArea.enabled = true;
        smokeParticule.Play();
        //Debug.Log($"Estado da fumaçao {smokeArea.enabled}");
    }

    public void StopSpewing()
    {
        smokeParticule.Stop();
        smokeArea.enabled = false;
    }
}