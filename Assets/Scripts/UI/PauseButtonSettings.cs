using UnityEngine;
using UnityEngine.UI;

public class PauseButtonSettings : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Button controlButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("GameObjects")]
    [SerializeField] private GameObject pauseCanvas;
    private SceneController sceneController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneController = new SceneController();
        SettingButton();
    }

    private void SettingButton()
    {
        controlButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(() => DesactiveCanvas());
        mainMenuButton.onClick.AddListener(() => GoMainMenu());
        quitButton.onClick.AddListener(() => SairDoJogo());
    }

    private void DesactiveCanvas()
    {
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    private void GoMainMenu()
    {
        sceneController.LoadScene(0);
    }

    private void SairDoJogo()
    {
        Application.Quit();
    }
}
