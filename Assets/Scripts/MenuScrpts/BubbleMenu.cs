using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BubbleMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Bubble Sprites")]
    public Sprite bubbleSprite; // The default bubble sprite
    public Sprite popSprite;    // The sprite to show when the bubble pops
    public Sprite[] regenSprites; // Array of sprites for the regeneration animation (e.g., 4 frames)

    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 10f;
    public float hoverShakeMagnitude = 5f;

    [Header("Pop Settings")]
    public float regenDelay = 1f; // Delay before starting regeneration
    public float regenFrameDuration = 0.2f; // Duration of each frame in the regeneration animation

    [Header("Movement Settings")]
    public float movementRadius = 50f;
    public float movementSpeed = 1f;

    [Header("Scaling Settings")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float scalingSpeed = 2f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;
    public float maxRotationAngle = 15f;

    private Image bubbleImage;
    private RectTransform bubbleRect;
    private int clickCount = 0;
    private bool isPopped = false;
    private Vector3 startPosition;
    private float savedIndex;
    private float currentRotationAngle = 0f;
    private float rotationDirection = 1f;
    private bool isHovering = false;

    private void Start()
    {
        bubbleImage = GetComponent<Image>();
        bubbleRect = GetComponent<RectTransform>();

        if (bubbleImage == null)
        {
            Debug.LogError("This script must be attached to a GameObject with an Image component!");
            return;
        }

        bubbleImage.sprite = bubbleSprite;

        startPosition = bubbleRect.localPosition;
        savedIndex = Random.Range(0f, Mathf.PI * 2f);
        rotationDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
    }

    private void Update()
    {
        if (!isPopped)
        {
            MoveBubble();
            ScaleBubble();
            RotateBubble();

            if (isHovering)
            {
                HoverShake();
            }
        }
    }

    public void OnBubbleClick()
    {
        if (isPopped) return;

        clickCount++;

        if (clickCount >= 3)
        {
            PopBubble();
        }
        else
        {
            StartCoroutine(Shake());
        }
    }

    private void MoveBubble()
    {
        float sine = Mathf.Sin(Time.time * movementSpeed + savedIndex);
        float cosine = Mathf.Cos(Time.time * movementSpeed + savedIndex);

        bubbleRect.localPosition = startPosition + new Vector3(sine * movementRadius, cosine * movementRadius, 0);
    }

    private void ScaleBubble()
    {
        float scale = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(Time.time * scalingSpeed, 1));
        bubbleRect.localScale = new Vector3(scale, scale, 1);
    }

    private void RotateBubble()
    {
        currentRotationAngle += rotationSpeed * Time.deltaTime * rotationDirection;

        if (Mathf.Abs(currentRotationAngle) >= maxRotationAngle)
        {
            rotationDirection *= -1f;
        }

        bubbleRect.localRotation = Quaternion.Euler(0, 0, currentRotationAngle);
    }

    private void PopBubble()
    {
        isPopped = true;
        bubbleImage.sprite = popSprite;
        StartCoroutine(RegenerateBubble());
    }

    private IEnumerator Shake()
    {
        Vector3 originalPosition = bubbleRect.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);

            bubbleRect.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;

            yield return null;
        }

        bubbleRect.localPosition = originalPosition;
    }

    private IEnumerator RegenerateBubble()
    {
        yield return new WaitForSeconds(regenDelay);

        for (int i = 0; i < regenSprites.Length; i++)
        {
            bubbleImage.sprite = regenSprites[i];
            yield return new WaitForSeconds(regenFrameDuration);
        }

        bubbleImage.sprite = bubbleSprite;
        clickCount = 0;
        isPopped = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        bubbleRect.localPosition = startPosition;
    }

    private void HoverShake()
    {
        Vector3 originalPosition = startPosition;
        float offsetX = Random.Range(-hoverShakeMagnitude, hoverShakeMagnitude);
        float offsetY = Random.Range(-hoverShakeMagnitude, hoverShakeMagnitude);

        bubbleRect.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);
    }
}
