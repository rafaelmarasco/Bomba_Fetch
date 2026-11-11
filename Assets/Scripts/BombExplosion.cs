using UnityEngine;

public class BombExplosion : MonoBehaviour
{

    [Header("Event Manager")]
    [SerializeField] private EventManager eventManager;// Precisa melhorar isso. Como Bomb é um prefab, quando ele aparecer ele não vai estar conectado com o event manager

    [Header("Explosion Settings")]
    [SerializeField] private float radiusExplosion;
    [SerializeField] private float explosionForce;
    [SerializeField] private float upwardsModifier;//How tall the explosion will push the object
    [SerializeField] private GameObject vfxExplosion;
    [SerializeField] private GameObject vfxFireExplosion;
    [SerializeField] private VFXAudio vfxAudioExplosion;

    [SerializeField] private GameObject bomb;
    [SerializeField] private GameObject bombProp;
    [SerializeField] private GameObject bombKey;

    // Esse start é só para teste, o original provavelmente ele vai ser instanciado
    private void Start()
    {
        //eventManager.OnBombExploded += ExplodeBomb;
        Debug.Log("Start EventManager id: " + eventManager.GetInstanceID(), eventManager);
    }


    private void OnEnable()
    {
        Debug.Log("OnEnable EventManager id: " + eventManager.GetInstanceID(), eventManager);
        eventManager.OnBombExploded += ExplodeBomb;
    }

    private void OnDisable()
    {
        eventManager.OnBombExploded -= ExplodeBomb;
        Time.timeScale = 0f;
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

        vfxExplosion.SetActive(true);
        vfxAudioExplosion.PlayVFX();

        CancelInvoke(nameof(DestroyBomb));
        Invoke(nameof(DestroyBomb), 2f);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bomb != null ? bomb.transform.position : transform.position, radiusExplosion);
    }

    private void DestroyBomb()
    {
        bombProp.SetActive(false);
        bombKey.SetActive(false);
        vfxFireExplosion.SetActive(true);
    }
}
