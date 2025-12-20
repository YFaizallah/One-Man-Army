using UnityEngine;

public class NPCWomanScript : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Animator animator;
    private Rigidbody rb;

    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;

    [Header("Interaction Settings")]
    public float detectionRange = 8f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Follow Settings")]
    public float followSpeed = 3f;
    public float runSpeed = 5f;
    public float followDistance = 2f;
    public float runThreshold = 4f;
    public float startFollowDistance = 2.5f;

    private bool playerInRange = false;
    private bool dialogueStarted = false;
    private bool hasTalkedOnce = false;
    private bool isFollowing = false;
    private bool isCurrentlyIdle = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Before talking
        if (!hasTalkedOnce)
        {
            if (dist <= detectionRange)
            {
                if (!playerInRange)
                {
                    playerInRange = true;
                    DialogueUI.instance.ShowPressE(true);
                }

                if (Input.GetKeyDown(interactKey) && !dialogueStarted)
                    StartDialogue();
            }
            else
            {
                if (playerInRange)
                {
                    playerInRange = false;
                    DialogueUI.instance.ShowPressE(false);
                }
            }
        }
        else
        {
            FollowPlayer(dist);
        }
    }

    void StartDialogue()
    {
        dialogueStarted = true;
        DialogueUI.instance.ShowPressE(false);

        // Pass THIS NPC
        DialogueUI.instance.StartDialogue(dialogueLines, gameObject);
    }

    // CALLED BY DialogueUI
    public void EndDialogue()
    {
        dialogueStarted = false;
        hasTalkedOnce = true;
        isFollowing = true;

        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        StoryProgress.talkedToWoman = true; // Update story progress

        if (animator != null)
        {
            animator.SetBool("hasTalked", true);
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }

        if (LevelManager.instance != null)
            LevelManager.instance.PlayerTalkedToNPC();
    }

    void FollowPlayer(float dist)
    {
        if (!isFollowing) return;

        if (dist <= followDistance)
        {
            SetIdle();
            return;
        }

        bool run = dist > runThreshold;
        float speed = run ? runSpeed : followSpeed;

        if (run) SetRunning();
        else SetWalking();

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * 5f);

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            speed * Time.deltaTime);
    }

    void SetWalking()
    {
        if (!animator) return;
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        animator.SetBool("isIdle", false);
    }

    void SetRunning()
    {
        if (!animator) return;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        animator.SetBool("isIdle", false);
    }

    void SetIdle()
    {
        if (!animator) return;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isIdle", true);
    }
}
