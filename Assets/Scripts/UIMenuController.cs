using UnityEngine;

public class UIMenuController : MonoBehaviour
{
    [Header("Menu Canvas")]
    public GameObject menuCanvas; // Drag your menu canvas here

    private bool isMenuOpen = false;

    void Start()
    {
        if (menuCanvas != null)
            menuCanvas.SetActive(false);

        LockCursor(true);
    }

    void Update()
    {
        // Toggle menu with Shift+Escape key
        if (Input.GetKeyDown(KeyCode.Escape) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        if (menuCanvas != null)
            menuCanvas.SetActive(isMenuOpen);

        // Unlock cursor when menu is open, lock it when closed
        LockCursor(!isMenuOpen);

        // Optional: Pause/unpause game when menu is open
        Time.timeScale = isMenuOpen ? 0f : 1f;
    }

    private void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    // You can also assign these to buttons
    public void ResumeGame()
    {
        if (isMenuOpen)
            ToggleMenu();
    }

    public void BackToTitle()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
