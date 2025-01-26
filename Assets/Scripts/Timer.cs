using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Reference to the TextMeshPro component
    private float timer = 0f;
    private bool isTimerRunning = true;

    void Update()
    {
        if (!isTimerRunning) return;

        timer += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public float GetElapsedTime()
    {
        return timer; // Return the current timer value in seconds
    }
}
