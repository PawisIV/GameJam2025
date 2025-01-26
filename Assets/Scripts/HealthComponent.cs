using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] public float MaxHealth = 50f;
    public float currentHealth;
    [SerializeField] private Timer timer; // Reference to your timer
    private bool isDead = false; // To prevent multiple calls to death logic
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damageColor = Color.red; // Flash color
    [SerializeField] private float colorFlashDuration = 0.2f; // Flash duration
    private BossController bossController;

    private void Awake()
    {
        currentHealth = MaxHealth;
        bossController = GetComponent<BossController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is missing!");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // Ignore further damage if already dead

        if (!bossController.isImmune)
        {
            currentHealth -= damage;

            // Flash damage color if SpriteRenderer is available
            if (spriteRenderer != null)
            {
                StartCoroutine(FlashDamageColor());
            }
        }
        Debug.Log(currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            timer.StopTimer(); // Stop the timer when the boss dies
            bossController.cooldownCoroutine = null; // Cancel boss cooldowns
            bossController.ChangeState(BossController.BossState.Death);
            StartCoroutine(HandleBossDeath());
        }
    }

    private IEnumerator HandleBossDeath()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second (death animation or effects)

        // Record the time taken to defeat the boss
        float timeToDefeatBoss = timer.GetElapsedTime(); // Assuming Timer class has GetElapsedTime
        PlayerPrefs.SetFloat("TimeToDefeatBoss", timeToDefeatBoss); // Save defeat time using PlayerPrefs

        // Load the win score scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Ensure the next scene is the win score scene
    }

    private IEnumerator FlashDamageColor()
    {
        spriteRenderer.color = damageColor; // Change to damage color
        yield return new WaitForSeconds(colorFlashDuration);
        spriteRenderer.color = Color.white; // Reset to original color
    }
}
