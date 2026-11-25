// ControleMicrontek.cs
// Faz o "Microntek USB Joystick" aparecer na Unity como um Gamepad (layout de Xbox).
// Assim, as actions ligadas a <Gamepad>/... funcionam sem mapeador externo.
//
// IMPORTANTE: feche o mapeador (x360ce/DS4Windows/etc.) ao testar, senão a entrada chega duplicada.
//
// Pontos para conferir no Input Debugger (marcados com "CONFERIR"):
//   1. Se o match por fabricante/produto funciona (ou trocar para VID/PID).
//   2. Se o analógico direito é mesmo Z (byte 4) e Rz (byte 5).
//   3. Qual botão físico é o Button 1..12 e em que byte/bit cada um está.

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

[StructLayout(LayoutKind.Explicit, Size = 9)] // o controle manda pacotes de 9 bytes
public struct MicrontekHIDState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('H', 'I', 'D');

    // ---------- Analógico esquerdo (bytes 1 e 2) ----------
    // No modo ANALOG desligado, o direcional também cai aqui (testado).
    [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
    [InputControl(name = "leftStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
    [InputControl(name = "leftStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(1)] public byte leftStickX;
    [FieldOffset(2)] public byte leftStickY;

    // ---------- Analógico direito (Z = byte 4, Rz = byte 5) ----------
    // CONFERIR: mexa o analógico direito e veja se z/rz mudam no Input Debugger.
    [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
    [InputControl(name = "rightStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
    [InputControl(name = "rightStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(4)] public byte rightStickX;
    [FieldOffset(5)] public byte rightStickY;

    // ---------- Byte 6: direcional (hat, bits 0-3) + Botões 1-4 (bits 4-7) ----------
    // Hat: 0=cima, 1=cima-dir, 2=dir, ... 7=cima-esq, 15=solto.
    [InputControl(name = "dpad", format = "BIT", layout = "Dpad", sizeInBits = 4, defaultState = 15)]
    [InputControl(name = "dpad/up", format = "BIT", layout = "DiscreteButton", parameters = "minValue=7,maxValue=1,nullValue=15,wrapAtValue=7", bit = 0, sizeInBits = 4)]
    [InputControl(name = "dpad/right", format = "BIT", layout = "DiscreteButton", parameters = "minValue=1,maxValue=3", bit = 0, sizeInBits = 4)]
    [InputControl(name = "dpad/down", format = "BIT", layout = "DiscreteButton", parameters = "minValue=3,maxValue=5", bit = 0, sizeInBits = 4)]
    [InputControl(name = "dpad/left", format = "BIT", layout = "DiscreteButton", parameters = "minValue=5,maxValue=7", bit = 0, sizeInBits = 4)]
    // CONFERIR: qual botão físico é cada "Button N". Chute pelo padrão desses controles:
    [InputControl(name = "buttonNorth", bit = 4)] // Button 1 (Y do Xbox)
    [InputControl(name = "buttonEast",  bit = 5)] // Button 2 (B do Xbox)
    [InputControl(name = "buttonSouth", bit = 6)] // Button 3 (A do Xbox)
    [InputControl(name = "buttonWest",  bit = 7)] // Button 4 (X do Xbox)
    [FieldOffset(6)] public byte hatAndButtons1;

    // ---------- Byte 7: Botões 5-12 ----------
    [InputControl(name = "leftTrigger",     format = "BIT", bit = 0)] // Button 5  (LT / L2)
    [InputControl(name = "rightTrigger",    format = "BIT", bit = 1)] // Button 6  (RT / R2)
    [InputControl(name = "leftShoulder",    bit = 2)]                 // Button 7  (LB / L1)
    [InputControl(name = "rightShoulder",   bit = 3)]                 // Button 8  (RB / R1)
    [InputControl(name = "select",          bit = 4)]                 // Button 9  (View / Select)
    [InputControl(name = "start",           bit = 5)]                 // Button 10 (Menu / Start)
    [InputControl(name = "leftStickPress",  bit = 6)]                 // Button 11 (L3)
    [InputControl(name = "rightStickPress", bit = 7)]                 // Button 12 (R3)
    [FieldOffset(7)] public byte buttons2;
}

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(stateType = typeof(MicrontekHIDState), displayName = "Controle Microntek (Xbox)")]
public class MicrontekGamepad : Gamepad
{
    static MicrontekGamepad()
    {
        // Vendor ID 0x79 / Product ID 0x6 (confirmado no HID Descriptor do controle).
        InputSystem.RegisterLayout<MicrontekGamepad>(
            matches: new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x79)
                .WithCapability("productId", 0x6));
    }

    // Garante que o construtor estático rode no build (fora do Editor).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() { }
}
