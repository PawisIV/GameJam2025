using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public enum BossState { Idle, Attack, Charge, Death }

    [Header("Boss Settings")]
    public float attackCooldown = 2f; // Time between attacks
    public float chargeDistance = 3f; // Distance the boss charges
    public float chargeSpeed = 5f; // Speed of the charge
    public float chargeDelay = 0.5f; // Delay before charging
    public float returnSpeed = 2f; // Speed for returning to the original position

    [Header("Bullet Settings")]
    public GameObject bulletPrefab; // Prefab for the bullet
    public Transform bulletSpawnPoint; // Point where bullets are spawned
    public float bulletSpeed = 5f; // Speed of the bullets

    public BossState currentState;

    private CircleCollider2D circleCollider;
    private Animator animator;
    private Vector3 originalPosition; // The position to return to after charging
    public Coroutine cooldownCoroutine;

    private HealthComponent healthComponent;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        healthComponent = GetComponent<HealthComponent>();
        animator = GetComponent<Animator>();
        originalPosition = transform.position;
        ChangeState(BossState.Idle);
    }

    private void Update()
    {
        Debug.Log($"State changed to: {currentState}");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            health.TakeDamage(1);
        }
    }
    private void HandleIdleState()
    {
        // Start the cooldown coroutine only if it isn't already running
        if (cooldownCoroutine == null)
        {
            cooldownCoroutine = StartCoroutine(IdleBehavior());
        }
    }

    private IEnumerator IdleBehavior()
    {
        yield return new WaitForSeconds(attackCooldown); // Wait for cooldown

        // Randomly choose between Attack and Charge
        int randomAttack = Random.Range(0, 2);
        if (randomAttack == 0)
        {
            ChangeState(BossState.Attack);
        }
        else
        {
            ChangeState(BossState.Charge);
        }

        cooldownCoroutine = null; // Reset coroutine when finished
    }

    private void HandleAttackState()
    {
        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        animator.SetBool("Attack", true);

        // Spawn a bullet
        SpawnBullet();

        yield return new WaitForSeconds(0.5f); // Brief animation delay (adjust as needed)

        animator.SetBool("Attack", false);

        ChangeState(BossState.Idle); // Return to Idle after attack
    }

    private void HandleChargeState()
    {
        StartCoroutine(PerformCharge());
    }

    private IEnumerator PerformCharge()
    {
        animator.SetTrigger("Charge");

        // Step back slightly
        Vector3 stepBackPosition = originalPosition + transform.right * 0.5f;
        yield return MoveToPosition(stepBackPosition, returnSpeed);

        // Pause briefly before charging
        yield return new WaitForSeconds(chargeDelay);

        // Charge forward
        Vector3 chargeTarget = originalPosition - transform.right * chargeDistance;
        yield return MoveToPosition(chargeTarget, chargeSpeed);

        // Return to original position
        yield return MoveToPosition(originalPosition, returnSpeed);

        ChangeState(BossState.Idle); // Return to Idle after charge
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void HandleDeathState()
    {
        animator.SetTrigger("Death");
        Debug.Log("Boss is dead!");
    }

    public void ChangeState(BossState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        animator.SetInteger("State", (int)newState);

        switch (newState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;
            case BossState.Attack:
                HandleAttackState();
                break;
            case BossState.Charge:
                HandleChargeState();
                break;
            case BossState.Death:
                HandleDeathState();
                break;
        }
    }

    private void SpawnBullet()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(-bulletSpeed, 0f); // Move bullet horizontally
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bulletSpawnPoint.position, 0.2f); // Visualize bullet spawn point
    }
}
