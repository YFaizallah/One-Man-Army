using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class LevelManager : MonoBehaviour
{
    [Header("Sequence Settings")]
    public int sequence1Zombies = 10; // Zombies to kill in sequence 1
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;

    [Header("UI")]
    public TMP_Text sequenceText; // Shows current sequence info
    public TMP_Text zombieCountText; // Shows zombies killed
    public GameObject scriptPanel; // Panel to show story script
    public TMP_Text scriptText; // Text for the story
    public GameObject losePanel;

    [Header("Story Scripts")]
    public string sequence1Script = "You saved the person! But more zombies are coming...";

    private int currentSequence = 1;
    private int zombiesKilled = 0;
    private bool gameEnded = false;

    void Start()
    {
        losePanel.SetActive(false);
        scriptPanel.SetActive(false);
        
        UpdateUI();
        SpawnZombies(sequence1Zombies);
    }

    void Update()
    {
        // No timer logic needed
    }

    // Call this when a zombie dies
    public void ZombieKilled()
    {
        if (gameEnded) return;

        zombiesKilled++;
        UpdateUI();

        // Check if sequence 1 is complete
        if (currentSequence == 1 && zombiesKilled >= sequence1Zombies)
        {
            CompleteSequence1();
        }
    }

    // Call this when the player dies
    public void PlayerDied()
    {
        if (gameEnded) return;
        LoseLevel();
    }

    void CompleteSequence1()
    {
        Debug.Log("Sequence 1 Complete!");
        
        // Show the story script
        scriptPanel.SetActive(true);
        scriptText.text = sequence1Script;
        Time.timeScale = 0f; // Pause game while showing script
        
        // You can add a button to continue or use Invoke to auto-continue
        // For now, you'll need to add a button that calls ContinueToSequence2()
    }

    public void ContinueToSequence2()
    {
        // Hide script panel
        scriptPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game
        
        // TODO: Start sequence 2 (you can add this later)
        Debug.Log("Ready for Sequence 2!");
    }

    void UpdateUI()
    {
        if (sequenceText != null)
            sequenceText.text = $"Sequence {currentSequence}";
        
        if (zombieCountText != null)
        {
            int remaining = 0;
            if (currentSequence == 1)
                remaining = sequence1Zombies - zombiesKilled;
            
            zombieCountText.text = $"Zombies: {zombiesKilled} / {(currentSequence == 1 ? sequence1Zombies : 0)}";
        }
    }

    void LoseLevel()
    {
        Debug.Log("You lost!");
        gameEnded = true;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void SpawnZombies(int count)
    {
        if (zombiePrefab == null)
        {
            Debug.LogError("Zombie prefab is not assigned!");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition;

            // If spawn points are defined, use them
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                spawnPosition = spawnPoints[randomIndex].position;
            }
            else
            {
                // Random spawn around origin (customize as needed)
                float randomX = Random.Range(-10f, 10f);
                float randomZ = Random.Range(-10f, 10f);
                spawnPosition = new Vector3(randomX, 6, randomZ);
            }

            // Spawn the zombie
            GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
            
            // Optionally: assign player reference to zombie
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                ZombieAI zombieAI = zombie.GetComponent<ZombieAI>();
                if (zombieAI != null)
                {
                    zombieAI.player = player.transform;
                }
            }
        }
    }
}
