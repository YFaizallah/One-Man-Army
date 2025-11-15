using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public float levelTime = 45f; // 45 seconds
    public int totalZombies = 5;

    [Header("UI")]
    public TMP_Text timerText;
    public GameObject winPanel;
    public GameObject losePanel;

    private float currentTime;
    private int zombiesKilled = 0;
    private bool levelEnded = false;

    void Start()
    {
        currentTime = levelTime;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    void Update()
    {
        if (levelEnded) return;

        // Countdown timer
        currentTime -= Time.deltaTime;
        timerText.text = Mathf.Ceil(currentTime).ToString();

        // Check lose by time
        if (currentTime <= 0)
        {
            LoseLevel();
        }
    }

    // Call this when a zombie dies
    public void ZombieKilled()
    {
        if (levelEnded) return;

        zombiesKilled++;
        if (zombiesKilled >= totalZombies)
        {
            WinLevel();
        }
    }

    // Call this when the player dies
    public void PlayerDied()
    {
        if (levelEnded) return;
        LoseLevel();
    }

    void WinLevel()
    {
        Debug.Log("You won!");
        levelEnded = true;
        winPanel.SetActive(true);
        // Pause game if needed
        Time.timeScale = 0f;
        //gameEndManager.ShowWin();
        //SceneManager.LoadScene("WinLevel");
    }

    void LoseLevel()
    {
        Debug.Log("You lost!");
        levelEnded = true;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
        //gameEndManager.ShowLose();
        //SceneManager.LoadScene("LoseLevel");
    }
}
