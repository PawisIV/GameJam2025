using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3; // Default health
    private int currentHealth;

    [Header("UI Settings")]
    public GameObject healthContainer;       // Parent object for health sprites
    public GameObject healthSpritePrefab;   // Prefab for a single health sprite (e.g., heart)

    private List<GameObject> healthSprites = new List<GameObject>();

    private void Start()
    {
        // Initialize player health
        currentHealth = maxHealth;
        InitializeHealthUI();
    }

    private void InitializeHealthUI()
    {
        // Create the health sprites based on maxHealth
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject healthSprite = Instantiate(healthSpritePrefab, healthContainer.transform);
            healthSprites.Add(healthSprite);
        }
    }

    private void UpdateHealthUI()
    {
        // Update health sprites visibility based on current health
        for (int i = 0; i < healthSprites.Count; i++)
        {
            healthSprites[i].SetActive(i < currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Clamp to 0

        UpdateHealthUI();

        // Handle player death
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Clamp to maxHealth

        UpdateHealthUI();
    }

    private void HandleDeath()
    {
        Debug.Log("Player is dead!");
        // Add any death logic here (respawn, game over screen, etc.)
        Destroy(gameObject); // Example: Destroy the player
    }
}
