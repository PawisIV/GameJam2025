using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public enum BossState { Idle, Attack, Death }

    [Header("Boss Settings")]
    public float attackCooldown = 2f; // Time between attacks

    [Header("Bullet Settings")]
    public GameObject bulletPrefab; // Prefab for the bullet
    public Transform bulletSpawnPoint; // Point where bullets are spawned
    public float bulletSpeed = 5f; // Speed of the bullets

    public BossState currentState = BossState.Idle;
    private Animator animator;
    private float attackCooldownTimer = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ChangeState(BossState.Idle);
    }

    private void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;
            case BossState.Attack:
                HandleAttackState();
                break;
            case BossState.Death:
                HandleDeathState();
                break;
        }

        attackCooldownTimer -= Time.deltaTime;
    }

    private void HandleIdleState()
    {
        if (attackCooldownTimer <= 0f)
        {
            ChangeState(BossState.Attack);
        }
    }

    private void HandleAttackState()
    {
        animator.SetBool("Attack", true);
        SpawnBullet();
        attackCooldownTimer = attackCooldown;
        animator.SetBool("Attack", false);
        ChangeState(BossState.Idle); // Return to Idle after attacking
    }

    private void SpawnBullet()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

            //Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            //if (rb != null)
            //{
            //    rb.velocity = new Vector2(bulletSpeed, 0f); // Move bullet horizontally
            //}
        }
    }

    private void HandleDeathState()
    {
        animator.SetTrigger("Death");
        Debug.Log("Boss is dead!");
        Destroy(gameObject, 10f); // Destroy boss after 2 seconds
    }

    private void ChangeState(BossState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        animator.SetInteger("State", (int)newState);
    }
    private IEnumerator Attacking()
    {
        
        yield return new WaitForSeconds(attackCooldown);
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bulletSpawnPoint.position, 0.2f); // Visualize bullet spawn point
    }
}
