#if UNITY_EDITOR
// ============================================================
//  TrailerCameraDolly.cs
//  Câmera em trilho (dolly) para panorâmica do cenário.
//
//  COMO USAR:
//  1. Crie GameObjects filhos chamados "WP_0", "WP_1", "WP_2"...
//     e posicione-os como waypoints do trilho na cena.
//  2. A câmera percorre os waypoints em ordem e pode fazer loop.
//  3. Se "lookAtTarget" for preenchido, a câmera olha para ele
//     ao longo de todo o percurso (boa para seguir a ação).
// ============================================================

using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("_Trailer/Cameras/Trailer Camera Dolly")]
public class TrailerCameraDolly : MonoBehaviour
{
    [Header("Trilho")]
    [Tooltip("Waypoints do trilho em ordem. Crie GameObjects filhos e arraste-os aqui, ou deixe vazio para usar filhos nomeados WP_0, WP_1...")]
    [SerializeField] private List<Transform> waypoints = new();

    [SerializeField] private float dollySpeed = 3f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool pingPong = false;    // vai e volta em vez de loop circular

    [Header("Look")]
    [Tooltip("Se preenchido, a câmera olha para este Transform ao longo do trilho. Deixe vazio para olhar na direção do movimento.")]
    [SerializeField] private Transform lookAtTarget;
    [SerializeField] private float lookSmoothSpeed = 5f;

    // ---- Estado ----
    private int currentWP = 0;
    private int direction = 1;    // 1 = avançando, -1 = voltando (pingPong)

    private void Start()
    {
        // Auto-coleta waypoints filhos se a lista estiver vazia
        if (waypoints.Count == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name.StartsWith("WP_"))
                    waypoints.Add(child);
            }
            waypoints.Sort((a, b) =>
            {
                int ia = int.Parse(a.name.Substring(3));
                int ib = int.Parse(b.name.Substring(3));
                return ia.CompareTo(ib);
            });
        }

        if (waypoints.Count > 0)
            transform.position = waypoints[0].position;
    }

    private void LateUpdate()
    {
        if (waypoints.Count < 2) return;

        Transform wp = waypoints[currentWP];
        float step = dollySpeed * Time.deltaTime;
        float dist = Vector3.Distance(transform.position, wp.position);

        if (dist <= step)
        {
            transform.position = wp.position;
            AdvanceWaypoint();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, wp.position, step);
        }

        // Rotação
        if (lookAtTarget != null)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookAtTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * lookSmoothSpeed);
        }
        else
        {
            // Olha na direção do próximo waypoint
            Vector3 dir = (wp.position - transform.position);
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * lookSmoothSpeed);
            }
        }
    }

    private void AdvanceWaypoint()
    {
        if (pingPong)
        {
            currentWP += direction;
            if (currentWP >= waypoints.Count) { currentWP = waypoints.Count - 2; direction = -1; }
            else if (currentWP < 0) { currentWP = 1; direction = 1; }
        }
        else
        {
            currentWP++;
            if (currentWP >= waypoints.Count)
                currentWP = loop ? 0 : waypoints.Count - 1;
        }
    }

    // Desenha o trilho no Editor
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
        if (loop && waypoints[^1] != null && waypoints[0] != null)
            Gizmos.DrawLine(waypoints[^1].position, waypoints[0].position);

        Gizmos.color = Color.yellow;
        foreach (Transform wp in waypoints)
            if (wp != null) Gizmos.DrawSphere(wp.position, 0.2f);
    }
}
#endif
