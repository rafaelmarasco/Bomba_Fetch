using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    [Tooltip("Pontos de nascimento, na ordem dos slots (0..3).")]
    [SerializeField] private Transform[] spawnPoints;

    [Tooltip("Qual material do renderer do corpo trocar pela cor (normalmente 0).")]
    [SerializeField] private int materialIndex = 0;

    private void Start()
    {
        PlayerSelectionData data = PlayerSelectionData.Instance;

        if (data == null || data.JoinedPlayers.Count == 0)
        {
            if (Keyboard.current != null)
                Spawn(0, "Keyboard&Mouse", null, Keyboard.current, Mouse.current);
            return;
        }

        foreach (var joined in data.JoinedPlayers)
        {
            InputDevice device = InputSystem.GetDeviceById(joined.deviceId);
            if (device == null) continue;

            if (joined.controlScheme == "Keyboard&Mouse")
                Spawn(joined.slotIndex, joined.controlScheme, joined.material, device, Mouse.current);
            else
                Spawn(joined.slotIndex, joined.controlScheme, joined.material, device);
        }
    }

    private void Spawn(int slotIndex, string controlScheme, Material material, params InputDevice[] devices)
    {
        Transform point = spawnPoints != null && slotIndex < spawnPoints.Length ? spawnPoints[slotIndex] : null;
        Vector3 pos = point != null ? point.position : new Vector3(slotIndex * 2f, 1f, 0f);
        Quaternion rot = point != null ? point.rotation : Quaternion.identity;

        PlayerInput input = PlayerInput.Instantiate(playerPrefab, slotIndex, controlScheme, -1, devices);
        input.transform.SetPositionAndRotation(pos, rot);

        ApplyColor(input.gameObject, material);
    }

    private void ApplyColor(GameObject player, Material material)
    {
        if (material == null) return;

        var renderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer == null) return;

        Material[] mats = renderer.materials;
        if (materialIndex >= 0 && materialIndex < mats.Length)
        {
            mats[materialIndex] = material;
            renderer.materials = mats;
        }
    }
}
