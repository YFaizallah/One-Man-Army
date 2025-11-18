using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // Assign player in Inspector
    private PlayerHealth playerHealth; // Player health script

    [Header("Movement & Combat")]
    public float moveSpeed = 2f;      // Walking speed
    public float attackDistance = 2f; // Distance to start attacking
    public int damage = 1;            // Damage per attack
    private float attackCooldown = 1f; // Time between attacks
    private float lastAttackTime;

    [Header("Health")]
    public int maxHealth = 10;         // Zombie dies after 2 bullets
    private int currentHealth;
    private bool isDead = false;

    [Header("Audio")]
    public AudioClip deathSound;      // Assign death audio in Inspector
    public AudioClip attackSound;     // Assign attack audio in Inspector
    public AudioClip nearPlayerSound; // Assign near player audio in Inspector


    public int distanceToPlayerSound = 5; // Distance to play near player sound
    private AudioSource audioSource;
    private bool hasPlayedNearSound = false;

    private Rigidbody rb;
    private CapsuleCollider capsule;


    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();  // Get Zombie Rigidbody
        capsule = GetComponent<CapsuleCollider>();
        
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1.0f; // Make it 3D sound
            audioSource.playOnAwake = false;
        }
        
        currentHealth = maxHealth;

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (player == null || isDead) return;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        // Rotate zombie toward player
        Vector3 lookDir = new Vector3(direction.x, 0, direction.z);
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(lookDir),
                                                  Time.deltaTime * 5f);

        if (distance > attackDistance)
        {
            // Walk toward player
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
            
        }
        else
        {
            // Attack player
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);

           

            if (Time.time - lastAttackTime > attackCooldown)
            {
                if (playerHealth != null)
                    playerHealth.TakeDamage(damage);

                // Play attack sound
                if (attackSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(attackSound);
                }

                lastAttackTime = Time.time;
            }
        }

        // Near player sound reset
        if (distance > distanceToPlayerSound)
        {
            // Reset sound flag when player moves away
            hasPlayedNearSound = false;
        }
        else
        {
             // Play near player sound once when entering range
            if (!hasPlayedNearSound && nearPlayerSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(nearPlayerSound);
                hasPlayedNearSound = true;
            }
        }
    }

    // Call this when the zombie gets hit by a bullet
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        rb.isKinematic = true;
        isDead = true;
        
        // Play death sound once
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        
        capsule.direction = 2;
        capsule.center = new Vector3(0f, -0.15f, 0.05f);
        animator.SetBool("isDead", true); // Make sure to create this bool in Animator
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);
        FindObjectOfType<LevelManager>().ZombieKilled();
        // Destroy zombie after 2 seconds so death animation can play
        //Destroy(gameObject, 2f);
    }
}

