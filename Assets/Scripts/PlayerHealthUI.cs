using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public RectTransform healthFill;

    private float maxWidth;

    void Start()
    {
        maxWidth = healthFill.sizeDelta.x; // initial width
    }

    void Update()
    {
        float t = (float)playerHealth.currentHealth / playerHealth.maxHealth;
        healthFill.sizeDelta = new Vector2(maxWidth * t, healthFill.sizeDelta.y);
    }
}
