using Unity.VisualScripting;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{

    [Header("Event Manager")]
    [SerializeField] private EventManager eventManager;// Precisa melhorar isso. Como Bomb é um prefab, quando ele aparecer ele não vai estar conectado com o event manager

    [Header("Explosion Settings")]
    [SerializeField] private float radiusExplosion;
    [SerializeField] private float explosionForce;
    [SerializeField] private float upwardsModifier;//How tall the explosion will push the object

    // Esse start é só para teste, o original provavelmente ele vai ser instanciado
    private void Start()
    {
        eventManager.OnBombExploded += ExplodeBomb;
    }

    
    private void OnEnable()
    {
        eventManager.OnBombExploded += ExplodeBomb;
    }

    private void OnDisable()
    {
        eventManager.OnBombExploded -= ExplodeBomb;
    }

    public void ExplodeBomb()
    {
        Collider[] nearColliders = Physics.OverlapSphere(transform.position, radiusExplosion);
        Debug.Log(nearColliders.Length + " colliders found in the explosion radius.");

        foreach (Collider hit in nearColliders)
        {
            Rigidbody rb = hit.attachedRigidbody; 
            if (rb != null)// Verify if the collider has a Rigidbody attached to it
            {
                rb.AddExplosionForce(explosionForce, transform.position, radiusExplosion, upwardsModifier, ForceMode.Impulse);
            }
        }

        Destroy(gameObject); // Destroy the bomb object after the explosion
    }
}
