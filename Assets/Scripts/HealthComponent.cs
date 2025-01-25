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

        currentHealth -= damage;
        Debug.Log(currentHealth);

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDamageColor());
        }

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            bossController.cooldownCoroutine = null;
            bossController.ChangeState(BossController.BossState.Death);
        }
    }

    private IEnumerator FlashDamageColor()
    {
        spriteRenderer.color = damageColor; // Change to damage color
        yield return new WaitForSeconds(colorFlashDuration);
        spriteRenderer.color = Color.white; // Reset to original color
    }
}
