using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public enum BossState { Idle, Attack, Charge, Summon, Death }

    [Header("Boss Settings")]
    public float attackCooldown = 2f; // Time between attacks
    public float chargeDistance = 3f; // Distance the boss charges
    public float chargeSpeed = 5f; // Speed of the charge
    public float chargeDelay = 0.5f; // Delay before charging
    public float returnSpeed = 2f; // Speed for returning to the original position
    public GameObject deathPrefab;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab; // Prefab for the bullet
    public Transform bulletSpawnPoint; // Point where bullets are spawned
    public float bulletSpeed = 5f; // Speed of the bullets

    [Header("Summon Settings")]
    public GameObject thornPrefab; // Thorn prefab
    public GameObject thornSignalPrefab;
    public float summonPreviewDelay = 0.5f; // Time before thorns rise
    public float thornFreezeTime = 1f; // Time thorns stay visible
    public float thornRiseSpeed = 2f; // Speed of thorn rising
    public float thornDisappearSpeed = 1f; // Speed of thorn disappearing
    public int minThorns = 3; // Minimum number of thorns
    public int maxThorns = 5; // Maximum number of thorns
    public Vector2 summonAreaMin; // Minimum summon area (world space)
    public Vector2 summonAreaMax; // Maximum summon area (world space)

    public BossState currentState;

    public bool isImmune = false;
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
        animator.SetFloat("HealthPercentage", healthComponent.MaxHealth);
        originalPosition = transform.position;
        ChangeState(BossState.Idle);
    }

    private void Update()
    {
        float healthPercentage = (healthComponent.currentHealth / healthComponent.MaxHealth) * 100f;
        animator.SetFloat("HealthPercentage", healthPercentage);
        ChangeStat(healthPercentage);
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
    private void ChangeStat(float healthPercentage)
    {
        if (healthPercentage < 65 && healthPercentage >= 35)
        {
            attackCooldown = 3.5f;
            bulletSpeed = 6;
            chargeDelay = 1.5f;
            summonPreviewDelay = 1.5f;
        }
        else if (healthPercentage < 30)
        {
            attackCooldown = 2;
            bulletSpeed = 7;
            chargeDelay = 1f;
            summonPreviewDelay = 1f;
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
        int randomAttack = Random.Range(0, 9);
        if (randomAttack >= 0 && randomAttack < 5)
        {
            ChangeState(BossState.Attack);
        }
        else if (randomAttack >= 5 && randomAttack < 7)
        {
            ChangeState(BossState.Charge);
        }
        else
        {
            ChangeState(BossState.Summon);
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
        yield return new WaitForSeconds(0.8f);
        // Spawn a bullet
        SpawnBullet();

        yield return new WaitForSeconds(0.8f);
        SpawnBullet();

        yield return new WaitForSeconds(0.8f);
        SpawnBullet();
        yield return new WaitForSeconds(0.8f);
        SpawnBullet();
        yield return new WaitForSeconds(0.5f);// Brief animation delay (adjust as needed)

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
        isImmune = true;

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
        isImmune = false;

        ChangeState(BossState.Idle); // Return to Idle after charge
    }

    private void HandleSummonState()
    {
        StartCoroutine(PerformSummon());
    }

    private IEnumerator PerformSummon()
    {
        animator.SetTrigger("Summon");

        int thornCount = Random.Range(minThorns, maxThorns + 1); // Randomize thorn count
        GameObject[] thorns = new GameObject[thornCount];
        GameObject[] signals = new GameObject[thornCount];

        // Summon thorns with preview signals
        for (int i = 0; i < thornCount; i++)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(summonAreaMin.x, summonAreaMax.x),
                Random.Range(summonAreaMin.y, summonAreaMax.y)
            );

            // Create the signal effect
            GameObject signal = Instantiate(thornSignalPrefab, randomPosition + new Vector2(0, 2.5f), Quaternion.identity);
            signals[i] = signal;

            // Wait for the preview delay
            yield return new WaitForSeconds(summonPreviewDelay);

            // Spawn the thorn at the same position
            GameObject thorn = Instantiate(thornPrefab, randomPosition, Quaternion.identity);
            thorns[i] = thorn;

            // Remove or deactivate the signal
            Destroy(signal);
            StartCoroutine(RiseThorn(thorn));
        }

        yield return new WaitForSeconds(thornFreezeTime);

        // Destroy thorns after they disappear
        foreach (var thorn in thorns)
        {
            if (thorn != null)
            {
                StartCoroutine(DisappearThorn(thorn));
            }
        }

        yield return new WaitForSeconds(1f); // Adjust timing as needed

        ChangeState(BossState.Idle); // Return to Idle after summoning
    }


    private IEnumerator RiseThorn(GameObject thorn)
    {
        Vector3 targetPosition = thorn.transform.position + Vector3.up * 3f; // Adjust height
        while (thorn.transform.position.y < targetPosition.y)
        {
            thorn.transform.position = Vector3.MoveTowards(thorn.transform.position, targetPosition, thornRiseSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator DisappearThorn(GameObject thorn)
    {
        Vector3 targetPosition = thorn.transform.position + Vector3.down * 3f; // Move down
        while (thorn.transform.position.y > targetPosition.y)
        {
            thorn.transform.position = Vector3.MoveTowards(thorn.transform.position, targetPosition, thornDisappearSpeed * Time.deltaTime);
            yield return null;
        }
        Destroy(thorn); // Remove thorn after disappearing
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
        Instantiate(deathPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
            case BossState.Summon:
                HandleSummonState();
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
            // Randomize the Y position of the bullet
            Vector3 randomSpawnPosition = bulletSpawnPoint.position;
            randomSpawnPosition.y += Random.Range(-1f, 0.5f); // Adjust the range as needed

            GameObject bullet = Instantiate(bulletPrefab, randomSpawnPosition, Quaternion.identity);
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
