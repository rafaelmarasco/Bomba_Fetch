#if UNITY_EDITOR
// ============================================================
//  TrailerCameraOrbit.cs
//  Câmera Cinemachine que orbita lentamente ao redor do player.
//  Coloque este script no GameObject pai do CinemachineCamera
//  para controlar a rotação do pivot de órbita.
//
//  No Inspector: arraste o Transform do player em "Target".
// ============================================================

using UnityEngine;

[AddComponentMenu("_Trailer/Cameras/Trailer Camera Orbit")]
public class TrailerCameraOrbit : MonoBehaviour
{
    [Tooltip("Transform do player a orbitar. Se vazio, busca automaticamente.")]
    [SerializeField] private Transform target;

    [SerializeField] private float orbitSpeed = 8f;       // graus por segundo
    [SerializeField] private float orbitHeight = 3f;       // altura do pivot
    [SerializeField] private float orbitRadius = 6f;       // distância horizontal

    private float currentAngle;

    private void Start()
    {
        if (target == null)
        {
            Player p = FindFirstObjectByType<Player>();
            if (p != null) target = p.transform;
        }
        currentAngle = transform.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        currentAngle += orbitSpeed * Time.deltaTime;

        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Sin(rad) * orbitRadius,
            orbitHeight,
            Mathf.Cos(rad) * orbitRadius
        );

        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * (orbitHeight * 0.5f));
    }
}
#endif
