//using UnityEngine;

//public class NPCManScript : MonoBehaviour
//{
//    [Header("References")]
//    public Transform player;
//    private Animator animator;

//    [Header("Dialogue")]
//    [TextArea(3, 5)]
//    public string[] dialogueLines;

//    [Header("Interaction Settings")]
//    public float detectionRange = 8f;
//    public KeyCode interactKey = KeyCode.E;
//    public float rotationSpeed = 5f;

//    private bool playerInRange = false;
//    private bool dialogueStarted = false;
//    private bool hasTalkedOnce = false; // stops waving after first convo

//    void Start()
//    {
//        animator = GetComponent<Animator>();

//        if (player == null)
//        {
//            GameObject p = GameObject.FindGameObjectWithTag("Player");
//            if (p != null)
//                player = p.transform;
//        }
//    }

//    void Update()
//    {
//        if (player == null) return;

//        float dist = Vector3.Distance(transform.position, player.position);

//        if (dist <= detectionRange)
//        {
//            if (!playerInRange)
//            {
//                playerInRange = true;

//                // Only wave if player hasn't talked yet
//                if (animator != null && !hasTalkedOnce)
//                    animator.SetBool("PlayerNear", true);

//                // Always show Press E
//                if (!dialogueStarted)
//                    DialogueUI.instance.ShowPressE(true);
//            }

//            // Rotate NPC towards player if waving or during dialogue
//            if (!hasTalkedOnce || dialogueStarted)
//                LookAtPlayer();

//            if (Input.GetKeyDown(interactKey) && !dialogueStarted)
//            {
//                StartDialogue();
//            }
//        }
//        else
//        {
//            if (playerInRange)
//            {
//                playerInRange = false;

//                // Stop wave only if player hasn't talked yet
//                if (animator != null && !hasTalkedOnce)
//                    animator.SetBool("PlayerNear", false);

//                DialogueUI.instance.ShowPressE(false);
//                dialogueStarted = false;
//            }
//        }
//    }

//    void StartDialogue()
//    {
//        dialogueStarted = true;
//        DialogueUI.instance.ShowPressE(false);

//        if (animator != null)
//            animator.SetBool("NPCTalked", true); // play talking animation

//        DialogueUI.instance.StartDialogue(dialogueLines);
//    }

//    // Call this from DialogueUI when dialogue finishes
//    public void EndDialogue()
//    {
//        dialogueStarted = false;
//        hasTalkedOnce = true; // stops waving permanently

//        if (animator != null)
//        {
//            animator.SetBool("NPCTalked", false);
//            animator.SetBool("PlayerNear", false); // stop waving permanently
//        }
//    }

//    void LookAtPlayer()
//    {
//        Vector3 direction = (player.position - transform.position).normalized;
//        direction.y = 0;
//        if (direction.magnitude == 0) return;

//        Quaternion lookRotation = Quaternion.LookRotation(direction);
//        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
//    }
//}





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
    public float followDistance = 2f; // Stop following when this close
    public float runThreshold = 4f; // Start running when player is this far
    public float startFollowDistance = 2.5f; // Start following again when player is this far (hysteresis)

    private bool playerInRange = false;
    private bool dialogueStarted = false;
    private bool hasTalkedOnce = false;
    private bool isFollowing = false;
    private bool isCurrentlyIdle = false; // Track if NPC is in idle state
    
    private Vector3 lastPosition;
    private float movementCheckTimer = 0f;
    private const float MOVEMENT_CHECK_INTERVAL = 0.1f;

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

        lastPosition = transform.position;

        // Set initial terrified state - all animations off means terrified plays
        if (animator != null)
        {
            animator.SetBool("hasTalked", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);
        }

        // Freeze NPC initially (terrified state)
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Before dialogue ends - show terrified, allow interaction
        if (!hasTalkedOnce)
        {
            if (dist <= detectionRange)
            {
                if (!playerInRange)
                {
                    playerInRange = true;
                    if (!dialogueStarted)
                        DialogueUI.instance.ShowPressE(true);
                }

                if (Input.GetKeyDown(interactKey) && !dialogueStarted)
                {
                    StartDialogue();
                }
            }
            else
            {
                if (playerInRange)
                {
                    playerInRange = false;
                    DialogueUI.instance.ShowPressE(false);
                    dialogueStarted = false;
                }
            }
        }
        // After dialogue ends - follow the player
        else
        {
            FollowPlayer(dist);
        }
    }

    void StartDialogue()
    {
        dialogueStarted = true;
        DialogueUI.instance.ShowPressE(false);

        if (animator != null)
           // animator.SetBool("isTalked", true);

        DialogueUI.instance.StartDialogue(dialogueLines, gameObject);
    }

    public void EndDialogue()
    {
        dialogueStarted = false;
        hasTalkedOnce = true;
        isFollowing = true;

        Debug.Log("NPC Woman: Ending dialogue, transitioning from terrified to idle");

        if (animator != null)
        {
            // Explicitly end terrified state
            animator.SetBool("hasTalked", true);
            
            // Force idle animation to start
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }

        // Unfreeze the NPC so she can move and rotate
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        // Initialize position tracking
        lastPosition = transform.position;
        movementCheckTimer = 0f;
    }

    void FollowPlayer(float dist)
    {
        if (!isFollowing) return;

        // Check if player is moving by monitoring their position change
        Vector3 playerCurrentPos = player.position;
        float playerMovedDistance = Vector3.Distance(playerCurrentPos, lastPosition);
        bool playerIsMoving = playerMovedDistance > 0.05f * Time.deltaTime * 60f; // Adjusted for framerate
        lastPosition = playerCurrentPos;

        // Use hysteresis to prevent jittering between idle and walking
        float distanceThreshold = isCurrentlyIdle ? startFollowDistance : followDistance;

        // If player is close enough AND not moving, stop and idle
        if (dist <= distanceThreshold && !playerIsMoving)
        {
            if (!isCurrentlyIdle)
            {
                isCurrentlyIdle = true;
                SetIdleAnimation();
            }
            return; // Don't move at all when idle
        }

        // If player is close but moving, still follow them
        if (dist <= followDistance && playerIsMoving)
        {
            isCurrentlyIdle = false;
            SetWalkingAnimation();
            
            // Look at player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            
            if (direction.magnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
            
            // Move slowly when close
            transform.position = Vector3.MoveTowards(transform.position, player.position, followSpeed * 0.7f * Time.deltaTime);
            return;
        }

        // Player is far away, need to follow
        isCurrentlyIdle = false;
        
        // Determine if NPC should walk or run based on player distance
        bool shouldRun = dist > runThreshold;
        float currentSpeed = shouldRun ? runSpeed : followSpeed;

        // Set animation BEFORE moving
        if (shouldRun)
            SetRunningAnimation();
        else
            SetWalkingAnimation();

        // Look at player (only Y-axis rotation)
        Vector3 direction2 = (player.position - transform.position).normalized;
        direction2.y = 0;
        
        if (direction2.magnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction2);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Move towards player
        transform.position = Vector3.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);
    }

    void SetWalkingAnimation()
    {
        if (animator == null) return;
        
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
        animator.SetBool("isIdle", false);
    }

    void SetRunningAnimation()
    {
        if (animator == null) return;
        
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", true);
        animator.SetBool("isIdle", false);
    }

    void SetIdleAnimation()
    {
        if (animator == null) return;
        
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isIdle", true);
        
        // Stop any rotation when idle
        if (rb != null)
        {
            rb.angularVelocity = Vector3.zero;
        }
    }
}
