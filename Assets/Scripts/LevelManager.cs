using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
        if (spawnedArrow != null)
            spawnedArrow.SetActive(false);
        //if (arrowPrefab != null)
        //    arrowPrefab.SetActive(false);
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
                // Position above player
                //arrowPrefab.transform.position = player.transform.position + Vector3.up * 2f;
                // Rotate to look at NPC
                arrowPrefab.transform.LookAt(npcTarget.position);
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

    //IEnumerator SpawnArrowAfterDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);

    //    if (arrowPrefab != null && npcTarget != null)
    //    {
    //        //// Instantiate arrow as child of your UI Canvas
    //        //spawnedArrow = Instantiate(arrowPrefab, objectiveCanvas.transform); // Use your main Canvas
    //        //spawnedArrow.transform.localPosition = new Vector3(0, 100, 0); // Adjust as needed
    //        //spawnedArrow.transform.localRotation = Quaternion.identity;

    //        spawnedArrow = Instantiate(arrowPrefab, objectiveCanvas.transform);

    //        // Get RectTransform of the spawned arrow
    //        RectTransform rt = spawnedArrow.GetComponent<RectTransform>();
    //        if (rt != null)
    //        {
    //            rt.anchoredPosition = new Vector2(0, 180);   // your Y=180, X=0
    //            rt.sizeDelta = new Vector2(100, 100);        // width & height
    //            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); // anchors
    //            rt.pivot = new Vector2(0.5f, 0.5f);         // pivot
    //            rt.localRotation = Quaternion.Euler(80, 0, 0); // rotation x=80
    //            rt.localScale = Vector3.one;                // scale x,y,z =1
    //        }
    //        arrowActive = true;
    //        spawnedArrow.SetActive(true); // Make sure it's active
    //        Debug.Log("Arrow spawned and activated!");
    //    }
    //    else
    //    {
    //        Debug.Log("Arrow Not spawned and Not activated!");
    //    }
    //}


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
        if (spawnedArrow != null)
        {
            Destroy(spawnedArrow);
            arrowActive = false;
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
