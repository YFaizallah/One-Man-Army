using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;         // for slider (legacy UI)
using TMPro;                  // only if using TextMeshPro

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject optionsPanel;

    [Header("Options")]
    public Slider volumeSlider;       // UI Slider
    // public TMP_Text versionText;   // optional TMP usage

    void Start()
    {
        // Ensure main menu shows, options hidden
        if (menuPanel != null) menuPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // initialize slider from saved prefs
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 1f);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // Button wired to Play
    public void PlayGame()
    {
        // Replace "GameScene" with the exact name of your scene
        SceneManager.LoadScene("LevelOne");
    }

    // Button wired to Quit
    //public void QuitGame()
    //{
    //    #if UNITY_EDITOR
    //    UnityEditor.EditorApplication.isPlaying = false;
    //    #else
    //    Application.Quit();
    //    #endif
    //}
    public void QuitGame()
    {
        Debug.Log("Quit button pressed!"); // for testing in the Editor
        Application.Quit();
    }

    // Options
    public void OpenOptions()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(true);
    }

    public void SetVolume(float value)
    {
        // Set Unity master volume (simple approach)
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("masterVolume", value);
    }
}
