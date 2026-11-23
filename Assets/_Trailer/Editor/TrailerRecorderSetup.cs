// ============================================================
//  TrailerRecorderSetup.cs  (Editor only — pasta Editor/)
//  Menu: Tools > Trailer > Configurar Recorder
//
//  Cria e salva um RecorderControllerSettings com:
//    - MovieRecorder (MP4 / H.264)
//    - 1920 × 1080, 60 fps
//    - Sem áudio
//    - Saída em  <projeto>/Trailer_Gravacoes/<cena>_<take>.mp4
// ============================================================

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;

public static class TrailerRecorderSetup
{
    private const string SettingsAssetPath = "Assets/_Trailer/Editor/TrailerRecorderSettings.asset";
    private const string OutputFolder = "../Trailer_Gravacoes";   // relativo a Assets/ → fora dele

    // ------------------------------------------------------------------ //
    //  Menu principal
    // ------------------------------------------------------------------ //

    [MenuItem("Tools/Trailer/Configurar Recorder (MP4 1080p 60fps)")]
    public static void SetupRecorder()
    {
        RecorderControllerSettings controllerSettings = CreateOrLoadSettings();

        controllerSettings.SetRecordModeToManual();
        controllerSettings.FrameRate = 60f;
        controllerSettings.CapFrameRate = true;

        // Remove recorders já existentes para não duplicar.
        // ClearRecorderSettings() é internal na 5.x, então usamos RemoveRecorder()
        // em cópia da lista (não modificar durante iteração).
        List<RecorderSettings> existing = controllerSettings.RecorderSettings.ToList();
        foreach (RecorderSettings old in existing)
            controllerSettings.RemoveRecorder(old);

        // ---- Configura o MovieRecorder ----
        MovieRecorderSettings movie = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        movie.name = "Trailer MP4";
        movie.Enabled = true;

        // Codec
        movie.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
        movie.VideoBitRateMode = VideoBitrateMode.High;

        // Áudio: desativado
        movie.AudioInputSettings.PreserveAudio = false;

        // Resolução
        movie.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth = 1920,
            OutputHeight = 1080,
            FlipFinalOutput = false
        };

        // Caminho de saída — <projeto>/Trailer_Gravacoes/<cena>_take_N.mp4
        movie.OutputFile = Path.Combine(OutputFolder, "<Scene>_take_<Take>");

        controllerSettings.AddRecorderSettings(movie);

        // Salva o asset
        if (!AssetDatabase.Contains(controllerSettings))
            AssetDatabase.CreateAsset(controllerSettings, SettingsAssetPath);

        EditorUtility.SetDirty(controllerSettings);
        AssetDatabase.SaveAssets();

        // Abre a janela do Recorder com as configurações
        RecorderWindow window = EditorWindow.GetWindow<RecorderWindow>("Recorder");
        window.SetRecorderControllerSettings(controllerSettings);
        window.Show();

        Debug.Log("[Trailer] Recorder configurado: 1920×1080, 60fps, MP4 sem áudio.\n" +
                  "Saída: <projeto_unity>/Trailer_Gravacoes/");
    }

    // ------------------------------------------------------------------ //
    //  Abre o Recorder com as configurações salvas
    // ------------------------------------------------------------------ //

    [MenuItem("Tools/Trailer/Abrir Recorder")]
    public static void OpenRecorder()
    {
        RecorderControllerSettings settings =
            AssetDatabase.LoadAssetAtPath<RecorderControllerSettings>(SettingsAssetPath);

        RecorderWindow window = EditorWindow.GetWindow<RecorderWindow>("Recorder");
        if (settings != null)
            window.SetRecorderControllerSettings(settings);
        window.Show();
    }

    // ------------------------------------------------------------------ //
    //  Abre a pasta de saída no Explorer
    // ------------------------------------------------------------------ //

    [MenuItem("Tools/Trailer/Abrir Pasta de Gravações")]
    public static void OpenOutputFolder()
    {
        string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string fullOutput = Path.GetFullPath(Path.Combine(projectPath, "Trailer_Gravacoes"));

        if (!Directory.Exists(fullOutput))
            Directory.CreateDirectory(fullOutput);

        EditorUtility.RevealInFinder(fullOutput);
    }

    // ------------------------------------------------------------------ //
    //  Helpers
    // ------------------------------------------------------------------ //

    private static RecorderControllerSettings CreateOrLoadSettings()
    {
        RecorderControllerSettings existing =
            AssetDatabase.LoadAssetAtPath<RecorderControllerSettings>(SettingsAssetPath);

        if (existing != null) return existing;

        // Garante que a pasta existe no AssetDatabase
        if (!AssetDatabase.IsValidFolder("Assets/_Trailer/Editor"))
            AssetDatabase.CreateFolder("Assets/_Trailer", "Editor");

        return ScriptableObject.CreateInstance<RecorderControllerSettings>();
    }
}
