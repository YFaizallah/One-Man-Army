using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.Services.Core;
using Unity.Services.Analytics;


public class LevelManager : MonoBehaviour
{
    [Header("Sequence Settings")]
    public int sequence1Zombies = 10; // Zombies to kill for barrier
    public GameObject zombiePrefab;
    public Transform[] spawnPoints;

    [Header("Barrier")]
    public GameObject barrier;

    [Header("UI")]
    public GameObject objectiveCanvas;
    public TMP_Text objectiveText;
    public TMP_Text sequenceText;
    public TMP_Text zombieCountText;
    public GameObject scriptPanel;
    public TMP_Text scriptText;
    public GameObject losePanel;

    [Header("Objective Arrow")]
    public GameObject arrowPrefab;  // Assign UI or 3D arrow prefab
    public Transform npcTarget;     // The NPC to point to

    [HideInInspector] public GameObject spawnedArrow;  // Accessible to NPC scripts
    [HideInInspector] public bool arrowActive = false; // Accessible to NPC scripts
    private bool arrowSpawned = false;

    [Header("Story Scripts")]
    public string sequence1Script = "You saved the person! But more zombies are coming...";

    private int currentSequence = 1;
    private int zombiesKilled = 0;
    private bool gameEnded = false;

    public static LevelManager instance;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            // Uncomment if you want this object to persist across scenes
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
        if (spawnedArrow != null)
            spawnedArrow.SetActive(false);
        if (arrowPrefab != null)
            arrowPrefab.SetActive(false);
        // Hide UI initially
        losePanel.SetActive(false);
        scriptPanel.SetActive(false);
        if (objectiveCanvas != null)
            objectiveCanvas.SetActive(false);

        UpdateUI();
        SpawnZombies(sequence1Zombies);

        // Show objective canvas after 15 seconds
        StartCoroutine(ShowObjectiveCanvas());
    }

    IEnumerator ShowObjectiveCanvas()
    {
        yield return new WaitForSeconds(15f);
        if (objectiveCanvas != null)
            objectiveCanvas.SetActive(true);

        UpdateObjectiveText();
    }

    void Update()
    {
        if (arrowActive && arrowPrefab != null && npcTarget != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {

                //  Compute direction from player to NPC in world space
                Vector3 dirToNPC = npcTarget.position - player.transform.position;

                //  Project direction relative to player forward
                Vector3 localDir = player.transform.InverseTransformDirection(dirToNPC);

                //  Calculate angle in degrees
                float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

                //  Rotate arrow around Z axis
                arrowPrefab.transform.rotation = Quaternion.Euler(0, 0, -angle);
            }
        }
    }


    public void ZombieKilled()
    {
        if (gameEnded) return;

        zombiesKilled++;
        UpdateUI();
        UpdateObjectiveText();

        // Check if barrier objective completed
        if (zombiesKilled >= sequence1Zombies)
        {
            RemoveBarrier();

            // Spawn arrow to NPC after 30 seconds (only once)
            if (!arrowSpawned)
            {
                arrowSpawned = true;
                StartCoroutine(SpawnArrowAfterDelay(30f));
            }
        }

        if (currentSequence == 1 && zombiesKilled >= sequence1Zombies)
            CompleteSequence1();
    }

    void RemoveBarrier()
    {
        if (barrier != null)
            barrier.SetActive(false);

        if (objectiveText != null)
            objectiveText.gameObject.SetActive(false); // only hide the text
    }


    IEnumerator SpawnArrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (arrowPrefab != null && npcTarget != null)
        {
            arrowPrefab.SetActive(true); // Show the arrow
            arrowActive = true;
            Debug.Log("Arrow is now visible!");
        }
    }


    public void PlayerTalkedToNPC()
    {
        // Call this from NPC interaction script
        if (arrowPrefab != null)
        {
            arrowPrefab.SetActive(false);  // Hide it
            arrowActive = false;           // Stop updating rotation
            Debug.Log("Arrow hidden after dialogue.");
        }
    }

    void UpdateUI()
    {
        if (sequenceText != null)
            sequenceText.text = $"Sequence {currentSequence}";

        if (zombieCountText != null)
            zombieCountText.text = $"Zombies: {zombiesKilled} / {sequence1Zombies}";
    }

    void UpdateObjectiveText()
    {
        if (objectiveText != null)
        {
            int remaining = Mathf.Max(sequence1Zombies - zombiesKilled, 0);
            objectiveText.text = $"Kill {sequence1Zombies} Zombies to take down the barrier\n{remaining} remaining";
        }
    }

    void CompleteSequence1()
    {
        scriptPanel.SetActive(true);
        scriptText.text = sequence1Script;
        Time.timeScale = 0f;
    }

    public void ContinueToSequence2()
    {
        scriptPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void LoseLevel()
    {
        gameEnded = true;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void PlayerDied()
    {
        LoseLevel();
    }

    void SpawnZombies(int count)
    {
        if (zombiePrefab == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = (spawnPoints.Length > 0) ?
                spawnPoints[Random.Range(0, spawnPoints.Length)].position :
                new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));

            GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                ZombieAI ai = zombie.GetComponent<ZombieAI>();
                if (ai != null)
                    ai.player = player.transform;
            }
        }
    }
}
