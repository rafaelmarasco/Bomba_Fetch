// ============================================================
//  TrailerHelpWindow.cs  (Editor only)
//  Menu: Tools > Trailer > Guia de Uso
//  Janela de referência rápida com todas as teclas e câmeras.
// ============================================================

using UnityEditor;
using UnityEngine;

public class TrailerHelpWindow : EditorWindow
{
    private Vector2 scroll;

    [MenuItem("Tools/Trailer/Guia de Uso")]
    public static void ShowWindow()
    {
        TrailerHelpWindow w = GetWindow<TrailerHelpWindow>("Guia do Modo Trailer");
        w.minSize = new Vector2(420, 500);
        w.Show();
    }

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        GUIStyle title = new GUIStyle(EditorStyles.boldLabel) { fontSize = 15 };
        GUIStyle section = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
        GUIStyle body = new GUIStyle(EditorStyles.label) { wordWrap = true };

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("🎬  Modo Trailer — Bomb Fetcher", title);
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Funciona só no Editor. Teclas ativas somente durante o Play Mode.", body);
        EditorGUILayout.Space(10);

        // ---- Teclas ----
        EditorGUILayout.LabelField("TECLAS", section);
        DrawRow("F1", "Liga / desliga o Modo Trailer inteiro.\n" +
                      "Ao desligar, restaura timeScale, HUD e câmera ao estado original.");
        DrawRow("F2", "Mostra / oculta todo o HUD e UI da cena (todos os Canvas).\n" +
                      "Útil para tomadas sem interface.");
        DrawRow("F3", "Alterna câmera lenta (timeScale 0.3×) ↔ velocidade normal.\n" +
                      "Não funciona se o jogo estiver pausado com F4.");
        DrawRow("F4", "Pausa / retoma o jogo SEM abrir o menu de pausa.\n" +
                      "A FlyCam continua funcionando normalmente mesmo pausada.");
        DrawRow("F5", "Ativa / desativa a câmera livre (FlyCam).\n" +
                      "Desativa o CameraManager do jogo enquanto estiver ativa.");

        EditorGUILayout.Space(10);

        // ---- FlyCam ----
        EditorGUILayout.LabelField("FLY CAM (F5)", section);
        DrawRow("Mouse", "Olhar ao redor (pitch + yaw).");
        DrawRow("W / S", "Mover para frente / trás.");
        DrawRow("A / D", "Mover para esquerda / direita.");
        DrawRow("Q / E", "Descer / subir.");
        DrawRow("Shift", "Multiplicar velocidade (padrão: 3×).");
        DrawRow("Scroll", "Ajustar velocidade base da câmera.");

        EditorGUILayout.Space(10);

        // ---- Câmeras Cinemachine ----
        EditorGUILayout.LabelField("CÂMERAS CINEMATOGRÁFICAS", section);

        EditorGUILayout.LabelField(
            "Arraste os prefabs de Assets/_Trailer/Prefabs/ para a cena e posicione-os a gosto.\n" +
            "Ative apenas UM de cada vez pela prioridade do Cinemachine Brain ou desative os outros.", body);
        EditorGUILayout.Space(4);

        DrawRow("TC_OrbitPlayer\n(TrailerCameraOrbit)", 
                "Orbita lentamente em volta do player.\n" +
                "Inspector: ajuste Orbit Speed (graus/s), Orbit Height e Orbit Radius.");
        DrawRow("TC_PushIn\n(TrailerCameraPushIn)", 
                "Avança devagar em direção ao player (push-in).\n" +
                "Inspector: Approach Speed, Stop Distance. 'Loop' reinicia automaticamente.");
        DrawRow("TC_LowAngle\n(TrailerCameraLowAngle)", 
                "Câmera rente ao chão seguindo o player.\n" +
                "Inspector: Offset (posição relativa ao player), Follow Smooth.");
        DrawRow("TC_DollyPan\n(TrailerCameraDolly)", 
                "Câmera em trilho para panorâmica.\n" +
                "Crie filhos chamados WP_0, WP_1, WP_2... e posicione-os como waypoints.\n" +
                "Se 'Look At Target' for preenchido, a câmera segue esse objeto ao longo do trilho.\n" +
                "Trilho azul visível no Scene View.");

        EditorGUILayout.Space(10);

        // ---- Recorder ----
        EditorGUILayout.LabelField("GRAVAÇÃO (Unity Recorder)", section);
        EditorGUILayout.LabelField(
            "1. Tools > Trailer > Configurar Recorder  →  cria as configurações e abre a janela.\n" +
            "2. Entre no Play Mode.\n" +
            "3. Na janela Recorder, clique em START RECORDING.\n" +
            "4. Use as teclas do Modo Trailer para posicionar e gravar.\n" +
            "5. Clique em STOP RECORDING ou saia do Play Mode.\n" +
            "6. Tools > Trailer > Abrir Pasta de Gravações  →  abre a pasta no Explorer.\n\n" +
            "Configuração: MP4 (H.264) · 1920×1080 · 60fps · sem áudio\n" +
            "Destino: <projeto>/Trailer_Gravacoes/<cena>_take_N.mp4", body);

        EditorGUILayout.Space(10);

        // ---- Dicas ----
        EditorGUILayout.LabelField("DICAS DE TOMADA", section);
        EditorGUILayout.LabelField(
            "• F3 (câmera lenta) + câmera Orbit = tomada dramática girando ao redor do caos.\n" +
            "• F4 (pausa) + F5 (FlyCam) = enquadre o momento perfeito sem o jogo continuar.\n" +
            "• Dolly com Look At no player = panorâmica do cenário mantendo o player no foco.\n" +
            "• Oculte o HUD (F2) antes de apertar START RECORDING no Recorder.", body);

        EditorGUILayout.Space(10);
        EditorGUILayout.EndScrollView();
    }

    private void DrawRow(string key, string desc)
    {
        GUIStyle keyStyle = new GUIStyle(EditorStyles.boldLabel) { fixedWidth = 160 };
        GUIStyle descStyle = new GUIStyle(EditorStyles.label) { wordWrap = true };

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(key, keyStyle);
        EditorGUILayout.LabelField(desc, descStyle);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(2);
    }
}
