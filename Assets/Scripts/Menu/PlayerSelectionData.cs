using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectionData : MonoBehaviour
{
    public struct JoinedPlayer
    {
        public int slotIndex;
        public int deviceId;
        public string controlScheme;
        public Material material;
    }

    public static PlayerSelectionData Instance { get; private set; }

    private readonly List<JoinedPlayer> joinedPlayers = new();
    public IReadOnlyList<JoinedPlayer> JoinedPlayers => joinedPlayers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Clear() => joinedPlayers.Clear();

    public bool IsSlotTaken(int slotIndex)
    {
        foreach (var p in joinedPlayers)
            if (p.slotIndex == slotIndex) return true;
        return false;
    }

    public bool IsDeviceJoined(int deviceId)
    {
        foreach (var p in joinedPlayers)
            if (p.deviceId == deviceId) return true;
        return false;
    }

    public void AddPlayer(int slotIndex, int deviceId, string controlScheme, Material material)
    {
        joinedPlayers.Add(new JoinedPlayer
        {
            slotIndex = slotIndex,
            deviceId = deviceId,
            controlScheme = controlScheme,
            material = material
        });
    }

    public void RemovePlayerBySlot(int slotIndex)
    {
        joinedPlayers.RemoveAll(p => p.slotIndex == slotIndex);
    }
}
