using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public FMODEvents audioData;
    public EventInstance currentBGMusic;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        PlayBGMusic(instance.audioData.floor1);
    }

    void Update()
    {
        if (EnemyManager.instance.currentRoom.hasBeenExplored)
            instance.currentBGMusic.setParameterByName("Intensity", 0);
        else
            instance.currentBGMusic.setParameterByName("Intensity", 1);
    }

    public static void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound, Vector3.zero);
    }

    public static void PlayOneShot(EventReference sound, float delay)
    {
        instance.StartCoroutine(instance.DelayedOneShot(sound, delay));
    }

    public static void PlayBGMusic(EventReference music)
    {
        instance.currentBGMusic = RuntimeManager.CreateInstance(music);
        instance.currentBGMusic.start();
    }

    public static void StopCurrentBGMusic()
    {
        instance.currentBGMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public static FMODEvents GetAudioData()
    {
        return instance.audioData;
    }

    private IEnumerator DelayedOneShot(EventReference sound, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        RuntimeManager.PlayOneShot(sound, Vector3.zero);
    }
}
