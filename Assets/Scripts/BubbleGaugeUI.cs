using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BubbleGaugeUI : MonoBehaviour
{
    [Header("Gauge Settings")]
    public Sprite normalBubbleSprite; // Sprite for a full bubble
    public Sprite brokenBubbleSprite; // Sprite for an empty bubble
    public Sprite chargingBubbleSprite; // Sprite for a charging bubble
    public Sprite[] rechargeAnimationSprites; // Sprites for recharge animation
    public GameObject bubblePrefab; // Prefab for the bubble (with Image component)
    public Transform gaugeContainer; // Parent container for the gauge
    public float rechargeAnimationSpeed = 0.1f; // Speed of the recharge animation

    private List<Image> bubbleImages = new List<Image>(); // List to hold gauge bubble images
    private Coroutine[] rechargeCoroutines; // Array to track recharging animations for each bubble

    public void InitializeGauge(int maxGauge)
    {
        // Clear existing gauge visuals
        foreach (Transform child in gaugeContainer)
        {
            Destroy(child.gameObject);
        }
        bubbleImages.Clear();

        // Create new gauge visuals
        for (int i = 0; i < maxGauge; i++)
        {
            GameObject bubble = Instantiate(bubblePrefab, gaugeContainer);
            Image bubbleImage = bubble.GetComponent<Image>();
            bubbleImages.Add(bubbleImage);
            bubbleImage.sprite = brokenBubbleSprite; // Set default as broken bubble
        }

        rechargeCoroutines = new Coroutine[maxGauge];
    }

    public void UpdateGauge(int currentGauge, int chargingLevel, int maxGauge)
    {
        for (int i = 0; i < maxGauge; i++)
        {
            // Stop any ongoing recharge animation
            if (rechargeCoroutines[i] != null)
            {
                StopCoroutine(rechargeCoroutines[i]);
                rechargeCoroutines[i] = null;
            }

            if (i < currentGauge - chargingLevel)
            {
                bubbleImages[i].sprite = normalBubbleSprite; // Normal bubble
            }
            else if (i >= currentGauge - chargingLevel && i < currentGauge)
            {
                bubbleImages[i].sprite = chargingBubbleSprite; // Charging bubble
            }
            else
            {
                bubbleImages[i].sprite = brokenBubbleSprite; // Broken bubble
            }
        }
    }

    public void AnimateBubbleRecharge(int index)
    {
        if (index < 0 || index >= bubbleImages.Count) return; // Ensure the index is valid

        if (rechargeCoroutines[index] != null)
        {
            StopCoroutine(rechargeCoroutines[index]);
        }

        rechargeCoroutines[index] = StartCoroutine(PlayRechargeAnimation(index));
    }

    private IEnumerator PlayRechargeAnimation(int index)
    {
        Image bubbleImage = bubbleImages[index];

        foreach (var sprite in rechargeAnimationSprites)
        {
            bubbleImage.sprite = sprite;
            yield return new WaitForSeconds(rechargeAnimationSpeed);
        }

        bubbleImage.sprite = normalBubbleSprite; // Set the bubble to normal after recharge
    }
}
