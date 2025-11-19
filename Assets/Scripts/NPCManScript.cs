using UnityEngine;
using HeneGames.DialogueSystem;

public class NPCManScript : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Animator animator;
    private DialogueManager dialogueManager;
    private DialogueTrigger dialogueTrigger;

    [Header("Interaction Settings")]
    public float detectionRange = 10f; // Distance to detect player
    public KeyCode interactKey = KeyCode.E; // Key to interact with NPC
    
    private bool playerInRange = false;
    private bool dialogueStarted = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        dialogueManager = GetComponent<DialogueManager>();
        dialogueTrigger = GetComponent<DialogueTrigger>();
        
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check if player is in range
        if (distanceToPlayer <= detectionRange)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                if (animator != null)
                    animator.SetBool("PlayerNear", true);
                
                // Show interaction UI
                if (!dialogueStarted)
                    DialogueUI.instance.ShowInteractionUI(true);
            }

            // Check for interaction to start dialogue
            if (!dialogueStarted && Input.GetKeyDown(interactKey))
            {
                StartDialogue();
            }
            // Continue dialogue with E key
            else if (dialogueStarted && Input.GetKeyDown(interactKey))
            {
                DialogueUI.instance.NextSentenceSoft();
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                if (animator != null)
                    animator.SetBool("PlayerNear", false);
                
                // Hide interaction UI
                DialogueUI.instance.ShowInteractionUI(false);
                
                // Stop dialogue if player walks away
                if (dialogueStarted && dialogueManager != null)
                {
                    dialogueManager.StopDialogue();
                    dialogueStarted = false;
                }
            }
        }
    }

    void StartDialogue()
    {
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager component not found on NPC!");
            return;
        }

        dialogueStarted = true;
        
        // Set animation parameter
        if (animator != null)
            animator.SetBool("NPCTalked", true);
        
        // Trigger dialogue events
        if (dialogueTrigger != null)
            dialogueTrigger.startDialogueEvent.Invoke();
        
        // Start the dialogue through the UI
        DialogueUI.instance.StartDialogue(dialogueManager);
        
        // Hide interaction UI when dialogue starts
        DialogueUI.instance.ShowInteractionUI(false);
    }

    // Optional: visualize detection range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}