#if UNITY_EDITOR
// ============================================================
//  TrailerCameraLowAngle.cs
//  Câmera de ângulo baixo que segue o player.
//  Fica rente ao chão, olhando ligeiramente para cima,
//  com leve delay para suavizar o movimento.
// ============================================================

using UnityEngine;

[AddComponentMenu("_Trailer/Cameras/Trailer Camera Low Angle")]
public class TrailerCameraLowAngle : MonoBehaviour
{
    [Tooltip("Transform do player. Vazio = busca automática.")]
    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset = new Vector3(0f, 0.4f, -4f);  // relativo ao player
    [SerializeField] private float followSmooth = 0.15f;
    [SerializeField] private float lookHeightOffset = 0.8f;    // para onde olhar (altura no player)

    private Vector3 smoothVel;

    private void Start()
    {
        if (target == null)
        {
            Player p = FindFirstObjectByType<Player>();
            if (p != null) target = p.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calcula posição desejada no espaço do mundo (offset relativo à rotação do player)
        Vector3 desiredPos = target.TransformPoint(offset);

        transform.position = Vector3.SmoothDamp(
            transform.position, desiredPos, ref smoothVel, followSmooth);

        // Olha para o player com leve offset de altura
        Vector3 lookTarget = target.position + Vector3.up * lookHeightOffset;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(lookTarget - transform.position),
            Time.deltaTime * 8f
        );
    }
}
#endif
