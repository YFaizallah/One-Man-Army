using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple bridge between a Timeline signal and scene loading.
/// Attach to a signal receiver and assign the target scene.
/// </summary>
public class CutsceneSignalHandler : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName = "NextScene"; // Scene to load once the cutscene finishes

    /// <summary>
    /// Invoked by a Timeline signal to advance to the next scene.
    /// </summary>
    public void OnCutsceneEnd()
    {
        SceneManager.LoadScene(nextSceneName);  // Perform the actual scene transition
    }
}
