using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image healthFill;   // green bar
    [SerializeField] private Image damageFill;   // delayed bar (optional)

    [Header("Timing")]
    [SerializeField] private float minDuration = 0.15f; // small damage
    [SerializeField] private float maxDuration = 0.6f;  // big damage
    [SerializeField] private float damageDelay = 0.2f;

    private Coroutine healthRoutine;
    private Coroutine damageRoutine;

    public void SetHealth01(float target01)
    {
        target01 = Mathf.Clamp01(target01);

        if (healthRoutine != null) StopCoroutine(healthRoutine);
        healthRoutine = StartCoroutine(SmoothFill(healthFill, target01));

        if (damageFill != null)
        {
            if (damageRoutine != null) StopCoroutine(damageRoutine);
            damageRoutine = StartCoroutine(SmoothDamageFill(target01));
        }
    }

    private IEnumerator SmoothFill(Image img, float target)
    {
        float start = img.fillAmount;
        float diff = Mathf.Abs(start - target);

        float duration = Mathf.Lerp(minDuration, maxDuration, diff);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Ease Out (smooth finish)
            t = EaseOutCubic(t);

            img.fillAmount = Mathf.Lerp(start, target, t);
            yield return null;
        }

        img.fillAmount = target;
    }

    private IEnumerator SmoothDamageFill(float target)
    {
        yield return new WaitForSeconds(damageDelay);

        float start = damageFill.fillAmount;
        float diff = Mathf.Abs(start - target);

        float duration = Mathf.Lerp(minDuration, maxDuration, diff);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = EaseOutCubic(t);

            damageFill.fillAmount = Mathf.Lerp(start, target, t);
            yield return null;
        }

        damageFill.fillAmount = target;
    }

    // ?? EASING FUNCTION (this is the magic)
    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
