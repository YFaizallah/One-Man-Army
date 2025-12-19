using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;
using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack.Interface;
using InfimaGames.LowPolyShooterPack;

public class HelicopterCutsceneTrigger : MonoBehaviour
{
    public PlayableDirector director;
    public CinemachineBrain brain;
    public GameObject player;
    public GameObject dialogueUIForCutscene;

    bool triggered;
    private List<Element> uiElements = new List<Element>();
    private CharacterBehaviour playerCharacter;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.gameObject != player) return;

        triggered = true;

        // Get the player character component
        playerCharacter = player.GetComponent<CharacterBehaviour>();
        
        // Disable player movement and input
        if (playerCharacter != null)
        {
            MonoBehaviour characterScript = playerCharacter as MonoBehaviour;
            if (characterScript != null)
            {
                characterScript.enabled = false;
            }
        }

        // Disable all player UI elements
        Element[] elements = FindObjectsOfType<Element>();
        foreach (Element element in elements)
        {
            uiElements.Add(element);
            element.SetUIEnabled(false);
        }

        // Activate dialogue UI for cutscene
        if (dialogueUIForCutscene != null)
        {
            dialogueUIForCutscene.SetActive(true);
        }

        brain.enabled = true;   // 🎬 Cinemachine ON
        director.Play();
    }

    public void EndCutscene()
    {
        brain.enabled = false;  // 🎮 FPS camera returns
        
        // Re-enable player movement and input
        if (playerCharacter != null)
        {
            MonoBehaviour characterScript = playerCharacter as MonoBehaviour;
            if (characterScript != null)
            {
                characterScript.enabled = true;
            }
        }
        
        // Re-enable all player UI elements
        foreach (Element element in uiElements)
        {
            if (element != null)
            {
                element.SetUIEnabled(true);
            }
        }
        uiElements.Clear();
        
        // Deactivate dialogue UI for cutscene
        if (dialogueUIForCutscene != null)
        {
            dialogueUIForCutscene.SetActive(false);
        }
    }
}
