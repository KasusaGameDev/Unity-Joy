using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Reference")]
    public PlayerMovement playerMovement;

    [Header("UI")]
    public Text timerText;
    public Text scoreText;
    public GameObject gameOverUI;
    public GameObject winUI;
    public Image signImage;

    [Header("Warning Effect")]
    public float warningTime = 10f;     // a detik terakhir
    public float blinkSpeed = 0.3f;     // kecepatan kedip

    [Header("Minus Score Effect")]
    public Text minusScoreText;
    public float minusFadeDuration = 1f;

    private Coroutine minusCoroutine;
    private float currentTime;
    private bool isGameOver;
    private bool isWarningActive;
    private Coroutine warningCoroutine;

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
        signImage.gameObject.SetActive(false);

        isGameOver = false;
        isWarningActive = false;
    }

    private void Update()
    {
        if (isGameOver)
            return;

        if (playerMovement.menang)
        {
            Win();
            return;
        }

        currentTime -= Time.deltaTime;
        UpdateTimerUI(currentTime);

        // Aktifkan warning saat a detik terakhir
        if (currentTime <= warningTime && !isWarningActive)
        {
            isWarningActive = true;
            warningCoroutine = StartCoroutine(WarningEffect());
        }

        if (currentTime <= 0f || playerMovement.Score <= 0)
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
        scoreText.text = "Poin Kerapihan : " + playerMovement.Score;
    }

    IEnumerator WarningEffect()
    {
        while (!isGameOver)
        {
            // Kuning
            timerText.color = Color.yellow;
            signImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(blinkSpeed);

            // Putih
            timerText.color = Color.white;
            signImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    void GameOver()
    {
        isGameOver = true;

        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        signImage.gameObject.SetActive(false);
        timerText.color = Color.white;

        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void Win()
    {
        isGameOver = true;

        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        signImage.gameObject.SetActive(false);
        timerText.color = Color.white;

        winUI.SetActive(true);
        Time.timeScale = 0f;
    }
    private void OnEnable()
{
    PlayerMovement.OnScoreReduced += ShowMinusScore;
}

private void OnDisable()
{
    PlayerMovement.OnScoreReduced -= ShowMinusScore;
}
public void ShowMinusScore(int amount)
{
    if (minusCoroutine != null)
        StopCoroutine(minusCoroutine);

    minusCoroutine = StartCoroutine(MinusScoreEffect(amount));
}
IEnumerator MinusScoreEffect(int amount)
{
    minusScoreText.gameObject.SetActive(true);
    minusScoreText.text = "-" + amount.ToString();

    Color c = minusScoreText.color;
    c.a = 1f;
    minusScoreText.color = c;

    float t = 0f;

    while (t < minusFadeDuration)
    {
        t += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, t / minusFadeDuration);
        minusScoreText.color = new Color(c.r, c.g, c.b, alpha);
        yield return null;
    }

    minusScoreText.gameObject.SetActive(false);
    minusCoroutine = null;
}

}
