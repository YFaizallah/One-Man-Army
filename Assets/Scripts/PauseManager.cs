using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Pause UI")]
    [SerializeField] private GameObject pauseMenu;  // Panel or UI container

    [Header("UI To Hide While Paused")]
    [Tooltip("Drag in: GameWinCanvas, GameLoseCanvas, Objective1UI, PlayerUi (and anything else you want hidden).")]
    [SerializeField] private List<GameObject> uiToHide = new List<GameObject>();

    private bool isPaused = false;

    // Cache original active states so we restore exactly as it was
    private readonly Dictionary<GameObject, bool> cachedActiveStates = new Dictionary<GameObject, bool>();

    private GameEndManager gameEndManager;
    private bool shouldTriggerEscape = false;

    void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        gameEndManager = FindFirstObjectByType<GameEndManager>();
    }

    void Update()
    {
        // Don't allow pausing if game has ended (win or lose canvas is active)
        if (gameEndManager != null)
        {
            if (gameEndManager.winCanvas != null && gameEndManager.winCanvas.activeSelf) return;
            if (gameEndManager.loseCanvas != null && gameEndManager.loseCanvas.activeSelf) return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        if (shouldTriggerEscape)
        {
            shouldTriggerEscape = false;
            TriggerEscapeForOtherScripts();
        }
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        // Cache UI state ONCE when pausing
        cachedActiveStates.Clear();
        foreach (var go in uiToHide)
        {
            if (go == null) continue;
            cachedActiveStates[go] = go.activeSelf;
            go.SetActive(false);
        }

        Time.timeScale = 0f;
        isPaused = true;

        if (pauseMenu) pauseMenu.SetActive(true);

        // Unlock cursor when paused
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Update Character script cursor state
        var character = FindFirstObjectByType<InfimaGames.LowPolyShooterPack.Character>();
        if (character != null)
            character.SetCursorLocked(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenu) pauseMenu.SetActive(false);

        // Restore UI to EXACT previous states
        foreach (var kvp in cachedActiveStates)
        {
            if (kvp.Key == null) continue;
            kvp.Key.SetActive(kvp.Value);
        }
        cachedActiveStates.Clear();

        // Lock cursor when resumed
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var character = FindFirstObjectByType<InfimaGames.LowPolyShooterPack.Character>();
        if (character != null)
            character.SetCursorLocked(true);

        // Trigger escape event for other systems on next frame
        shouldTriggerEscape = true;
    }

    private void TriggerEscapeForOtherScripts()
    {
        UIMenuController menuController = FindFirstObjectByType<UIMenuController>();
        if (menuController != null)
            menuController.ResumeGame();
    }
}
