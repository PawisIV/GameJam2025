using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BubbleGaugeUI : MonoBehaviour
{
    [Header("Gauge Settings")]
    public Sprite normalBubbleSprite; // Sprite for a full bubble
    public Sprite brokenBubbleSprite; // Sprite for an empty bubble
    public Sprite[] rechargeAnimationSprites; // Sprites for recharge animation
    public GameObject bubblePrefab; // Prefab for the bubble (with Image component)
    public Transform gaugeContainer; // Parent container for the gauge

    private List<Image> bubbleImages = new List<Image>(); // List to hold gauge bubble images
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
    }
    public void UpdateGauge(int currentGauge, int maxGauge)
    {
        for (int i = 0; i < maxGauge; i++)
        {
            bubbleImages[i].sprite = i < currentGauge ? normalBubbleSprite : brokenBubbleSprite;
        }
    }

    public void AnimateBubbleRecharge(int index)
    {
        if (index < 0 || index >= bubbleImages.Count) return; // Ensure the index is valid

        StartCoroutine(PlayRechargeAnimation(bubbleImages[index]));
    }

    private IEnumerator PlayRechargeAnimation(Image bubbleImage)
    {
        // Play the recharge animation using the sprites
        foreach (var sprite in rechargeAnimationSprites)
        {
            bubbleImage.sprite = sprite;
            yield return new WaitForSeconds(0.1f); // Adjust duration for animation frames
        }

        // Set the bubble to normal after the animation
        bubbleImage.sprite = normalBubbleSprite;
    }
}
