using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SoundType
{
    SWORD,
    FIREBALL,
    HURT,
    ITEMPICKUP,
    BREAK,
    OPEN,
    CLOSE,
    EXPLOSION,
    RAIN,
    NIGHT
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    private static AudioManager instance;
    private AudioSource audioSource;

    public AudioSource rainAudioSource;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound,float volume)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

    public static void PlayRainSound(float volume)
    {
        if (instance.rainAudioSource.isPlaying) return; 
        AudioClip[] clips = instance.soundList[(int)SoundType.RAIN].Sounds;
        if (clips.Length > 0)
        {
            instance.rainAudioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            instance.rainAudioSource.volume = volume;
            instance.rainAudioSource.Play();
        }
    }

    public static void StopRainSound()
    {
        if (instance.rainAudioSource.isPlaying)
        {
            instance.rainAudioSource.Stop();
        }
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }

    }

#endif
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds;  }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
