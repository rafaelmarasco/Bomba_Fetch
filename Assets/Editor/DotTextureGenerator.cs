#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class DotTextureGenerator
{
    [MenuItem("Tools/Gerar textura pontilhada")]
    static void Generate()
    {
        const int size = 512, cell = 32;
        const string path = "Assets/Textures/TitleDots.png";

        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var light = Color.white;
        var dark = new Color(0.72f, 0.72f, 0.72f, 1f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float cx = (x / cell) * cell + cell * 0.5f;
                float cy = (y / cell) * cell + cell * 0.5f;
                float t = cy / size; // 0 = base, 1 = topo
                float radius = Mathf.Lerp(cell * 0.40f, cell * 0.10f, t);
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(cx, cy));
                float a = Mathf.Clamp01(radius - d + 0.5f);
                tex.SetPixel(x, y, Color.Lerp(light, dark, a));
            }
        }
        tex.Apply();

        Directory.CreateDirectory("Assets/Textures");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);

        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.mipmapEnabled = false;
        imp.npotScale = TextureImporterNPOTScale.None;
        imp.wrapModeU = TextureWrapMode.Repeat;
        imp.wrapModeV = TextureWrapMode.Clamp;
        imp.SaveAndReimport();
        Debug.Log("Textura gerada em " + path);
    }
}
#endif