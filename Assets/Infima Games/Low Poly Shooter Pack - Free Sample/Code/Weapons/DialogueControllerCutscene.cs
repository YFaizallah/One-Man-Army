using UnityEngine;
using TMPro;
using System.Collections;

#region Dialogue Data Structures


[System.Serializable]
public class DialogueScenario
{
    public string scenarioName;
    public DialogueLine[] lines;
}

#endregion

/// <summary>
/// Handles cutscene dialogue UI using Timeline signals.
/// Supports multiple dialogue scenarios driven by story state.
/// </summary>
public class DialogueControllerCutscene : MonoBehaviour
{
    // =========================
    // UI REFERENCES
    // =========================
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI messageText;

    // =========================
    // TYPING SETTINGS
    // =========================
    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    // =========================
    // DIALOGUE SCENARIOS
    // =========================
    [Header("Dialogue Scenarios")]
    public DialogueScenario nobodyScenario;          // talked to nobody
    public DialogueScenario manOnlyScenario;         // talked to man only
    public DialogueScenario womanOrBothScenario;     // talked to woman OR both

    // =========================
    // INTERNAL STATE
    // =========================
    private DialogueLine[] activeLines;
    private int currentIndex = 0;
    private bool dialogueStarted = false;
    private Coroutine typingCoroutine; // To manage typing coroutine


    // =====================================================
    // TIMELINE SIGNAL #1
    // Select which scenario to use at cutscene start
    // =====================================================
    public void SelectHelicopterScenario()
    {
        // Clear UI placeholders
        if (nameText != null) nameText.text = "";
        if (messageText != null) messageText.text = "";

        // 1️⃣ Talked to nobody
        if (!StoryProgress.talkedToMan && !StoryProgress.talkedToWoman)
        {
            activeLines = nobodyScenario.lines;
        }
        // 2️⃣ Talked to man only
        else if (StoryProgress.talkedToMan && !StoryProgress.talkedToWoman)
        {
            activeLines = manOnlyScenario.lines;
        }
        // 3️⃣ Talked to woman OR talked to both
        else
        {
            activeLines = womanOrBothScenario.lines;
        }

        currentIndex = 0;
        dialogueStarted = true;
        PlayCurrentLine();
    }

    // =====================================================
    // TIMELINE SIGNAL #2+
    // Advance to next dialogue line
    // =====================================================
    public void PlayNextDialogue()
    {

        if (!dialogueStarted || activeLines == null)
            return;

        currentIndex++;

        if (currentIndex >= activeLines.Length)
            return;

        PlayCurrentLine();
    }

    // =====================================================
    // INTERNAL: Play current dialogue line
    // =====================================================
    private void PlayCurrentLine()
    {
        DialogueLine line = activeLines[currentIndex];
        SetSpeaker(line.characterName);
        SetMessage(line.text);
    }

    // =====================================================
    // ORIGINAL METHODS (UNCHANGED)
    // =====================================================
    public void SetSpeaker(string speaker)
    {
        if (nameText == null)
        {
            Debug.LogWarning("DialogueControllerCutscene: nameText not assigned.");
            return;
        }

        nameText.text = speaker;
    }

    public void SetMessage(string message)
    {
        if (messageText == null)
        {
            Debug.LogWarning("DialogueControllerCutscene: messageText not assigned.");
            return;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeMessage(message));
    }

    private IEnumerator TypeMessage(string message)
    {
        messageText.text = "";

        foreach (char c in message)
        {
            messageText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
