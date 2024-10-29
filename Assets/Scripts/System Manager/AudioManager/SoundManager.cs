using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource sfx2DSource;

    [SerializeField]
    private AudioMixerGroup sfxMixerGroup; 

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        
        sfx2DSource.outputAudioMixerGroup = sfxMixerGroup;
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            // Tạo một đối tượng tạm thời để phát âm thanh
            GameObject tempAudioSourceObject = new GameObject("TempAudioSource");
            tempAudioSourceObject.transform.position = pos;

            // Thêm AudioSource vào đối tượng tạm thời
            AudioSource tempAudioSource = tempAudioSourceObject.AddComponent<AudioSource>();
            tempAudioSource.clip = clip;
            tempAudioSource.outputAudioMixerGroup = sfxMixerGroup; // Gán Audio Mixer Group cho SFX
            tempAudioSource.Play();

            // Hủy đối tượng khi âm thanh phát xong
            Destroy(tempAudioSourceObject, clip.length);
        }
    }


    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        if (clip != null)
        {
            sfx2DSource.clip = clip;
            sfx2DSource.loop = true;
            sfx2DSource.Play();
        }
    }

    public void StopSound2D(string soundName)
    {
        sfx2DSource.Stop();
    }
}
