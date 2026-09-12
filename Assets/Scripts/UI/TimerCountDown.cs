using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerCountDown : MonoBehaviour
{
    [Header("Timer Count Down Start")]
    [SerializeField] private GameObject timerScreenStarter;
    [SerializeField] private TextMeshProUGUI timerTextStart;

    [Header("Timer Count Down Final")]
    [SerializeField] private GameObject timerScreenFinal;
    [SerializeField] private TextMeshProUGUI timerTextFinal;


    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private Button restartButton;


    [Header("Controllers")]
    [SerializeField] private SceneController sceneController = new();


    private float timer;
    [SerializeField] private float setTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCountDown();
    }

    // Update is called once per frame
    void Update()
    {
        CountDown();
    }

    private void StartCountDown()
    {
        timer = setTimer; // Reset the timer to 60 seconds
        gameOverScreen.SetActive(false);
        timerScreenFinal.SetActive(false);
        timerScreenStarter.SetActive(true);
    }

    private void CountDown()
    {
        if (timer > 6)
        {
            timer -= Time.deltaTime;
            timerTextStart.text = timer.ToString("0.00");
        }
        else if (timer < 6 && timer > 0)
        {
            if (timerScreenFinal.activeInHierarchy == false)
            {
                timerScreenFinal.SetActive(true);
                timerScreenStarter.SetActive(false);
            }

            timer -= Time.deltaTime;
            timerTextFinal.text = timer.ToString("0");
        }
        else
        {
            timerScreenFinal.SetActive(false);
            gameOverScreen.SetActive(true);
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => sceneController.LoadScene(0));
        }
    }
}



