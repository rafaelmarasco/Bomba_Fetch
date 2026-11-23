using UnityEngine;

public class HoldPointSphereCast : MonoBehaviour
{
    [SerializeField] private Transform player;                                  // o objeto que gira com o personagem
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 1f, 1.2f);  // relativo ao Player: altura do peito, 1,2 m à frente
    [SerializeField] private float originHeight = 1f;                           // altura do peito, de onde o cast sai
    [SerializeField] private float radius = 0.4f;
    [SerializeField] private float minDistance = 0.3f;
    [SerializeField] private LayerMask wallMask;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        transform.SetParent(null);
    }

    private void FixedUpdate()
    {
        Vector3 origin = player.position + originHeight * Vector3.up;
        Vector3 target = player.TransformPoint(localOffset);
        Vector3 dir = target - origin;
        float dist = dir.magnitude;
        dir /= dist;

        Vector3 pos = target;

        if (Physics.SphereCast(origin, radius, dir, out RaycastHit hit, dist, wallMask, QueryTriggerInteraction.Ignore))
            pos = origin + dir * Mathf.Clamp(hit.distance, minDistance, dist);

        rb.MovePosition(pos);
        rb.MoveRotation(player.rotation);
    }
}
