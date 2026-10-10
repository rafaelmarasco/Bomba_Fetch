using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private enum Cameras // REMOVER ???????????????
    {
        main,
        bomb
    }

    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private Camera outputCamera;
    [SerializeField] private int maxPlayers = 4;

    [Header("Zoom Limits")]
    [SerializeField] private float minZoom = 7f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float padding = 3f;

    [Header("Smoothing")]
    [SerializeField] private float zoomSmoothTime = 0.35f;
    [SerializeField] private float moveSmoothTime = 0.25f;
    [SerializeField] private float searchInterval = 0.5f;

    private readonly List<Transform> players = new();
    private Transform pivot;
    private CinemachinePositionComposer composer;
    private float currentZoom;
    private float zoomVelocity;
    private Vector3 moveVelocity;
    private float searchTimer;
    private bool hasCentered;

    private void Awake()
    {
        // Pivot no centro dos jogadores que a camera segue
        pivot = new GameObject("CameraPivot").transform;
        mainCamera.Target.TrackingTarget = pivot;

        if (outputCamera == null) outputCamera = Camera.main;

        // O zoom e a distancia do composer da camera
        composer = mainCamera.GetComponent<CinemachinePositionComposer>();
        currentZoom = composer != null ? composer.CameraDistance : minZoom;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    public void MakeCameraFollow(PlayerInput input)
    {
        AddPlayer(input.transform);
    }

    private void AddPlayer(Transform player)
    {
        if (players.Count >= maxPlayers || players.Contains(player)) return;
        players.Add(player);
    }

    // Garante que jogadores que entraram sem o evento tambem sejam seguidos
    private void SearchPlayers()
    {
        searchTimer -= Time.deltaTime;
        if (searchTimer > 0f || players.Count >= maxPlayers) return;
        searchTimer = searchInterval;

        foreach (Player found in FindObjectsByType<Player>(FindObjectsSortMode.None))
            AddPlayer(found.transform);
    }

    private void LateUpdate()
    {
        players.RemoveAll(p => p == null);
        SearchPlayers();
        if (players.Count == 0) return;

        Vector3 center = GetCenter(out float distance);

        // Primeiro frame com jogador: posiciona o pivot sem suavizar
        if (!hasCentered)
        {
            pivot.position = center;
            hasCentered = true;
        }
        pivot.position = Vector3.SmoothDamp(pivot.position, center, ref moveVelocity, moveSmoothTime);

        float targetZoom = Mathf.Clamp(distance + padding, minZoom, maxZoom);
        currentZoom = Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVelocity, zoomSmoothTime);
        if (composer != null) composer.CameraDistance = currentZoom;
    }

    // Centro do grupo e distancia necessaria para enquadrar todos
    private Vector3 GetCenter(out float distance)
    {
        Bounds bounds = new(players[0].position, Vector3.zero);
        for (int i = 1; i < players.Count; i++)
            bounds.Encapsulate(players[i].position);

        Transform camT = mainCamera.transform;
        float width = Mathf.Abs(Vector3.Dot(bounds.size, camT.right));
        float height = Mathf.Abs(Vector3.Dot(bounds.size, camT.up));

        float aspect = outputCamera != null ? outputCamera.aspect : 16f / 9f;
        float halfHeight = Mathf.Max(height * 0.5f, width * 0.5f / aspect);

        float halfFov = mainCamera.Lens.FieldOfView * Mathf.Deg2Rad * 0.5f;
        distance = halfHeight / Mathf.Tan(halfFov);
        return bounds.center;
    }

    /*
    private IEnumerator MoveCameraToBomb()
    {
        Debug.Log("Come�ou a mover a camera");

        Vector3 bombFowardOffSet = bomb.forward * .65f;
        Vector3 downOffSet = Vector3.down * .25f;
        //Vector3 fwdOffSet = bomb.right * -.3f;

        Vector3 startPos = mainCamera.transform.position;
        Vector3 finalPos = bomb.position + bombFowardOffSet + downOffSet; //+ fwdOffSet;


        float timePassed = 0f;
        float duration = .2f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;

            bombCamera.transform.position = Vector3.Lerp(startPos, finalPos, timePassed / duration);

            Vector3 bombDir = bomb.position - bombCamera.transform.position;
            bombCamera.transform.rotation = Quaternion.LookRotation(bombDir, bomb.up);
            yield return null;
        }
        bombCamera.transform.position = finalPos;
        bombCamera.transform.rotation = Quaternion.LookRotation(bomb.position - finalPos, bomb.up);
        head.SetActive(false);
    }
    */
}
