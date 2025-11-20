using UnityEngine;

public class NPCManScript : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Animator animator;

    [Header("Interaction Settings")]
    public float detectionRange = 10f;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Player enters range
        if (distanceToPlayer <= detectionRange)
        {
            if (!playerInRange)
            {
                playerInRange = true;

                if (animator != null)
                    animator.SetBool("PlayerNear", true);
            }

            // Interaction key (currently does nothing)
            if (Input.GetKeyDown(interactKey))
            {
                Debug.Log("Player interacted with NPC.");
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;

                if (animator != null)
                    animator.SetBool("PlayerNear", false);
            }
        }
    }

    // Optional: visualize detection range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
