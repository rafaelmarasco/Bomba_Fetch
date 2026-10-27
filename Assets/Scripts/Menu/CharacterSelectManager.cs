using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Slots (ordem = cor do jogador)")]
    [SerializeField] private List<CharacterSelectSlot> slots = new();

    [Header("Persistencia")]
    [Tooltip("Prefab com o componente PlayerSelectionData. Instanciado se nao existir.")]
    [SerializeField] private PlayerSelectionData selectionDataPrefab;

    [Header("Inicio do jogo")]
    [Tooltip("Nome da cena do jogo a carregar (precisa estar no Build Settings).")]
    [SerializeField] private string gameSceneName = "Sistema-Porta";

    [Tooltip("Minimo de jogadores para liberar o inicio.")]
    [SerializeField] private int minPlayersToStart = 1;

    private PlayerSelectionData selectionData;

    private readonly Dictionary<int, int> slotToDeviceId = new();

    private void Awake()
    {
        selectionData = PlayerSelectionData.Instance;
        if (selectionData == null && selectionDataPrefab != null)
            selectionData = Instantiate(selectionDataPrefab);

        selectionData?.Clear();

        for (int i = 0; i < slots.Count; i++)
            slots[i].SetEmpty();
    }

    private void Update()
    {
        ReadKeyboard();
        ReadGamepads();
        TryStartGame();
    }

    private void ReadKeyboard()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        if (kb.spaceKey.wasPressedThisFrame)
            TryJoin(kb.deviceId, "Keyboard&Mouse");

        if (kb.escapeKey.wasPressedThisFrame)
            TryLeave(kb.deviceId);
    }

    private void ReadGamepads()
    {
        foreach (Gamepad pad in Gamepad.all)
        {
            if (pad.buttonSouth.wasPressedThisFrame)
                TryJoin(pad.deviceId, "Gamepad");

            if (pad.buttonEast.wasPressedThisFrame)
                TryLeave(pad.deviceId);
        }
    }

    private void TryJoin(int deviceId, string controlScheme)
    {
        if (IsDeviceJoined(deviceId)) return;

        int freeSlot = GetFirstFreeSlot();
        if (freeSlot < 0) return;

        slots[freeSlot].SetOccupied();
        slotToDeviceId[freeSlot] = deviceId;
        selectionData?.AddPlayer(slots[freeSlot].SlotIndex, deviceId, controlScheme, slots[freeSlot].SlotMaterial);
    }

    private void TryLeave(int deviceId)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slotToDeviceId.TryGetValue(i, out int id) || id != deviceId) continue;

            slots[i].SetEmpty();
            selectionData?.RemovePlayerBySlot(slots[i].SlotIndex);
            slotToDeviceId.Remove(i);
            return;
        }
    }

    private void TryStartGame()
    {
        if (slotToDeviceId.Count < minPlayersToStart) return;

        bool startPressed = false;

        Keyboard kb = Keyboard.current;
        if (kb != null && (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame))
            startPressed = true;

        foreach (Gamepad pad in Gamepad.all)
            if (pad.startButton.wasPressedThisFrame)
                startPressed = true;

        if (startPressed)
            SceneManager.LoadScene(gameSceneName);
    }

    private bool IsDeviceJoined(int deviceId)
    {
        foreach (var id in slotToDeviceId.Values)
            if (id == deviceId) return true;
        return false;
    }

    private int GetFirstFreeSlot()
    {
        for (int i = 0; i < slots.Count; i++)
            if (!slotToDeviceId.ContainsKey(i)) return i;
        return -1;
    }
}
