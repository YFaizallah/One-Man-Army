using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    // Called by the Quit button
    public void QuitGame()
    {
        // In the Unity Editor, Application.Quit() does nothing,
        // so we also stop play mode for testing.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
