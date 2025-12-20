//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class GameEndManager : MonoBehaviour
//{
//    [Header("Canvases")]
//    public GameObject winCanvas;   // Drag your Win Canvas here
//    public GameObject loseCanvas;  // Drag your Lose Canvas here

//    void Start()
//    {
//        // Hide both canvases at the start
//        winCanvas.SetActive(false);
//        loseCanvas.SetActive(false);
//    }

//    // Call this when player wins
//    public void ShowWin()
//    {
//        winCanvas.SetActive(true);
//        Time.timeScale = 0f; // pause the game
//    }

//    // Call this when player loses
//    public void ShowLose()
//    {
//        loseCanvas.SetActive(true);
//        Time.timeScale = 0f; // pause the game
//    }

//    // Restart current level
//    public void RestartLevel()
//    {
//        Time.timeScale = 1f; // unpause
//        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//    }

//    // Go to title screen (main menu)
//    public void BackToTitle()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("MainMenu"); // replace with your main menu scene name
//    }

//    // Go back to level select screen
//    public void BackToLevelScreen()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("LevelViewer"); // replace with your level select scene name
//    }

//    public void nextLevel2()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("LevelTwo"); // replace with your level select scene name
//    }
//}













using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using InfimaGames.LowPolyShooterPack;

public class GameEndManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject winCanvas;
    public GameObject loseCanvas;

    [Header("Player")]
    public GameObject player; // The player GameObject (P_LPSP_SP_CH)

    private PlayerInput playerInput;
    private Character characterScript;
    private Movement movementScript;
    private AudioSource[] audioSources;
    private bool gameEnded = false;

    void Start()
    {
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);

        // Cache the PlayerInput, Character, and Movement components
        if (player != null)
        {
            // Try to find PlayerInput on player or children
            playerInput = player.GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                playerInput = player.GetComponentInChildren<PlayerInput>();
            }
            
            // Try to find Character script on player or children
            characterScript = player.GetComponent<Character>();
            if (characterScript == null)
            {
                characterScript = player.GetComponentInChildren<Character>();
            }
            
            // Try to find Movement script on player or children
            movementScript = player.GetComponent<Movement>();
            if (movementScript == null)
            {
                movementScript = player.GetComponentInChildren<Movement>();
            }
            
            // Get all AudioSource components on player and children
            audioSources = player.GetComponentsInChildren<AudioSource>();
            
            Debug.Log($"PlayerInput found: {playerInput != null}, Character script found: {characterScript != null}, Movement script found: {movementScript != null}, AudioSources found: {audioSources.Length}");
        }
    }

    void Update()
    {
        // If either canvas is open and game hasn't ended yet
        if ((winCanvas.activeSelf || loseCanvas.activeSelf) && !gameEnded)
        {
            UnlockCursorAndStopMovement();
        }
    }

    private void UnlockCursorAndStopMovement()
    {
        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable the PlayerInput component to stop all player input
        if (playerInput != null)
        {
            playerInput.enabled = false;
            Debug.Log("PlayerInput component disabled.");
        }
        
        // Disable the Character script
        if (characterScript != null)
        {
            characterScript.enabled = false;
            Debug.Log("Character script disabled.");
        }
        
        // Disable the Movement script to prevent any movement
        if (movementScript != null)
        {
            movementScript.enabled = false;
            Debug.Log("Movement script disabled.");
        }
        
        // Stop and disable all audio sources (footsteps, running sounds, etc.)
        if (audioSources != null && audioSources.Length > 0)
        {
            foreach (var audioSource in audioSources)
            {
                if (audioSource != null)
                {
                    audioSource.Stop();
                    audioSource.enabled = false;
                }
            }
            Debug.Log($"Stopped and disabled {audioSources.Length} AudioSource(s).");
        }
        
        if (playerInput == null && characterScript == null && movementScript == null)
        {
            Debug.LogError("Cannot disable player input - PlayerInput, Character, and Movement components are all null!");
        }

        gameEnded = true;
    }

    public void ShowWin()
    {
        winCanvas.SetActive(true);
        Time.timeScale = 0f;
        UnlockCursorAndStopMovement();
    }

    public void ShowLose()
    {
        loseCanvas.SetActive(true);
        Time.timeScale = 0f;
        UnlockCursorAndStopMovement();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        ResetGameState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToTitle()
    {
        Time.timeScale = 1f;
        ResetGameState();
        SceneManager.LoadScene("MainMenu");
    }

    private void ResetGameState()
    {
        StoryProgress.Reset();
        if (LevelManager.instance != null)
        {
            LevelManager.instance.ResetState();
        }
    }

    public void BackToLevelScreen()
    {
        Time.timeScale = 1f;
        ResetGameState();
        SceneManager.LoadScene("LevelViewer");
    }

    public void NextLevel2()
    {
        Time.timeScale = 1f;
        ResetGameState();
        SceneManager.LoadScene("LevelTwo");
    }
}