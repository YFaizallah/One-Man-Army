using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using HeneGames.DialogueSystem;

public class LevelLoader : MonoBehaviour
{
    [Header("Timing")]
    public float cutsceneDuration = 15f;
    public float fadeDuration = 1.0f;
    public string nextSceneName = "LevelOne";

    [Header("References")]
    public Image blackCurtain;

    void Start()
    {
        StartCoroutine(EndCutsceneSequence());
    }

    IEnumerator EndCutsceneSequence()
    {
        yield return new WaitForSeconds(cutsceneDuration);

        // --- NEW FIX: SEARCH AND DESTROY IMMEDIATE ---
        // Find ALL Dialogue UIs in the scene (in case there are duplicates)
        DialogueUI[] allUIs = FindObjectsOfType<DialogueUI>();

        foreach (DialogueUI ui in allUIs)
        {
            // DestroyImmediate forces it to vanish INSTANTLY before the scene loads
            DestroyImmediate(ui.gameObject);
        }
        // ---------------------------------------------

        if (blackCurtain != null)
        {
            blackCurtain.gameObject.SetActive(true);
            blackCurtain.CrossFadeAlpha(1f, fadeDuration, false);
        }

        yield return new WaitForSeconds(fadeDuration);

        SceneManager.LoadScene(nextSceneName);
    }
}