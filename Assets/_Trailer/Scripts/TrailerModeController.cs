#if UNITY_EDITOR
// ============================================================
//  TrailerModeController.cs
//  Orquestrador do Modo Trailer — só compila no Editor.
//
//  Teclas:
//    F1  Liga / desliga o Modo Trailer inteiro
//    F2  Mostra / oculta todo o HUD (Canvas da cena)
//    F3  Câmera lenta (timeScale 0.3) / velocidade normal
//    F4  Pausa o jogo sem acionar o PauseSystem do jogo
//    F5  Ativa / desativa a FlyCam
// ============================================================

using UnityEngine;

[AddComponentMenu("_Trailer/Trailer Mode Controller")]
public class TrailerModeController : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste aqui o GameObject que contém o CameraManager da cena.")]
    [SerializeField] private CameraManager cameraManager;

    [Tooltip("FlyCam no mesmo prefab — preenchida automaticamente se estiver no mesmo GameObject.")]
    [SerializeField] private TrailerFlyCam flyCam;

    [Tooltip("Invincibility helper no mesmo prefab.")]
    [SerializeField] private TrailerInvincibility invincibility;

    [Header("Configuração")]
    [SerializeField] private float slowMotionScale = 0.3f;

    // ---- Estado ----
    public static bool IsTrailerModeActive { get; private set; }

    private bool hudVisible = true;
    private bool slowMotion = false;
    private bool gamePaused = false;
    private bool flyCamActive = false;

    private Canvas[] allCanvases;
    private bool[] canvasWasEnabled;    // estado original de cada Canvas

    // ------------------------------------------------------------------ //

    private void Awake()
    {
        // Auto-resolve componentes no mesmo GameObject / filhos
        if (flyCam == null) flyCam = GetComponentInChildren<TrailerFlyCam>();
        if (invincibility == null) invincibility = GetComponentInChildren<TrailerInvincibility>();

        if (flyCam != null) flyCam.enabled = false;
    }

    private void Update()
    {
        // F1 — Liga / desliga o Modo Trailer
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (IsTrailerModeActive) DeactivateTrailerMode();
            else ActivateTrailerMode();
            return;
        }

        if (!IsTrailerModeActive) return;

        // F2 — HUD toggle
        if (Input.GetKeyDown(KeyCode.F2)) ToggleHUD();

        // F3 — Câmera lenta
        if (Input.GetKeyDown(KeyCode.F3)) ToggleSlowMotion();

        // F4 — Pausa (sem menu do jogo)
        if (Input.GetKeyDown(KeyCode.F4)) TogglePause();

        // F5 — FlyCam
        if (Input.GetKeyDown(KeyCode.F5)) ToggleFlyCam();
    }

    // ------------------------------------------------------------------ //
    //  Ativar / Desativar
    // ------------------------------------------------------------------ //

    private void ActivateTrailerMode()
    {
        IsTrailerModeActive = true;
        Debug.Log("[Trailer] Modo Trailer ATIVADO");

        // Registra o estado atual de todos os Canvas para poder restaurar
        allCanvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        canvasWasEnabled = new bool[allCanvases.Length];
        for (int i = 0; i < allCanvases.Length; i++)
            canvasWasEnabled[i] = allCanvases[i].enabled;

        // Ativa invencibilidade
        if (invincibility != null) invincibility.SetInvincible(true);
    }

    private void DeactivateTrailerMode()
    {
        IsTrailerModeActive = false;
        Debug.Log("[Trailer] Modo Trailer DESATIVADO — restaurando estado original");

        // Restaura HUD
        if (!hudVisible) RestoreHUD();

        // Restaura timeScale
        if (slowMotion || gamePaused)
        {
            Time.timeScale = 1f;
            slowMotion = false;
            gamePaused = false;
        }

        // Desativa FlyCam
        if (flyCamActive) DeactivateFlyCam();

        // Remove invencibilidade
        if (invincibility != null) invincibility.SetInvincible(false);
    }

    // ------------------------------------------------------------------ //
    //  HUD  (F2)
    // ------------------------------------------------------------------ //

    private void ToggleHUD()
    {
        if (hudVisible) HideHUD();
        else RestoreHUD();
    }

    private void HideHUD()
    {
        // Re-captura para pegar Canvas criados após ativar o modo
        allCanvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        canvasWasEnabled = new bool[allCanvases.Length];
        for (int i = 0; i < allCanvases.Length; i++)
        {
            canvasWasEnabled[i] = allCanvases[i].enabled;
            allCanvases[i].enabled = false;
        }
        hudVisible = false;
        Debug.Log("[Trailer] HUD ocultado");
    }

    private void RestoreHUD()
    {
        if (allCanvases == null) return;
        for (int i = 0; i < allCanvases.Length; i++)
        {
            if (allCanvases[i] != null)
                allCanvases[i].enabled = canvasWasEnabled[i];
        }
        hudVisible = true;
        Debug.Log("[Trailer] HUD restaurado");
    }

    // ------------------------------------------------------------------ //
    //  Câmera Lenta  (F3)
    // ------------------------------------------------------------------ //

    private void ToggleSlowMotion()
    {
        // Não aplica câmera lenta se o jogo estiver pausado
        if (gamePaused) return;

        slowMotion = !slowMotion;
        Time.timeScale = slowMotion ? slowMotionScale : 1f;
        Debug.Log(slowMotion ? $"[Trailer] Câmera lenta ({slowMotionScale}x)" : "[Trailer] Velocidade normal");
    }

    // ------------------------------------------------------------------ //
    //  Pausa sem menu  (F4)
    // ------------------------------------------------------------------ //

    private void TogglePause()
    {
        gamePaused = !gamePaused;

        if (gamePaused)
        {
            // Desativa câmera lenta antes de pausar para não conflitar
            slowMotion = false;
            Time.timeScale = 0f;
            Debug.Log("[Trailer] Jogo pausado (FlyCam continua com unscaledDeltaTime)");
        }
        else
        {
            Time.timeScale = 1f;
            Debug.Log("[Trailer] Jogo retomado");
        }
    }

    // ------------------------------------------------------------------ //
    //  FlyCam  (F5)
    // ------------------------------------------------------------------ //

    private void ToggleFlyCam()
    {
        if (flyCamActive) DeactivateFlyCam();
        else ActivateFlyCam();
    }

    private void ActivateFlyCam()
    {
        flyCamActive = true;

        // Desativa o CameraManager para a câmera do jogo parar de se mover
        if (cameraManager != null) cameraManager.enabled = false;

        if (flyCam != null) flyCam.enabled = true;

        Debug.Log("[Trailer] FlyCam ATIVADA — WASD + Mouse, Q/E sobe/desce, Shift acelera");
    }

    private void DeactivateFlyCam()
    {
        flyCamActive = false;

        if (flyCam != null) flyCam.enabled = false;

        // Reativa o CameraManager
        if (cameraManager != null) cameraManager.enabled = true;

        Debug.Log("[Trailer] FlyCam desativada — câmera do jogo restaurada");
    }

    // ------------------------------------------------------------------ //
    //  Gizmo de status no Editor
    // ------------------------------------------------------------------ //

    private void OnGUI()
    {
        if (!IsTrailerModeActive) return;

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 13;
        style.normal.textColor = Color.yellow;

        string status =
            $"[TRAILER MODE]\n" +
            $"F1 Desligar  |  F2 HUD: {(hudVisible ? "visível" : "oculto")}\n" +
            $"F3 SlowMo: {(slowMotion ? "ON" : "OFF")}  |  F4 Pausa: {(gamePaused ? "ON" : "OFF")}\n" +
            $"F5 FlyCam: {(flyCamActive ? "ATIVA" : "off")}";

        GUI.Box(new Rect(10, 10, 320, 80), status, style);
    }
}
#endif
