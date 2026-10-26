using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectSlot : MonoBehaviour
{
    [Header("Identidade do slot")]
    [Tooltip("Indice do slot (0..3). Define a cor/ordem do jogador.")]
    [SerializeField] private int slotIndex;

    [Tooltip("Cor/material aplicado ao personagem deste slot.")]
    [SerializeField] private Material slotMaterial;

    [Header("Preview do personagem")]
    [Tooltip("Prefab do personagem (normalmente o prefab Player).")]
    [SerializeField] private GameObject characterPrefab;

    [Tooltip("Onde o preview sera instanciado. Se vazio, usa este transform.")]
    [SerializeField] private Transform spawnPoint;

    [Tooltip("Multiplicador de tamanho do preview.")]
    [SerializeField] private float previewScale = 2.5f;

    [Header("Visuais")]
    [Tooltip("Objeto mostrado quando o slot esta VAZIO (ex: texto 'Press A').")]
    [SerializeField] private GameObject emptyVisual;

    [Tooltip("Qual material do renderer do corpo trocar pela cor (normalmente 0).")]
    [SerializeField] private int materialIndex = 0;

    public int SlotIndex => slotIndex;
    public Material SlotMaterial => slotMaterial;
    public bool IsOccupied { get; private set; }

    private GameObject spawnedPreview;

    private void Awake()
    {
        SetEmpty();
    }

    public void SetEmpty()
    {
        IsOccupied = false;

        if (spawnedPreview != null)
        {
            Destroy(spawnedPreview);
            spawnedPreview = null;
        }

        if (emptyVisual != null) emptyVisual.SetActive(true);
    }

    public void SetOccupied()
    {
        IsOccupied = true;
        if (emptyVisual != null) emptyVisual.SetActive(false);

        SpawnPreview();
    }

    private void SpawnPreview()
    {
        if (characterPrefab == null) return;

        Transform parent = spawnPoint != null ? spawnPoint : transform;
        spawnedPreview = Instantiate(characterPrefab, parent.position, parent.rotation, parent);

        spawnedPreview.transform.localScale *= previewScale;
        FaceCamera(spawnedPreview.transform);

        StripGameplayComponents(spawnedPreview);
        ApplyColor(spawnedPreview);
    }

    private static void FaceCamera(Transform preview)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 dir = cam.transform.position - preview.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            preview.rotation = Quaternion.LookRotation(dir);
    }

    private void StripGameplayComponents(GameObject preview)
    {
        foreach (var cam in preview.GetComponentsInChildren<Camera>(true))
            cam.enabled = false;

        foreach (var listener in preview.GetComponentsInChildren<AudioListener>(true))
            Destroy(listener);

        foreach (var canvas in preview.GetComponentsInChildren<Canvas>(true))
            canvas.gameObject.SetActive(false);

        foreach (var rb in preview.GetComponentsInChildren<Rigidbody>(true))
            rb.isKinematic = true;

        foreach (var col in preview.GetComponentsInChildren<Collider>(true))
            col.enabled = false;

        foreach (var input in preview.GetComponentsInChildren<PlayerInput>(true))
            input.enabled = false;

        foreach (var mb in preview.GetComponentsInChildren<MonoBehaviour>(true))
        {
            string ns = mb.GetType().Namespace ?? "";
            if (ns.StartsWith("UnityEngine")) continue;
            mb.enabled = false;
        }
    }

    private void ApplyColor(GameObject preview)
    {
        if (slotMaterial == null) return;

        var renderer = preview.GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer == null) return;

        Material[] mats = renderer.materials;
        if (materialIndex >= 0 && materialIndex < mats.Length)
        {
            mats[materialIndex] = slotMaterial;
            renderer.materials = mats;
        }
    }
}
