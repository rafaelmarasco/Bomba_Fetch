#if UNITY_EDITOR
// ============================================================
//  TrailerCameraPushIn.cs
//  Câmera que se aproxima lentamente do player (push-in).
//  Começa a partir da posição inicial e avança para o alvo.
//  Quando chega, pode parar ou repetir.
// ============================================================

using UnityEngine;

[AddComponentMenu("_Trailer/Cameras/Trailer Camera Push-In")]
public class TrailerCameraPushIn : MonoBehaviour
{
    [Tooltip("Transform do player. Vazio = busca automática.")]
    [SerializeField] private Transform target;

    [SerializeField] private float approachSpeed = 2f;
    [SerializeField] private float stopDistance = 2.5f;    // distância mínima do alvo
    [SerializeField] private float lookHeightOffset = 1f;  // olhar um pouco acima do chão
    [SerializeField] private bool loop = true;             // reinicia ao chegar
    [SerializeField] private float resetDistance = 12f;    // recua para cá ao reiniciar

    private Vector3 startPos;

    private void Start()
    {
        if (target == null)
        {
            Player p = FindFirstObjectByType<Player>();
            if (p != null) target = p.transform;
        }
        startPos = transform.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 lookTarget = target.position + Vector3.up * lookHeightOffset;

        // Roda a câmera para olhar para o player
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(lookTarget - transform.position),
            Time.deltaTime * 5f
        );

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > stopDistance)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * approachSpeed * Time.deltaTime;
        }
        else if (loop)
        {
            // Recua para a posição inicial (relativa ao player para acompanhar o movimento)
            Vector3 backDir = (startPos - target.position).normalized;
            transform.position = target.position + backDir * resetDistance + Vector3.up * (startPos.y - target.position.y);
        }
    }
}
#endif
