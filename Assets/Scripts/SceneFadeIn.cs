using UnityEngine;
using UnityEngine.UI;

public class SceneFadeIn : MonoBehaviour
{
    [Header("Settings")]
    public Image curtain; // Drag the OpeningCurtain here
    public float fadeDuration = 1.5f; // How long to lighten up

    void Start()
    {
        if (curtain != null)
        {
            // 1. Force it to be completely solid black right at the start
            curtain.canvasRenderer.SetAlpha(1.0f);

            // 2. Slowly fade the alpha to 0 (Invisible)
            curtain.CrossFadeAlpha(0f, fadeDuration, false);
        }
    }
}