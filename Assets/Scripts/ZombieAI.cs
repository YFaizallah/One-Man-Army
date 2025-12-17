using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // Player target
    private PlayerHealth playerHealth;

    [Header("Movement & Combat")]
    public float moveSpeed = 2f;
    public float attackDistance = 2f;
    public float detectionRange = 25f;
    public int damage = 1;
    private float attackCooldown = 1f;
    private float lastAttackTime;

    [Header("Health")]
    public int maxHealth = 10;
    private int currentHealth;
    private bool isDead = false;

    [Header("Audio")]
    public AudioClip deathSound;
    public AudioClip attackSound;
    public AudioClip nearPlayerSound;
    public int distanceToPlayerSound = 5;
    private AudioSource audioSource;
    private bool hasPlayedNearSound = false;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Animator animator;

    void Awake()
    {
        // Auto-assign player if null
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("Player not found! Make sure Player has tag 'Player'.");
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        // Audio setup
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
        }

        // Freeze Y rotation + position
        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeRotationX |
                             RigidbodyConstraints.FreezeRotationZ |
                             RigidbodyConstraints.FreezePositionY;

        currentHealth = maxHealth;

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (player == null || isDead) return;

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        // Too far? Idle
        if (distance > detectionRange)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isFar", true);
            return;
        }
        else
            animator.SetBool("isFar", false);

        // Rotate toward player
        Vector3 lookDir = new Vector3(direction.x, 0, direction.z);
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(lookDir),
                                                  Time.deltaTime * 5f);

        if (distance > attackDistance)
        {
            // Move toward player
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }
        else
        {
            // Attack
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);

            if (Time.time - lastAttackTime > attackCooldown)
            {
                playerHealth?.TakeDamage(damage);
                if (attackSound != null) audioSource.PlayOneShot(attackSound);
                lastAttackTime = Time.time;
            }
        }

        // Play near player sound once
        if (distance <= distanceToPlayerSound && !hasPlayedNearSound && nearPlayerSound != null)
        {
            audioSource.PlayOneShot(nearPlayerSound);
            hasPlayedNearSound = true;
        }
        else if (distance > distanceToPlayerSound)
            hasPlayedNearSound = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.None;
        }

        if (deathSound != null) audioSource.PlayOneShot(deathSound);

        capsule.direction = 2;
        capsule.center = new Vector3(0f, -0.15f, 0.05f);
        animator.SetBool("isDead", true);
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);

        FindObjectOfType<LevelManager>()?.ZombieKilled();

        Destroy(gameObject, 5f);
    }
}
