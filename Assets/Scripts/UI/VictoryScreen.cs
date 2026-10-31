using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private Button tryAgain;
    private SceneController sceneController;

    private void OnEnable()
    {
        sceneController = new SceneController();
        tryAgain.onClick.RemoveAllListeners();
        tryAgain.onClick.AddListener(() => TryAgain());
        Time.timeScale = 0f;
    }

    private void TryAgain()
    {
        sceneController.LoadScene(0);
    }
}
