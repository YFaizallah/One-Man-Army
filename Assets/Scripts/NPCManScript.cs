using UnityEngine;

public class NPCManScript : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Animator animator;

    [Header("Dialogue")]
    public DialogueLine[] dialogueLines; // <-- now you can assign both text & image
    public LevelManager levelManager;

    [Header("Interaction Settings")]
    public float detectionRange = 8f;
    public KeyCode interactKey = KeyCode.E;
    public float rotationSpeed = 5f;

    private bool playerInRange = false;
    private bool dialogueStarted = false;
    private bool hasTalkedOnce = false;

    




    void Start()
    {
        animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                if (animator != null && !hasTalkedOnce)
                    animator.SetBool("PlayerNear", true);
                if (!dialogueStarted)
                    DialogueUI.instance.ShowPressE(true);
            }

            if (!hasTalkedOnce || dialogueStarted)
                LookAtPlayer();

            if (Input.GetKeyDown(interactKey) && !dialogueStarted)
                StartDialogue();
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                if (animator != null && !hasTalkedOnce)
                    animator.SetBool("PlayerNear", false);
                DialogueUI.instance.ShowPressE(false);
                dialogueStarted = false;
            }
        }
    }

    void StartDialogue()
    {
        dialogueStarted = true;
        DialogueUI.instance.ShowPressE(false);

        if (animator != null)
            animator.SetBool("NPCTalked", true);
        if (LevelManager.instance != null && LevelManager.instance.arrowPrefab != null)
        {
            LevelManager.instance.arrowPrefab.SetActive(false);
            LevelManager.instance.arrowActive = false;
            Debug.Log("Arrow hidden after dialogue.");
        }
        // Pass dialogue lines with optional images
        //DialogueUI.instance.StartDialogue(dialogueLines);
        DialogueUI.instance.StartDialogue(dialogueLines, gameObject);

    }

    public void EndDialogue()
    {
        dialogueStarted = false;
        hasTalkedOnce = true;

        if (animator != null)
        {
            animator.SetBool("NPCTalked", false);
            animator.SetBool("PlayerNear", false);
        }

        if (LevelManager.instance != null)
        {
            Debug.Log("test1");
            LevelManager.instance.PlayerTalkedToNPC();
        }
    }

    void LookAtPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction.magnitude == 0) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

}
