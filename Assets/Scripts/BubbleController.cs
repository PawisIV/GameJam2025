using UnityEngine;

public class BubbleController : MonoBehaviour
{
    [Header("Bubble Settings")]
    public int maxGauge = 8; // Maximum gauge capacity
    public int maxCharge = 4; // Maximum charge usable
    public float chargeTimePerGauge = 0.5f; // Time required to charge each gauge
    public float rechargeTimePerGauge = 0.5f; // Time required to recharge one gauge
    public BubbleStats[] bubbleStats; // Array to store bubble stats for each charge level

    private int currentCharge = 1; // Start at 1
    private int currentGauge; // Current available gauge
    private float chargeTimer = 0f; // Timer to track charging progress
    private float rechargeTimer = 0f; // Timer to track recharging progress

    private void Start()
    {
        currentGauge = maxGauge; // Initialize current gauge to maximum
    }

    private void Update()
    {
        HandleBubbleCharge();
        RechargeGauge();
    }

    private void HandleBubbleCharge()
    {
        if (Input.GetButton("Fire1")) // Start charging when Fire1 is held
        {
            chargeTimer += Time.deltaTime;

            if (chargeTimer >= chargeTimePerGauge && currentCharge < maxCharge && currentCharge < currentGauge)
            {
                currentCharge++;
                chargeTimer = 0f; // Reset the timer
                Debug.Log($"Charging: {currentCharge}");
            }
        }

        if (Input.GetButtonUp("Fire1")) // Release to shoot the bubble
        {
            if (currentCharge <= currentGauge)
            {
                ShootBubble(currentCharge);
                currentGauge -= currentCharge; // Subtract current charge from the gauge
                Debug.Log($"Bubble shot! Remaining Gauge: {currentGauge}");
            }
            else
            {
                Debug.LogWarning("Not enough gauge to shoot!");
            }

            currentCharge = 1; // Reset charge to 1
            chargeTimer = 0f; // Reset the timer
        }
    }

    private void RechargeGauge()
    {
        if (currentGauge < maxGauge) // Recharge only if not at max gauge
        {
            rechargeTimer += Time.deltaTime;

            if (rechargeTimer >= rechargeTimePerGauge)
            {
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

            GameObject bubble = Instantiate(Resources.Load<GameObject>("BubblePrefab"), transform.position, Quaternion.identity);
            Bubble bullet = bubble.GetComponent<Bubble>();
            bullet.Initialize(stats.damage, stats.speed, stats.size);

            Debug.Log($"Bubble shot with Charge {chargeLevel}: Damage {stats.damage}, Speed {stats.speed}, Size {stats.size}x");
        }
        else
        {
            Debug.LogWarning("Invalid charge level!");
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
