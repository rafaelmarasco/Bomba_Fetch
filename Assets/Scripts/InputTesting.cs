using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
public class InputTesting : MonoBehaviour
{
    private void OnEnable()
    {
        InputSystem.onEvent += OnIntputEvent;
    }

    private void OnIntputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (device is Gamepad gamepad)
            if (gamepad.buttonSouth.wasPressedThisFrame)
                Debug.Log($"Input vindo de: {device.displayName}");
    }
}
