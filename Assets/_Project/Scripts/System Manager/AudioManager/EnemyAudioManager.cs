using System.Collections;
using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    public AudioClip[] idleSounds;       
    public AudioClip[] attackSounds;     
    public AudioClip[] hurtSounds;       
    public AudioClip deathSound;         

    private AudioSource audioSource;
    private Transform player;
    private Enemy enemyScript;           

    public float idleSoundInterval = 5f; 
    public float maxHearingDistance = 10f; 

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.spatialBlend = 1f; 
        audioSource.playOnAwake = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyScript = GetComponent<Enemy>(); 
    }

    private void Start()
    {
        StartCoroutine(PlayIdleSounds());
    }

    private void Update()
    {
        AdjustVolumeByDistance();
    }

    private void AdjustVolumeByDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float volume = Mathf.Clamp01(1 - (distance / maxHearingDistance));
        audioSource.volume = volume; // Giảm âm lượng khi xa hơn
    }

    private IEnumerator PlayIdleSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleSoundInterval);
            PlayRandomSound(idleSounds);
        }
    }

    public void PlayAttackSound()
    {
        PlayRandomSound(attackSounds);
    }

    public void PlayHurtSound()
    {
        PlayRandomSound(hurtSounds);
    }

    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
            StartCoroutine(DisableAfterSound());
        }
    }

    private IEnumerator DisableAfterSound()
    {
        yield return new WaitForSeconds(deathSound.length);
        gameObject.SetActive(false); // Tắt quái sau khi phát âm thanh chết
    }

    private void PlayRandomSound(AudioClip[] clips)
    {
        if (clips.Length == 0 || audioSource.isPlaying) return;

        int index = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[index]);
    }
}
