using UnityEngine;
using System.Collections;

public class PauseMenuAnimator : MonoBehaviour
{
    [Header("Target (MenuPanel or PauseMenu)")]
    public RectTransform panel;

    [Header("Animation")]
    public float startScale = 0.4f;
    public float endScale = 0.44f;
    public float duration = 0.12f;

    CanvasGroup cg;

    void Awake()
    {
        if (panel == null) panel = (RectTransform)transform;
        cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.gameObject.AddComponent<CanvasGroup>();
    }

    public void PlayShow()
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        float t = 0f;
        cg.alpha = 0f;
        panel.localScale = Vector3.one * startScale;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);

            // Ease out
            float eased = 1f - Mathf.Pow(1f - k, 3f);

            cg.alpha = eased;
            panel.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, eased);
            yield return null;
        }

        cg.alpha = 1f;
        panel.localScale = Vector3.one * endScale;
    }
}
