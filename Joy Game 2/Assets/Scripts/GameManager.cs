using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Reference")]
    public PlayerMovement playerMovement;

    [Header("UI")]
    public Text timerText;
    public GameObject gameOverUI;
    public GameObject winUI;

    private float currentTime;
    private bool isGameOver;

    private void Start()
    {
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement belum di-assign.");
            return;
        }

        currentTime = playerMovement.TimeHad;
        gameOverUI.SetActive(false);
        winUI.SetActive(false);
        isGameOver = false;
    }

    private void Update()
    {
        if (isGameOver)
            return;

        // Jika player menang
        if (playerMovement.menang)
        {
            Win();
            return;
        }

        // Countdown waktu
        currentTime -= Time.deltaTime;
        UpdateTimerUI(currentTime);

        // Time habis
        if (currentTime <= 0f)
        {
            GameOver();
            return;
        }

        // Score habis
        if (playerMovement.Score <= 0)
        {
            GameOver();
            return;
        }
    }

    void UpdateTimerUI(float time)
    {
        time = Mathf.Max(time, 0f);
        int a = Mathf.CeilToInt(time);
        int b = 390 - a;
        int minute = Mathf.FloorToInt(b / 60f);
        int second = Mathf.FloorToInt(b % 60f);
        timerText.text = minute.ToString("00") + ":" + second.ToString("00");
    }

    void GameOver()
    {
        isGameOver = true;
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void Win()
    {
        isGameOver = true;
        winUI.SetActive(true);
        Time.timeScale = 0f;
    }
}
