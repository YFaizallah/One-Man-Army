using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Handles the UI portion of dialogues: speaker name, message, and a typewriter effect.
/// </summary>
public class DialogueControllerCutscene : MonoBehaviour
{
    public TextMeshProUGUI nameText;      // Text component for speaker's name
    public TextMeshProUGUI messageText;   // Text component for the dialogue message

    public float typingSpeed = 0.03f;     // Delay between each typed character

    private Coroutine typingCoroutine;    // Used to stop previous typing if a new one begins

    // ---------------------------------------------------------------
    // Sets the speaker name instantly (used by Timeline or scripts)
    // ---------------------------------------------------------------
    public void SetSpeaker(string speaker)
    {
        if (nameText == null)
        {
            Debug.LogWarning("DialogueController has no nameText assigned."); // Warn when UI reference missing
            return;                                                         // Skip update without valid target
        }
        nameText.text = speaker;                                             // Populate the speaker label immediately
    }

    // ---------------------------------------------------------------
    // Starts a typewriter effect for the given message
    //
    // Timeline or scripts should call it
    // ---------------------------------------------------------------
    public void SetMessage(string message)
    {
        if (messageText == null)
        {
            Debug.LogWarning("DialogueController has no messageText assigned."); // Warn if primary text element missing
            return;                                                             // Avoid running typewriter without UI
        }

        // If a previous typing animation is still running, stop it
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);             // Cancel any in-progress typewriter animation

        // Start typing the new message letter by letter
        typingCoroutine = StartCoroutine(TypeMessage(message)); // Launch coroutine for gradual reveal
    }

    // ---------------------------------------------------------------
    // Typewriter coroutine:
    // Reveals the text gradually, one character at a time.
    // ---------------------------------------------------------------
    IEnumerator TypeMessage(string message)
    {
        messageText.text = "";  // Clear text before typing begins

        // Loop through each character in the message
        foreach (char c in message)                        // Iterate over each character to reveal sequentially
        {
            messageText.text += c;               // Append the next character to the label
            yield return new WaitForSeconds(typingSpeed);  // Pause before showing the next character
        }
    }
}
