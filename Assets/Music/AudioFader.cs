using UnityEngine;

public class AudioFader : MonoBehaviour
{
    public AudioSource audioSource; // The audio source to fade
    public float fadeDuration = 2.0f; // Duration of the fade in seconds
    public float maxVolume = 1.0f; // Maximum volume to fade to

    // Fades in the audio over the given duration to the specified max volume
    public void FadeIn()
    {
        StartCoroutine(FadeAudio(0, maxVolume, fadeDuration));
    }

    // Coroutine for fading audio in
    private System.Collections.IEnumerator FadeAudio(float startVolume, float targetVolume, float duration)
    {
        float currentTime = 0;

        // Ensure the audio source is playing
        if (!audioSource.isPlaying)
        {
            audioSource.volume = 0;
            audioSource.Play();
        }

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            audioSource.volume = newVolume;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
