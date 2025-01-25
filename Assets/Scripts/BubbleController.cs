using UnityEngine;

public class BubbleController : MonoBehaviour
{
    [Header("Bubble Settings")]
    public int maxGauge = 8; // Maximum gauge capacity
    public int maxCharge = 4; // Maximum charge usable
    public float chargeTimePerGauge = 0.5f; // Time required to charge each gauge
    public float rechargeTimePerGauge = 0.5f; // Time required to recharge one gauge
    public BubbleStats[] bubbleStats; // Array to store bubble stats for each charge level

    [Header("UI References")]
    public BubbleGaugeUI bubbleGaugeUI; // Reference to the UI manager for the gauge

    [Header("Bubble Sprites")]
    public Sprite smallBubbleSprite;
    public Sprite mediumBubbleSprite;
    public Sprite largeBubbleSprite;

    [Header("Spawn Settings")]
    public Transform bubbleSpawnPoint; // Transform to set spawn position
    public Vector2 spawnOffset; // Offset from the spawn point
    public bool facingRight = true; // Track the player's facing direction

    private bool isCharging = false;
    private int currentCharge = 1; // Start at 1
    private int currentGauge; // Current available gauge
    private float chargeTimer = 0f; // Timer to track charging progress
    private float rechargeTimer = 0f; // Timer to track recharging progress

    private void Start()
    {
        currentGauge = maxGauge; // Initialize current gauge to maximum
        bubbleGaugeUI.InitializeGauge(maxGauge);
        bubbleGaugeUI.UpdateGauge(currentGauge, 0, maxGauge);
    }

    private void Update()
    {
        HandlePlayerDirection();
        HandleBubbleCharge();
        RechargeGauge();
    }

    private void HandlePlayerDirection()
    {
        // Flip facing direction based on horizontal input
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal > 0 && !facingRight)
        {
            FlipDirection();
        }
        else if (horizontal < 0 && facingRight)
        {
            FlipDirection();
        }
    }

    private void FlipDirection()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void HandleBubbleCharge()
    {
        if (Input.GetButton("Fire1"))
        {
            chargeTimer += Time.deltaTime;
            isCharging = true;
            if (chargeTimer >= chargeTimePerGauge && currentCharge < maxCharge && currentCharge < currentGauge)
            {
                currentCharge++;
                chargeTimer = 0f;
                Debug.Log($"Charging: {currentCharge}");
                bubbleGaugeUI.UpdateGauge(currentGauge, currentCharge, maxGauge);
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (currentCharge <= currentGauge)
            {
                ShootBubble(currentCharge);
                currentGauge -= currentCharge;
                bubbleGaugeUI.UpdateGauge(currentGauge, currentCharge,maxGauge); // Update the UI
                Debug.Log($"Bubble shot! Remaining Gauge: {currentGauge}");
            }
            else
            {
                Debug.LogWarning("Not enough gauge to shoot!");
            }

            isCharging=false;
            currentCharge = 1;
            chargeTimer = 0f;
            bubbleGaugeUI.UpdateGauge(currentGauge, 0, maxGauge);
        }
    }

    private void RechargeGauge()
    {
        if (currentGauge < maxGauge && !isCharging) // Recharge only if not at max gauge
        {
            rechargeTimer += Time.deltaTime;

            if (rechargeTimer >= rechargeTimePerGauge)
            {
                bubbleGaugeUI.AnimateBubbleRecharge(currentGauge); // Animate the recharging bubble
                currentGauge++;
                rechargeTimer = 0f; // Reset recharge timer
                Debug.Log($"Gauge recharged: {currentGauge}/{maxGauge}");
            }
        }
    }

    private void ShootBubble(int chargeLevel)
    {
        if (chargeLevel > 0 && chargeLevel <= bubbleStats.Length)
        {
            BubbleStats stats = bubbleStats[chargeLevel - 1];

            // Create a new GameObject for the bubble
            GameObject bubble = new GameObject("Bubble");
            bubble.transform.position = bubbleSpawnPoint.position + (Vector3)spawnOffset * (facingRight ? 1 : -1);
            bubble.transform.localScale = Vector3.one * stats.size;

            // Add SpriteRenderer to display the correct sprite
            SpriteRenderer renderer = bubble.AddComponent<SpriteRenderer>();
            renderer.sprite = GetBubbleSprite(chargeLevel);

            // Add the BubbleWobble component for the wobbling effect
            BubbleWobble wobble = bubble.AddComponent<BubbleWobble>();
            wobble.Initialize(stats.speed, facingRight);

            // Add CircleCollider2D and adjust its radius to match the size
            CircleCollider2D collider = bubble.AddComponent<CircleCollider2D>();
            collider.isTrigger = true; // Ensure it's a trigger for proper detection
            collider.radius = renderer.sprite.bounds.extents.x; // Use half the width of the sprite as the radius

            // Add Rigidbody2D for physics simulation
            Rigidbody2D rb = bubble.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // No gravity for the bubble

            // Add the Bubble script and initialize it
            Bubble bubbleComponent = bubble.AddComponent<Bubble>();
            bubbleComponent.Initialize(stats.damage, stats.speed, stats.size);

            // Set the bubble's velocity direction based on player facing
            float direction = facingRight ? 1f : -1f;  // 1 for right, -1 for left
            rb.velocity = new Vector2(direction * stats.speed, 0f); // Move the bubble in the correct direction

            Debug.Log($"Bubble shot with Charge {chargeLevel}: Damage {stats.damage}, Speed {stats.speed}, Size {stats.size}x");
        }
        else
        {
            Debug.LogWarning("Invalid charge level!");
        }
    }
    private Sprite GetBubbleSprite(int chargeLevel)
    {
        switch (chargeLevel)
        {
            case 1: return smallBubbleSprite;
            case 2: return mediumBubbleSprite;
            case 3: return largeBubbleSprite;
            default: return largeBubbleSprite;
        }
    }
}

[System.Serializable]
public class BubbleStats
{
    public float damage;
    public float speed;
    public float size;
}

public class BubbleWobble : MonoBehaviour
{
    private float speed;
    private bool facingRight;
    private float wobbleAmplitude = 0.5f; // Adjust for how much the bubble wobbles
    private float wobbleFrequency = 5f;  // Adjust for how fast the wobble is
    private Vector3 startPosition;

    public void Initialize(float bubbleSpeed, bool isFacingRight)
    {
        speed = bubbleSpeed;
        facingRight = isFacingRight;
        startPosition = transform.position;
        tag = "Bubble";
    }

    private void Update()
    {
        // Move the bubble forward while applying a sine wave wobble
        float direction = facingRight ? 1 : -1;
        float wobble = Mathf.Sin(Time.time * wobbleFrequency) * wobbleAmplitude;

        transform.position += new Vector3(speed * direction * Time.deltaTime, wobble * Time.deltaTime, 0);
    }
}
