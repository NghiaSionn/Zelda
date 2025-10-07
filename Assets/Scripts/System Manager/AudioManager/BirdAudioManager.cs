using System.Collections;
using UnityEngine;

public class BirdAudioManager : MonoBehaviour
{
    [Header("Âm thanh hành vi")]
    public AudioClip[] idleSounds;      
    public AudioClip[] takeOffSounds;    
    public AudioClip[] flyingSounds;     

    [Header("Cấu hình phát âm thanh")]
    public float idleSoundInterval = 6f;     

    private AudioSource audioSource;
    private Transform player;
    private BirdBehavior bird;             

    private bool isFlying => bird != null && bird.isFlying;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        bird = GetComponent<BirdBehavior>();
        if (bird == null)
            Debug.LogWarning("BirdAudioManager: Không tìm thấy BirdBehavior trên object này!");
    }

    private void Start()
    {
        //StartCoroutine(PlayIdleOrFlyingLoop());
    }

    public void PlayTakeOffSound()
    {
        PlayRandomSound(takeOffSounds);
    }

    //private IEnumerator PlayIdleOrFlyingLoop()
    //{
    //    while (true)
    //    {
    //        if (!isFlying)
    //        {
                
    //            PlayRandomSound(idleSounds);
    //        }
    //        else
    //        {
    //            Debug.Log("Chim đang bay, phát âm thanh bay");
    //            PlayRandomSound(flyingSounds);
    //        }
    //    }
    //}


    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0 || audioSource.isPlaying) return;

        int index = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[index]);
    }
}
