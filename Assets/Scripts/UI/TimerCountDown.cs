using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerCountDown : MonoBehaviour
{
    [Header("Event Manager")]
    [SerializeField] private EventManager eventManager;


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

    [SerializeField] private bool isExploded = false;


    private float timer;
    [SerializeField] private float setTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("EventManager id: " + eventManager.GetInstanceID(), eventManager);
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
            timerTextStart.text = FormatTime(timer);
                //timer.ToString("0.00");
        }
        else if (timer < 6 && timer > 0)
        {
            if (timerScreenFinal.activeInHierarchy == false)
            {
                timerScreenFinal.SetActive(true);
                timerScreenStarter.SetActive(false);
            }

            timer -= Time.deltaTime;
            timerTextFinal.text = FormatTime(timer);
            //timer.ToString("0");
        }
        else
        {
            if (!isExploded)
            {
                eventManager.BombExploded();
                Debug.Log(eventManager);
                isExploded = true;
            }
            timerScreenFinal.SetActive(false);
            gameOverScreen.SetActive(true);
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => sceneController.LoadScene(0));
        }
    }

    private string FormatTime(float time)
    {
        if (time < 0) time = 0;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int centis = Mathf.FloorToInt((time * 100f) % 100f);

        return $"<size=120%>{minutes:00}<size=110%>:{seconds:00}<size=70%>.{centis:00}";
    }
}



