using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    [Header("UI")]
    [SerializeField] private HealthBarController healthBar; // drag from PlayerUI in inspector

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); // initialize bar
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); // re-init if object enabled again
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player hit! Health: " + currentHealth);

        UpdateHealthUI(); // ? update bar every time health changes

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // OPTIONAL: if you have healing
    public void Heal(int amount)
    {
        if (amount <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            float health01 = (float)currentHealth / maxHealth;
            healthBar.SetHealth01(health01);
        }
        else
        {
            // If you forgot to drag it, this will tell you once you test
            // (You can remove this log later)
            // Debug.LogWarning("HealthBarController reference not set on PlayerHealth!");
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        FindObjectOfType<LevelManager>().PlayerDied();
    }
}
