using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Image healthFill;   // green
    public Image damageFill;   // orange/red (optional)

    [Header("Speeds")]
    public float healthLerpSpeed = 15f;   // fast
    public float damageLerpSpeed = 4f;    // slower (lag effect)

    float target = 1f;
    float current = 1f;

    void Awake()
    {
        if (healthFill) healthFill.fillAmount = 1f;
        if (damageFill) damageFill.fillAmount = 1f;
    }

    void Update()
    {
        // Smooth the main bar quickly
        current = Mathf.Lerp(current, target, Time.deltaTime * healthLerpSpeed);

        if (healthFill)
            healthFill.fillAmount = current;

        // Damage bar follows slower (only when losing health)
        if (damageFill)
        {
            float dmg = damageFill.fillAmount;

            // If healing, snap damage bar up (optional nicer feel)
            if (current > dmg)
                dmg = current;
            else
                dmg = Mathf.Lerp(dmg, current, Time.deltaTime * damageLerpSpeed);

            damageFill.fillAmount = dmg;
        }
    }

    // Call this from your player health script (value between 0 and 1)
    public void SetHealth01(float value01)
    {
        target = Mathf.Clamp01(value01);
    }
}
