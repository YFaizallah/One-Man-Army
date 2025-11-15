using UnityEngine;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;  // Panel or UI container
    private bool isPaused = false;
    private GameEndManager gameEndManager;
    private bool shouldTriggerEscape = false;

    void Start()
    {
        // Ensure pause menu is initially off
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        
        // Find the GameEndManager to check game state
        gameEndManager = FindFirstObjectByType<GameEndManager>();
    }

    void Update()
    {
        // Don't allow pausing if game has ended (win or lose canvas is active)
        if (gameEndManager != null)
        {
            if (gameEndManager.winCanvas.activeSelf || gameEndManager.loseCanvas.activeSelf)
                return;
        }

        // Check for Escape key press to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Process delayed escape trigger (after resume button click)
        if (shouldTriggerEscape)
        {
            shouldTriggerEscape = false;
            TriggerEscapeForOtherScripts();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseMenu) pauseMenu.SetActive(true);
        
        // Unlock cursor when paused
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Update Character script cursor state
        InfimaGames.LowPolyShooterPack.Character character = FindFirstObjectByType<InfimaGames.LowPolyShooterPack.Character>();
        if (character != null)
        {
            character.SetCursorLocked(false);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseMenu) pauseMenu.SetActive(false);
        
        // Lock cursor when resumed
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Update Character script cursor state
        InfimaGames.LowPolyShooterPack.Character character = FindFirstObjectByType<InfimaGames.LowPolyShooterPack.Character>();
        if (character != null)
        {
            character.SetCursorLocked(true);
        }
        
        // Trigger escape event for other systems on next frame
        shouldTriggerEscape = true;
    }
    
    private void TriggerEscapeForOtherScripts()
    {
        // Find and trigger UIMenuController if it exists
        UIMenuController menuController = FindFirstObjectByType<UIMenuController>();
        if (menuController != null)
        {
            menuController.ResumeGame();
        }
    }
}