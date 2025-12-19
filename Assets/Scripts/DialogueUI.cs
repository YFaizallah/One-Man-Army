//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class DialogueUI : MonoBehaviour
//{
//    public static DialogueUI instance;

//    [Header("UI")]
//    public GameObject dialoguePanel;
//    public TextMeshProUGUI dialogueText;
//    public GameObject pressEIndicator; // Shows "Press E to talk"
//    public GameObject playerUICanvas; // assign your PlayerUI Canvas in the Inspector


//    private string[] sentences;
//    private int index;
//    private bool activeDialogue = false;

//    void Awake()
//    {
//        instance = this;
//        dialoguePanel.SetActive(false);
//        dialogueText.gameObject.SetActive(false);
//        pressEIndicator.SetActive(false);
//    }

//    void Update()
//    {
//        if (activeDialogue && Input.GetKeyDown(KeyCode.E))
//        {
//            NextSentence();
//        }
//    }

//    public void ShowPressE(bool show)
//    {
//        pressEIndicator.SetActive(show && !activeDialogue);
//    }


//    public void StartDialogue(DialogueLine[] lines)
//    {
//        sentences = lines;
//        index = 0;
//        activeDialogue = true;

//        dialoguePanel.SetActive(true);
//        if (sentences.Length > 0)
//        {
//            dialogueText.text = sentences[index].text;
//            dialogueImage.sprite = sentences[index].image;
//            dialogueImage.gameObject.SetActive(sentences[index].image != null);
//        }

//        if (playerUICanvas != null)
//            playerUICanvas.SetActive(false);

//        FreezePlayer(true);
//        FreezeZombies(true);
//    }


//    //public void StartDialogue(string[] lines)
//    //{
//    //    sentences = lines;
//    //    index = 0;
//    //    activeDialogue = true;

//    //    dialoguePanel.SetActive(true);
//    //    dialogueText.gameObject.SetActive(true);
//    //    dialogueText.text = sentences[index];
//    //    if (playerUICanvas != null)
//    //        playerUICanvas.SetActive(false);
//    //    // ------------------ OPTION 1: Player idle + freeze ------------------
//    //    SetPlayerIdleAndFreeze();
//    //    FreezeZombies(true);
//    //}

//    public void NextSentence()
//    {
//        index++;

//        if (index >= sentences.Length)
//        {
//            EndDialogue();
//            return;
//        }

//        dialogueText.text = sentences[index];
//    }

//    public void EndDialogue()
//    {
//        activeDialogue = false;
//        dialoguePanel.SetActive(false);
//        dialogueText.gameObject.SetActive(false);
//        pressEIndicator.SetActive(false);

//        // Unfreeze player and zombies
//        if (playerUICanvas != null)
//            playerUICanvas.SetActive(true);
//        FreezePlayer(false);
//        FreezeZombies(false);
//    }

//    // ------------------ FREEZE PLAYER & IDLE ------------------
//    void SetPlayerIdleAndFreeze()
//    {
//        var player = GameObject.FindGameObjectWithTag("Player");
//        if (player == null) return;

//        // Disable input
//        var playerInput = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
//        if (playerInput != null)
//            playerInput.enabled = false;

//        // Disable movement and character scripts
//        var movement = player.GetComponent<InfimaGames.LowPolyShooterPack.Movement>();
//        if (movement != null)
//            movement.enabled = false;

//        var character = player.GetComponent<InfimaGames.LowPolyShooterPack.Character>();
//        if (character != null)
//            character.enabled = false;

//        // Force idle animation
//        var animator = player.GetComponent<Animator>();
//        if (animator != null)
//        {
//            animator.SetFloat("Velocity", 0f);   // stops running animation
//            animator.SetBool("IsRunning", false);
//            animator.SetBool("IsMoving", false);
//        }
//    }

//    void FreezePlayer(bool freeze)
//    {
//        var player = GameObject.FindGameObjectWithTag("Player");
//        if (player == null) return;

//        var movement = player.GetComponent<InfimaGames.LowPolyShooterPack.Movement>();
//        if (movement != null)
//            movement.enabled = !freeze;

//        var character = player.GetComponent<InfimaGames.LowPolyShooterPack.Character>();
//        if (character != null)
//            character.enabled = !freeze;

//        var playerInput = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
//        if (playerInput != null)
//            playerInput.enabled = !freeze;
//    }

//    void FreezeZombies(bool freeze)
//    {
//        var zombies = GameObject.FindGameObjectsWithTag("Zombie");

//        foreach (var z in zombies)
//        {
//            var ai = z.GetComponent<ZombieAI>();
//            if (ai != null)
//                ai.enabled = !freeze;

//            var anim = z.GetComponent<Animator>();
//            if (anim != null)
//                anim.speed = freeze ? 0 : 1;
//        }
//    }

//    public bool IsActive() => activeDialogue;
//}



using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI instance;

    [Header("UI - Dialogue Assets")]
    public GameObject dialoguePanel;          // "Dialogue assets" parent object
    public GameObject background;             // Background panel
    public TextMeshProUGUI nameText;          // Name text
    public TextMeshProUGUI messageText;       // Message text
    public GameObject continueButton;         // Continue button/indicator
    
    [Header("UI - Other")]
    public GameObject interactionPopup;       // InteractionPopup - "Press E to talk"
    public GameObject playerUICanvas;         // Assign your Player UI Canvas here

    private DialogueLine[] sentences;
    private int index;
    private bool activeDialogue = false;
    private bool canAdvance = false; // Prevents advancing on the same frame dialogue starts
    private GameObject currentNPC; // Track which NPC started the dialogue

    void Awake()
    {
        instance = this;

        dialoguePanel.SetActive(false);
        
        if (interactionPopup != null)
            interactionPopup.SetActive(false);
    }

    void Update()
    {
        if (activeDialogue && canAdvance && Input.GetKeyDown(KeyCode.E))
        {
            NextSentence();
        }
        
        // Allow advancing after the first frame
        if (activeDialogue && !canAdvance)
        {
            canAdvance = true;
        }
    }

    public void ShowPressE(bool show)
    {
        if (interactionPopup != null)
            interactionPopup.SetActive(show && !activeDialogue);
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        StartDialogue(lines, null);
    }

    public void StartDialogue(DialogueLine[] lines, GameObject npc)
    {
        if (lines == null || lines.Length == 0) return;

        sentences = lines;
        currentNPC = npc;
        index = 0; // Start at index 0
        activeDialogue = true;
        canAdvance = false; // Prevents advancing on the same frame

        dialoguePanel.SetActive(true);
        
        if (interactionPopup != null)
            interactionPopup.SetActive(false);

        if (playerUICanvas != null)
            playerUICanvas.SetActive(false);

        SetPlayerIdleAndFreeze();
        FreezeZombies(true);
        
        // Show the first sentence immediately
        ShowCurrentSentence();
    }

    public void NextSentence()
    {
        index++;

        if (index >= sentences.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentSentence();
    }

    private void ShowCurrentSentence()
    {
        // Update the name text (character speaking)
        if (nameText != null && !string.IsNullOrEmpty(sentences[index].characterName))
        {
            nameText.text = sentences[index].characterName;
            nameText.gameObject.SetActive(true);
        }
        else if (nameText != null)
        {
            nameText.gameObject.SetActive(false);
        }

        // Update the message text (dialogue content)
        if (messageText != null)
        {
            messageText.text = sentences[index].text;
        }
        
        // Show continue indicator
        if (continueButton != null)
        {
            continueButton.SetActive(true);
        }
    }

    public void EndDialogue()
    {
        activeDialogue = false;
        canAdvance = false; // Reset flag
        dialoguePanel.SetActive(false);

        if (playerUICanvas != null)
            playerUICanvas.SetActive(true);

        UnfreezePlayer();
        FreezeZombies(false);

        // Notify the NPC that dialogue has ended
        if (currentNPC != null)
        {
            currentNPC.SendMessage("EndDialogue", SendMessageOptions.DontRequireReceiver);
            currentNPC = null;
        }
    }

    // ------------------ FREEZE PLAYER & IDLE ------------------
    private void SetPlayerIdleAndFreeze()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Disable input
        var playerInput = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;

        // Disable movement and character scripts
        var movement = player.GetComponent<InfimaGames.LowPolyShooterPack.Movement>();
        if (movement != null)
            movement.enabled = false;

        var character = player.GetComponent<InfimaGames.LowPolyShooterPack.Character>();
        if (character != null)
            character.enabled = false;

        // Force idle animation
        var animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Velocity", 0f);   // stops running animation
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsMoving", false);
        }
    }

    private void UnfreezePlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var movement = player.GetComponent<InfimaGames.LowPolyShooterPack.Movement>();
        if (movement != null)
            movement.enabled = true;

        var character = player.GetComponent<InfimaGames.LowPolyShooterPack.Character>();
        if (character != null)
            character.enabled = true;

        var playerInput = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = true;
    }

    private void FreezeZombies(bool freeze)
    {
        var zombies = GameObject.FindGameObjectsWithTag("Zombie");

        foreach (var z in zombies)
        {
            var ai = z.GetComponent<ZombieAI>();
            if (ai != null)
                ai.enabled = !freeze;

            var anim = z.GetComponent<Animator>();
            if (anim != null)
                anim.speed = freeze ? 0 : 1;
        }
    }

    public bool IsActive() => activeDialogue;
}





//public void StartDialogue(DialogueLine[] lines)
//{
//    sentences = lines;
//    index = 0;
//    activeDialogue = true;

//    dialoguePanel.SetActive(true);
//    if (sentences.Length > 0)
//    {
//        dialogueText.text = sentences[index].text;
//        dialogueImage.sprite = sentences[index].image;
//        dialogueImage.gameObject.SetActive(sentences[index].image != null);
//    }

//    if (playerUICanvas != null)
//        playerUICanvas.SetActive(false);

//    FreezePlayer(true);
//    FreezeZombies(true);
//}
