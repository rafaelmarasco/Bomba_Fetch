using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerCountDown : MonoBehaviour
{
    [Header("Timer Count Down")]
    [SerializeField] private GameObject timerScreen;
    [SerializeField] private TextMeshProUGUI timerText;


    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverScreen;


    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCountDown();
    }

    private void OnEnable()
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
        timer = 60f; // Reset the timer to 60 seconds
        gameOverScreen.SetActive(false);
        timerScreen.SetActive(true);
    }

    private void CountDown()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("0.00");
        }
        else
        {
            gameOverScreen.SetActive(true);
        }
    }
}
