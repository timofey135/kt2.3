using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float triggerRange = 8f;
    [SerializeField] private float stopDistance = 0.1f;
    [SerializeField] private float resetDistance = 20f;
    [SerializeField] private Transform player;

    [SerializeField] private Animator animator;

    private bool isChasing = false; 
    private bool hasBeenTriggered = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        CheckTriggerZone();


    }

    void FixedUpdate()
    {
        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void CheckTriggerZone()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (!hasBeenTriggered && distanceToPlayer <= triggerRange)
        {
            hasBeenTriggered = true;
            isChasing = true;
        }

        if (hasBeenTriggered && distanceToPlayer > resetDistance)
        {
            hasBeenTriggered = false;
            isChasing = false;
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            RestartLevel();
        }
    }


    void RestartLevel()
    {
        Debug.Log("Player caught! Restarting level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = hasBeenTriggered ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, resetDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}