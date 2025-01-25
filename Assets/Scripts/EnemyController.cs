using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Idle, Attack, Death }

    [Header("Enemy Settings")]
    public float attackCooldown = 1.5f; // Cooldown between attacks
    public GameObject bulletPrefab; // Prefab for the bullet
    public Transform bulletSpawnPoint; // Where bullets are spawned
    public float bulletSpeed = 5f; // Speed of the spawned bullets

    private float currentHealth = 100f;
    private EnemyState currentState = EnemyState.Idle;
    private Animator animator;
    private float attackCooldownTimer = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
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

        attackCooldownTimer -= Time.deltaTime;

        if (currentHealth <= 0)
        {
            ChangeState(EnemyState.Death);
        }
    }

    private void HandleIdleState()
    {
        // Transition to attack state when cooldown is over
        if (attackCooldownTimer <= 0)
        {
            ChangeState(EnemyState.Attack);
        }
    }

    private void HandleAttackState()
    {
        PerformAttack();
        attackCooldownTimer = attackCooldown; // Reset cooldown
        ChangeState(EnemyState.Idle); // Return to Idle after attacking
    }

    private void PerformAttack()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

            //Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            //if (rb != null)
            //{
            //    rb.velocity = Vector2.right * bulletSpeed; // Adjust direction as needed
            //}

            //// Optional: Destroy the bullet after a certain time to avoid clutter
            //Destroy(bullet, 5f);

            animator.SetTrigger("Attack"); // Play attack animation
        }
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
        Destroy(gameObject, 2f); // Destroy the boss after 2 seconds
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Boss took {damage} damage. Current health: {currentHealth}");
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the bullet spawn point in the editor
        if (bulletSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(bulletSpawnPoint.position, 0.1f);
        }
    }
}
