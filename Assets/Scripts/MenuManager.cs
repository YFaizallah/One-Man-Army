using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Needed for the Fade Image
using System.Collections; // Needed for IEnumerator

public class MenuManager : MonoBehaviour
{
    [Header("Loading Screen")]
    public GameObject loadingPanel;
    public Slider loadingSlider;

    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject optionsPanel;

    [Header("Fader")]
    public Image fadePanel; // Drag your Black Panel here
    public float fadeDuration = 1.0f; // How long the fade takes

    [Header("Options")]
    public Slider volumeSlider;

    void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // Ensure the fade panel is invisible at start
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.canvasRenderer.SetAlpha(0f);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 1f);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // UPDATED: This now starts the transition instead of loading instantly
    public void PlayGame()
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        // 1. Fade the black panel to Opacity 1 (Solid Black)
        if (fadePanel != null)
        {
            fadePanel.CrossFadeAlpha(1f, fadeDuration, false);
        }

        // 2. Wait for the fade to finish
        yield return new WaitForSeconds(fadeDuration);

        // 3. Load the CutScene (Make sure this name matches your scene exactly!)
        SceneManager.LoadScene("CutScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed!");
        Application.Quit();
    }

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
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("masterVolume", value);
    }

    public void GoToCutscene()
    {
        SceneManager.LoadScene("Cutscene");
    }
}