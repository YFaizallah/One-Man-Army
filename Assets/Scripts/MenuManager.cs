using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Loading Screen")]
    public GameObject loadingPanel;
    public Slider loadingSlider;

    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject optionsPanel;

    [Header("Options")]
    public Slider volumeSlider;

    void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 1f);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // Play ? Level One (WITH loading screen)
    public void redirectLevelOne()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevelOneAsync());
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void redirectMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
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

    private System.Collections.IEnumerator LoadLevelOneAsync()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        if (loadingSlider != null)
            loadingSlider.value = 0f;

        AsyncOperation op = SceneManager.LoadSceneAsync("LevelOne");
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            if (loadingSlider != null)
                loadingSlider.value = progress;

            if (op.progress >= 0.9f)
            {
                yield return new WaitForSecondsRealtime(0.2f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
