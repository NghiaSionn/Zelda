using System.Collections;
using UnityEngine;

public class BirdAudioManager : MonoBehaviour
{
    [Header("Âm thanh hành vi")]
    public AudioClip[] idleSounds;
    public AudioClip[] takeOffSounds;
    public AudioClip[] flyingSounds;
    [Header("Cấu hình phát âm thanh")]
    public float idleSoundInterval; 
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        idleSoundInterval = Random.Range(5f, 10f);
    }

    private void Start()
    {
        StartCoroutine(PlayIdle());
    }

    public void PlayTakeOffSound()
    {
        PlayRandomSound(takeOffSounds);
    }

    public void PlayFlyingSound()
    {
        PlayRandomSound(flyingSounds);
    }

    private IEnumerator PlayIdle()
    {      
        PlayRandomSound(idleSounds);
        yield return new WaitForSeconds(idleSoundInterval);   
    }

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0 || audioSource.isPlaying) return;
        int index = Random.Range(0, clips.Length);
        if (audioSource != null && clips[index] != null)
        {
            audioSource.PlayOneShot(clips[index]);
            //Debug.Log($"Playing sound: {clips[index].name} at {Time.time}");
        }
    }
}