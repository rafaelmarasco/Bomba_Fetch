using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Paineis")]
    [Tooltip("Objeto com os 3 botoes (Start, Configuracoes, Sair).")]
    [SerializeField] private GameObject menuPanel;

    [Tooltip("Painel de configuracoes (opcional).")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Selecao de personagem")]
    [SerializeField] private CharacterSelectManager selectionManager;

    [Tooltip("Tudo que so aparece na selecao: titulo, dica e slots.")]
    [SerializeField] private GameObject[] selectionObjects;

    private void Start()
    {
        ShowMenu();
    }

    public void OpenCharacterSelect()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        SetSelectionVisible(true);
    }

    public void OpenSettings()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void ShowMenu()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        SetSelectionVisible(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetSelectionVisible(bool visible)
    {
        if (selectionObjects != null)
            foreach (var go in selectionObjects)
                if (go != null) go.SetActive(visible);

        if (selectionManager != null) selectionManager.enabled = visible;
    }
}
