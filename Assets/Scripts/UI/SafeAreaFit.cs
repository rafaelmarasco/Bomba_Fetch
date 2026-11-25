using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    [Range(0f, 0.1f)][SerializeField] float extraMargin = 0.05f; // margem extra para TVs

    RectTransform rt;
    Rect lastSafe;
    Vector2Int lastScreen;

    void Awake() { rt = GetComponent<RectTransform>(); Apply(); }

    void Update()
    {
        if (Screen.safeArea != lastSafe ||
            lastScreen.x != Screen.width || lastScreen.y != Screen.height)
            Apply();
    }

    void Apply()
    {
        lastSafe = Screen.safeArea;
        lastScreen = new Vector2Int(Screen.width, Screen.height);

        Vector2 min = lastSafe.position;
        Vector2 max = lastSafe.position + lastSafe.size;

        min.x += Screen.width * extraMargin;
        max.x -= Screen.width * extraMargin;
        min.y += Screen.height * extraMargin;
        max.y -= Screen.height * extraMargin;

        rt.anchorMin = new Vector2(min.x / Screen.width, min.y / Screen.height);
        rt.anchorMax = new Vector2(max.x / Screen.width, max.y / Screen.height);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}