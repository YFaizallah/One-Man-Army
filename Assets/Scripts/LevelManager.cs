//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections;

//public class LevelManager : MonoBehaviour
//{
//    [Header("Sequence Settings")]
//    public int sequence1Zombies = 10; // Zombies to kill for barrier
//    public GameObject zombiePrefab;
//    public Transform[] spawnPoints;

//    [Header("Barrier")]
//    public GameObject barrier;

//    [Header("UI")]
//    public GameObject objectiveCanvas;
//    public TMP_Text objectiveText;
//    public TMP_Text sequenceText;
//    public TMP_Text zombieCountText;
//    public GameObject scriptPanel;
//    public TMP_Text scriptText;
//    public GameObject losePanel;

//    [Header("Objective Arrow")]
//    public GameObject arrowPrefab;  // Assign UI or 3D arrow prefab
//    public Transform npcTarget;     // MAN (unchanged)

//    // ======= ADDED (no existing variables touched) =======
//    public Transform womanTarget;
//    public Transform helicopterTarget;

//    private int arrowStep = 0;
//    // 0 = man, 1 = woman, 2 = helicopter
//    // =====================================================

//    [HideInInspector] public GameObject spawnedArrow;
//    [HideInInspector] public bool arrowActive = false;
//    private bool arrowSpawned = false;

//    [Header("Story Scripts")]
//    public string sequence1Script = "You saved the person! But more zombies are coming...";

//    private int currentSequence = 1;
//    private int zombiesKilled = 0;
//    private bool gameEnded = false;

//    public static LevelManager instance;

//    void Awake()
//    {
//        if (instance == null)
//            instance = this;
//        else
//            Destroy(gameObject);
//    }

//    void Start()
//    {
//        if (spawnedArrow != null)
//            spawnedArrow.SetActive(false);
//        if (arrowPrefab != null)
//            arrowPrefab.SetActive(false);

//        losePanel.SetActive(false);
//        scriptPanel.SetActive(false);
//        if (objectiveCanvas != null)
//            objectiveCanvas.SetActive(false);

//        UpdateUI();
//        SpawnZombies(sequence1Zombies);
//        StartCoroutine(ShowObjectiveCanvas());
//    }

//    IEnumerator ShowObjectiveCanvas()
//    {
//        yield return new WaitForSeconds(15f);
//        if (objectiveCanvas != null)
//            objectiveCanvas.SetActive(true);

//        UpdateObjectiveText();
//    }

//    void Update()
//    {
//        // ======= MODIFIED (target selection only) =======
//        Transform target =
//            arrowStep == 0 ? npcTarget :
//            arrowStep == 1 ? womanTarget :
//            arrowStep == 2 ? helicopterTarget : null;
//        // ================================================

//        if (arrowActive && arrowPrefab != null && target != null)
//        {
//            GameObject player = GameObject.FindGameObjectWithTag("Player");
//            if (player != null)
//            {
//                Vector3 dirToNPC = target.position - player.transform.position;
//                Vector3 localDir = player.transform.InverseTransformDirection(dirToNPC);
//                float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
//                arrowPrefab.transform.rotation = Quaternion.Euler(0, 0, -angle);
//            }
//        }
//    }

//    public void ZombieKilled()
//    {
//        if (gameEnded) return;

//        zombiesKilled++;
//        UpdateUI();
//        UpdateObjectiveText();

//        if (zombiesKilled >= sequence1Zombies)
//        {
//            RemoveBarrier();

//            if (!arrowSpawned)
//            {
//                arrowSpawned = true;
//                arrowStep = 0; // MAN
//                StartCoroutine(SpawnArrowAfterDelay(30f));
//            }
//        }

//        if (currentSequence == 1 && zombiesKilled >= sequence1Zombies)
//            CompleteSequence1();
//    }

//    void RemoveBarrier()
//    {
//        if (barrier != null)
//            barrier.SetActive(false);

//        if (objectiveText != null)
//            objectiveText.gameObject.SetActive(false);
//    }

//    IEnumerator SpawnArrowAfterDelay(float delay)
//    {
//        yield return new WaitForSeconds(delay);

//        if (arrowPrefab != null)
//        {
//            arrowPrefab.SetActive(true);
//            arrowActive = true;
//            Debug.Log("Arrow is now visible!");
//        }
//    }

//    // ======= EXTENDED ONLY =======
//    public void PlayerTalkedToNPC()
//    {
//        if (arrowPrefab == null) return;

//        arrowPrefab.SetActive(false);
//        arrowActive = false;

//        // MAN ? WOMAN
//        if (arrowStep == 0)
//        {
//            arrowStep = 1;
//            StartCoroutine(SpawnArrowAfterDelay(30f));
//        }
//        // WOMAN ? HELICOPTER
//        else if (arrowStep == 1)
//        {
//            arrowStep = 2;
//            arrowPrefab.SetActive(true);
//            arrowActive = true;
//        }
//    }
//    // ============================

//    void UpdateUI()
//    {
//        if (sequenceText != null)
//            sequenceText.text = $"Sequence {currentSequence}";

//        if (zombieCountText != null)
//            zombieCountText.text = $"Zombies: {zombiesKilled} / {sequence1Zombies}";
//    }

//    void UpdateObjectiveText()
//    {
//        if (objectiveText != null)
//        {
//            int remaining = Mathf.Max(sequence1Zombies - zombiesKilled, 0);
//            objectiveText.text =
//                $"Kill {sequence1Zombies} Zombies to take down the barrier\n{remaining} remaining";
//        }
//    }

//    void CompleteSequence1()
//    {
//        scriptPanel.SetActive(true);
//        scriptText.text = sequence1Script;
//        Time.timeScale = 0f;
//    }

//    public void ContinueToSequence2()
//    {
//        scriptPanel.SetActive(false);
//        Time.timeScale = 1f;
//    }

//    void LoseLevel()
//    {
//        gameEnded = true;
//        losePanel.SetActive(true);
//        Time.timeScale = 0f;
//    }

//    public void PlayerDied()
//    {
//        LoseLevel();
//    }

//    void SpawnZombies(int count)
//    {
//        if (zombiePrefab == null) return;

//        for (int i = 0; i < count; i++)
//        {
//            Vector3 spawnPos = (spawnPoints.Length > 0)
//                ? spawnPoints[Random.Range(0, spawnPoints.Length)].position
//                : new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));

//            GameObject zombie = Instantiate(zombiePrefab, spawnPos, Quaternion.identity);

//            GameObject player = GameObject.FindGameObjectWithTag("Player");
//            if (player != null)
//            {
//                ZombieAI ai = zombie.GetComponent<ZombieAI>();
//                if (ai != null)
//                    ai.player = player.transform;
//            }
//        }
//    }
//}



using UnityEngine;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Sequence Settings")]
    public int sequence1Zombies = 10;
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
    public GameObject arrowPrefab;
    public Transform npcTarget;        // MAN
    public Transform womanTarget;      // DAUGHTER
    public Transform helicopterTarget; // HELICOPTER

    [HideInInspector] public bool arrowActive = false;

    private int zombiesKilled = 0;
    private bool gameEnded = false;
    private bool arrowSpawned = false;

    // ===== OBJECTIVE STATE =====
    private enum ObjectiveState
    {
        KillZombies,
        TalkToMan,
        FindWoman,
        EscortToHelicopter
    }

    private ObjectiveState currentObjective = ObjectiveState.KillZombies;
    // ==========================

    public static LevelManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        arrowPrefab.SetActive(false);
        losePanel.SetActive(false);
        scriptPanel.SetActive(false);
        objectiveCanvas.SetActive(false);

        UpdateUI();
        SpawnZombies(sequence1Zombies);
        StartCoroutine(ShowObjectiveCanvas());
    }

    IEnumerator ShowObjectiveCanvas()
    {
        yield return new WaitForSeconds(15f);
        objectiveCanvas.SetActive(true);
        UpdateObjectiveText();
    }

    void Update()
    {
        if (!arrowActive) return;

        Transform target =
            currentObjective == ObjectiveState.TalkToMan ? npcTarget :
            currentObjective == ObjectiveState.FindWoman ? womanTarget :
            currentObjective == ObjectiveState.EscortToHelicopter ? helicopterTarget :
            null;

        if (target == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;

        Vector3 dir = target.position - player.transform.position;
        Vector3 localDir = player.transform.InverseTransformDirection(dir);
        float angle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;

        arrowPrefab.transform.rotation = Quaternion.Euler(0, 0, -angle);
    }

    // ========================= ZOMBIES =========================

    public void ZombieKilled()
    {
        if (gameEnded) return;

        zombiesKilled++;
        UpdateUI();

        if (zombiesKilled >= sequence1Zombies &&
            currentObjective == ObjectiveState.KillZombies)
        {
            RemoveBarrier();
            currentObjective = ObjectiveState.TalkToMan;
            zombieCountText.gameObject.SetActive(false);
            UpdateObjectiveText();

            if (!arrowSpawned)
            {
                arrowSpawned = true;
                StartCoroutine(SpawnArrowAfterDelay(30f));
            }
        }
    }

    void RemoveBarrier()
    {
        barrier.SetActive(false);
    }

    // ========================= NPC CALLBACK =========================

    public void PlayerTalkedToNPC()
    {
        arrowPrefab.SetActive(false);
        arrowActive = false;

        if (currentObjective == ObjectiveState.TalkToMan)
        {
            currentObjective = ObjectiveState.FindWoman;
            UpdateObjectiveText();
            StartCoroutine(SpawnArrowAfterDelay(30f));
        }
        else if (currentObjective == ObjectiveState.FindWoman)
        {
            currentObjective = ObjectiveState.EscortToHelicopter;
            UpdateObjectiveText();
            arrowPrefab.SetActive(true);
            arrowActive = true;
        }
    }

    IEnumerator SpawnArrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        arrowPrefab.SetActive(true);
        arrowActive = true;
    }

    // ========================= UI =========================

    void UpdateUI()
    {
        sequenceText.text = "";
        zombieCountText.text = $"Zombies: {zombiesKilled} / {sequence1Zombies}";
    }

    void UpdateObjectiveText()
    {
        switch (currentObjective)
        {
            case ObjectiveState.KillZombies:
                objectiveText.text = $"Kill {sequence1Zombies} Zombies to take down the barrier";
                break;

            case ObjectiveState.TalkToMan:
                objectiveText.text = "Find the man and talk to him";
                break;

            case ObjectiveState.FindWoman:
                objectiveText.text = "Find the Vice President's daughter";
                break;

            case ObjectiveState.EscortToHelicopter:
                objectiveText.text = "Escort the Vice President's daughter to the helicopter";
                break;
        }
    }

    // ========================= MISC =========================

    void SpawnZombies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = spawnPoints.Length > 0
                ? spawnPoints[Random.Range(0, spawnPoints.Length)].position
                : new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));

            Instantiate(zombiePrefab, pos, Quaternion.identity);
        }
    }

    public void PlayerDied()
    {
        gameEnded = true;
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
