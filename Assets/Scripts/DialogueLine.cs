using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string characterName;  // Name of the character speaking
    
    [TextArea(2, 5)]
    public string text;           // The dialogue text
}
