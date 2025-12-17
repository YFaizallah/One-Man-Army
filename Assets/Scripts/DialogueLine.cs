using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;       // The dialogue text
    public Sprite image;      // Optional image for this line
}
