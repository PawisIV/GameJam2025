using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleMenu : MonoBehaviour
{
    [Header("Bubble Sprites")]
    public Sprite bubbleSprite; // The default bubble sprite
    public Sprite popSprite;    // The sprite to show when the bubble pops

    [Header("Shake Settings")]
    public float shakeDuration = 0.2f; // Duration of the shake
    public float shakeMagnitude = 10f; // Magnitude of the shake effect

    [Header("Pop Settings")]
    public float regenDelay = 1f; // Delay before the bubble regenerates

    [Header("Movement Settings")]
    public float movementRadius = 50f; // Radius of the circular/oscillatory movement
    public float movementSpeed = 1f;   // Speed of the movement

    [Header("Scaling Settings")]
    public float minScale = 0.8f;      // Minimum scale
    public float maxScale = 1.2f;      // Maximum scale
    public float scalingSpeed = 2f;    // Speed of scaling

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;  // Speed of rotation (degrees per second)
    public float maxRotationAngle = 15f; // Maximum angle to rotate left and right

    private Image bubbleImage;    // Reference to the Image component
    private RectTransform bubbleRect; // Reference to the RectTransform
    private int clickCount = 0;   // Track how many times the bubble is clicked
    private bool isPopped = false; // Track if the bubble is in the popped state
    private Vector3 startPosition; // Original position of the bubble
    private float savedIndex;      // Used to offset movement for each bubble instance
    private float currentRotationAngle = 0f; // Current angle for rotation
    private float rotationDirection = 1f; // Direction of rotation (1 or -1)

    private void Start()
    {
        // Initialize components
        bubbleImage = GetComponent<Image>();
        bubbleRect = GetComponent<RectTransform>();

        if (bubbleImage == null)
        {
            Debug.LogError("This script must be attached to a GameObject with an Image component!");
            return;
        }

        // Set the initial sprite
        bubbleImage.sprite = bubbleSprite;

        // Save the original position for movement calculations
        startPosition = bubbleRect.localPosition;

        // Save a random offset to make each bubble's movement unique
        savedIndex = Random.Range(0f, Mathf.PI * 2f); // Randomize between 0 and 2π

        // Set an initial random rotation direction
        rotationDirection = Random.Range(0, 2) == 0 ? 1f : -1f;
    }

    private void Update()
    {
        if (!isPopped)
        {
            MoveBubble();
            ScaleBubble();
            RotateBubble();
        }
    }

    public void OnBubbleClick()
    {
        if (isPopped)
            return;

        clickCount++;

        if (clickCount >= 3) // Adjust the number of clicks needed to pop the bubble
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
        // Calculate sine and cosine for circular/oscillatory movement
        float sine = Mathf.Sin(Time.time * movementSpeed + savedIndex);
        float cosine = Mathf.Cos(Time.time * movementSpeed + savedIndex);

        // Update position using sine and cosine
        bubbleRect.localPosition = startPosition + new Vector3(sine * movementRadius, cosine * movementRadius, 0);
    }

    private void ScaleBubble()
    {
        // Calculate scale using PingPong for smooth scaling up and down
        float scale = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(Time.time * scalingSpeed, 1));
        bubbleRect.localScale = new Vector3(scale, scale, 1);
    }

    private void RotateBubble()
    {
        // Rotate the bubble left and right within the maxRotationAngle range
        currentRotationAngle += rotationSpeed * Time.deltaTime * rotationDirection;

        if (Mathf.Abs(currentRotationAngle) >= maxRotationAngle)
        {
            rotationDirection *= -1f; // Reverse direction when reaching the limit
        }

        bubbleRect.localRotation = Quaternion.Euler(0, 0, currentRotationAngle);
    }

    private void PopBubble()
    {
        isPopped = true;
        bubbleImage.sprite = popSprite; // Change to the pop sprite
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

        // Reset the bubble to its original state
        bubbleImage.sprite = bubbleSprite;
        clickCount = 0;
        isPopped = false;
    }
}
