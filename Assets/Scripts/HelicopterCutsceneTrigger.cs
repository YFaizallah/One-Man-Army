using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack.Interface;
using InfimaGames.LowPolyShooterPack;
using UnityEngine.InputSystem;

public class HelicopterCutsceneTrigger : MonoBehaviour
{
    [Header("Cutscene References")]
    public PlayableDirector director;
    public CinemachineBrain brain;
    public GameObject player;
    public GameObject dialogueUIForCutscene;

    private bool triggered = false;
    private bool cutsceneRunning = false;

    private List<Element> uiElements = new List<Element>();
    private CharacterBehaviour playerCharacter;
    private PlayerInput playerInput;

    // --------------------------------------------------
    // FORCE TIME SCALE WHILE CUTSCENE IS RUNNING
    // --------------------------------------------------
    private void Update()
    {
        if (cutsceneRunning && Time.timeScale != 1f)
        {
            Time.timeScale = 1f;
        }
    }

    // --------------------------------------------------
    // TRIGGER CUTSCENE
    // --------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.gameObject != player) return;

        triggered = true;
        cutsceneRunning = true;

        // 🔑 Ensure Timeline time always runs
        Time.timeScale = 1f;

        // Cache player components
        playerCharacter = player.GetComponent<CharacterBehaviour>();
        playerInput = player.GetComponent<PlayerInput>();

        // Disable Player Input (mouse, keyboard, actions)
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        // Disable player movement logic
        if (playerCharacter != null)
        {
            MonoBehaviour characterScript = playerCharacter as MonoBehaviour;
            if (characterScript != null)
            {
                characterScript.enabled = false;
            }
        }

        // Disable all Infima UI elements (may internally pause time)
        Element[] elements = FindObjectsOfType<Element>();
        foreach (Element element in elements)
        {
            uiElements.Add(element);
            element.SetUIEnabled(false);
        }

        // 🔐 Infima UI may pause time AFTER disabling — force again
        Time.timeScale = 1f;

        // Enable dialogue UI
        if (dialogueUIForCutscene != null)
        {
            dialogueUIForCutscene.SetActive(true);
        }

        // Enable Cinemachine only for cutscene
        if (brain != null)
        {
            brain.enabled = true;
        }

        // Play Timeline
        if (director != null)
        {
            director.Play();
        }
    }

    // --------------------------------------------------
    // END CUTSCENE (CALL FROM TIMELINE SIGNAL)
    // --------------------------------------------------
    public void EndCutscene()
    {
        cutsceneRunning = false;

        // Restore time (safety)
        Time.timeScale = 1f;

        // Disable Cinemachine
        if (brain != null)
        {
            brain.enabled = false;
        }

        // Re-enable Player Input
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        // Re-enable player movement
        if (playerCharacter != null)
        {
            MonoBehaviour characterScript = playerCharacter as MonoBehaviour;
            if (characterScript != null)
            {
                characterScript.enabled = true;
            }
        }

        // Re-enable UI
        foreach (Element element in uiElements)
        {
            if (element != null)
            {
                element.SetUIEnabled(true);
            }
        }
        uiElements.Clear();

        // Hide dialogue UI
        if (dialogueUIForCutscene != null)
        {
            dialogueUIForCutscene.SetActive(false);
        }
    }
}
