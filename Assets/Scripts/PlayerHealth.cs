using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3; // Default health
    private int currentHealth;

    [Header("UI Settings")]
    public RectTransform healthContainer; // Parent UI container for health sprites
    public Sprite healthSprite;          // Sprite for a single health icon (e.g., heart)
    public Vector2 spriteSize = new Vector2(50, 50); // Size of each sprite (in pixels)
    public float spacing = 10f;          // Spacing between health sprites

    [Header("Iframe Settings")]
    public float iframeDuration = 1.0f;  // Duration of invincibility frames
    public float blinkInterval = 0.1f;  // Interval for blinking effect
    private bool isInvincible = false;  // To track iframe state

    private List<Image> healthImages = new List<Image>();
    private SpriteRenderer playerRenderer; // Reference to the player's SpriteRenderer or MeshRenderer
    [SerializeField] private Timer timer;

    private void Start()
    {
        // Initialize player health
        currentHealth = maxHealth;
        InitializeHealthUI();

        // Get the player's sprite renderer (for blinking effect)
        playerRenderer = GetComponent<SpriteRenderer>();
        if (playerRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the player object!");
        }
    }

    private void InitializeHealthUI()
    {
        // Clear any existing health sprites (in case of reinitialization)
        foreach (var image in healthImages)
        {
            Destroy(image.gameObject);
        }
        healthImages.Clear();

        // Create health sprites based on maxHealth
        for (int i = 0; i < maxHealth; i++)
        {
            CreateHealthSprite(i);
        }
    }

    private void CreateHealthSprite(int index)
    {
        // Create a new GameObject for the sprite
        GameObject healthObject = new GameObject("HealthSprite_" + index, typeof(RectTransform));
        healthObject.transform.SetParent(healthContainer, false);

        // Set size and position of the sprite
        RectTransform rectTransform = healthObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = spriteSize;
        rectTransform.anchoredPosition = new Vector2(index * (spriteSize.x + spacing), 0);

        // Add Image component and assign the health sprite
        Image image = healthObject.AddComponent<Image>();
        image.sprite = healthSprite;
        healthImages.Add(image);
    }

    private void UpdateHealthUI()
    {
        // Update the visibility of each health sprite
        for (int i = 0; i < healthImages.Count; i++)
        {
            healthImages[i].enabled = i < currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // Prevent taking damage during iframe

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Clamp to 0

        UpdateHealthUI();

        // Trigger iframe and blinking effect
        StartCoroutine(HandleIframes());

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

    private IEnumerator HandleIframes()
    {
        isInvincible = true;

        // Blink the player during iframes
        float elapsedTime = 0f;
        while (elapsedTime < iframeDuration)
        {
            playerRenderer.enabled = !playerRenderer.enabled; // Toggle visibility
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        playerRenderer.enabled = true; // Ensure the player is visible at the end
        isInvincible = false;
    }

    private void HandleDeath()
    {
        Debug.Log("Player is dead!");
        if (timer != null)
        {
            timer.StopTimer();
        }
        else
        {
            Debug.LogWarning("Timer reference is missing!");
        }
        // Add any death logic here (respawn, game over screen, etc.)
        Destroy(gameObject); // Example: Destroy the player
        LoadLossScene();
    }
    private void LoadLossScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+2);
    }


}
