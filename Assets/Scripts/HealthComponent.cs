using System.Collections;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float MaxHealth = 50f;
    public float currentHealth;
    [SerializeField] private Timer timer;
    private bool isDead = false; // To prevent multiple calls to death logic
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    [SerializeField] private Color damageColor = Color.red; // Color to flash when damaged
    [SerializeField] private float colorFlashDuration = 0.2f; // Duration of the flash

    private void Awake()
    {
        currentHealth = MaxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer is missing!");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // Ignore further damage if already dead

        currentHealth -= damage;
        Debug.Log(currentHealth);

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDamageColor());
        }

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator FlashDamageColor()
    {
        spriteRenderer.color = damageColor; // Change to damage color
        yield return new WaitForSeconds(colorFlashDuration);
        spriteRenderer.color = Color.white; // Reset to original color
    }

    private IEnumerator HandleDeath()
    {
        // Stop the timer
        if (timer != null)
        {
            timer.StopTimer();
        }
        else
        {
            Debug.LogWarning("Timer reference is missing!");
        }

        // Add a delay before destroying the boss
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}
