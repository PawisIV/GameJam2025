using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Idle, Attack, Death }

    [Header("Enemy Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float moveSpeed = 2f;

    private float currentHealth;
    private EnemyState currentState = EnemyState.Idle;
    private Transform player;
    private Animator animator;
    private float attackCooldownTimer = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>(); 


    }


    private void Update()
    {
        if (currentState == EnemyState.Death) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;
            case EnemyState.Attack:
                HandleAttackState();
                break;
        }

        if (currentHealth <= 0)
        {
            ChangeState(EnemyState.Death);
        }

        attackCooldownTimer -= Time.deltaTime;
    }

    private void HandleIdleState()
    {
        // Look at the player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1); // Flip sprite to face player

        // Move towards the player if outside attack range
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            ChangeState(EnemyState.Attack);
        }
    }

    private void HandleAttackState()
    {
        if (attackCooldownTimer <= 0)
        {
            animator.SetTrigger("Attack");
            attackCooldownTimer = attackCooldown; // Reset cooldown
        }

        // Return to idle after attacking
        ChangeState(EnemyState.Idle);
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        animator.SetInteger("State", (int)newState);

        if (newState == EnemyState.Death)
        {
            HandleDeathState();
        }
    }

    private void HandleDeathState()
    {

        animator.SetTrigger("Death");
        Debug.Log("Boss is dead!");

        // Stop the timer

        Destroy(gameObject, 2f); // Destroy the boss after 2 seconds
    }

    // Optional: Add a debug visual for the attack range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player1"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // Deal 1 damage to the player
            }
        }
    }

}
