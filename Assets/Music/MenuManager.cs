using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour

{
    public AudioFader audioFader;


    void Start()
    {
        // Start playing and fade in to maximum volume
        audioFader.FadeIn();
    }

    void Update()
    {
    }
}