using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyState
{
    idle,
    walk,
    attack,
    stagger,
    chase,
    wander,
    retreat,
    death
}

public enum EnemyType
{
    melee,
    ranged,
    none
}

[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    public EnemyState currentState = EnemyState.idle;
    public EnemyType enemyType;

    [Header("Health")]
    public FloatValue maxHealth;
    public float health;
    public int baseAttack;
    public float moveSpeed;

    protected Animator anim;

    [Header("Rớt đồ")]
    public LootTable thisLoot;

    public GameObject enemyPrefab; 
    public SpawnArea spawnArea;

    [Header("Audio Settings")]
    public AudioClip[] idleSounds;       
    public AudioClip[] walkSounds;       
    public AudioClip[] attackSounds;     
    public AudioClip[] hurtSounds;       
    public AudioClip[] deathSounds;         
    public float idleSoundInterval = 5f; 
    public float walkSoundInterval = 0.4f; 
    public float maxHearingDistance = 10f; 

    protected AudioSource audioSource;
    protected bool isMoving = false;
    private Transform playerTransform;

    protected virtual void OnEnable()
    {
        health = maxHealth.initiaValue;
        anim = GetComponent<Animator>();

        if (spawnArea == null)
            spawnArea = GetComponentInParent<SpawnArea>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.spatialBlend = 1f; 
            audioSource.playOnAwake = false;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;

        StartCoroutine(PlayIdleSoundsRoutine());
        StartCoroutine(PlayWalkSoundsRoutine());
    }

    protected virtual void Update()
    {
        AdjustVolumeByDistance();
    }

    private void AdjustVolumeByDistance()
    {
        if (playerTransform == null || audioSource == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        float volume = Mathf.Clamp01(1 - (distance / maxHearingDistance));
        audioSource.volume = volume;
    }

    private IEnumerator PlayIdleSoundsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(idleSoundInterval);
            PlayRandomSound(idleSounds);
        }
    }

    private IEnumerator PlayWalkSoundsRoutine()
    {
        while (true)
        {
            if (isMoving && walkSounds != null && walkSounds.Length > 0)
            {
                PlayRandomSound(walkSounds);
                yield return new WaitForSeconds(walkSoundInterval);
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public virtual void SetMoving(bool moving)
    {
        isMoving = moving;
        if (anim != null)
        {
            anim.SetBool("moving", moving);
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

    private void PlayRandomSound(AudioClip[] clips)
    {
        // Bỏ điều kiện audioSource.isPlaying để tiếng rên rỉ (Hurt/Death) có thể đè đúp lên tiếng bước chân (Walk)
        if (clips == null || clips.Length == 0 || audioSource == null) return;

        int index = Random.Range(0, clips.Length);
        audioSource.PlayOneShot(clips[index]);
    }

    public void Knock(Rigidbody2D myRigibody, float knockTime, float damage)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(KnockCo(myRigibody, knockTime));
            TakeDamage(damage);
        }
    }

    private void MakeLoot()
    {
        if(thisLoot != null)
        {
            GameObject current = thisLoot.LootPowerup();

            if (current != null)
            {
                Instantiate(current.gameObject,transform.position, Quaternion.identity);
            }
        }
    }

    protected virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health > 0)
        {
            StartCoroutine(Hurt());
        }
        else
        {
            Exp();
            MakeLoot();
            StartCoroutine(DeadAnim());
        }
    }

    IEnumerator DeadAnim()
    {
        anim.SetBool("dead", true);

        // Phát ngẫu nhiên 1 âm thanh chết nếu có
        float waitTime = 0.5f; 
        if (deathSounds != null && deathSounds.Length > 0 && audioSource != null)
        {
            int index = Random.Range(0, deathSounds.Length);
            AudioClip clip = deathSounds[index];
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
                waitTime = Mathf.Max(waitTime, clip.length); // Đợi tối thiểu 0.5s hoặc bằng độ dài âm thanh
            }
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Chờ âm thanh và hoạt ảnh hoàn tất
        yield return new WaitForSeconds(waitTime);

        if (spawnArea != null)
        {
            spawnArea.EnemyDied(this.gameObject);
        }
        anim.SetBool("dead", false);
        Destroy(this.gameObject);
    }

    public int Exp()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        int expToGive = thisLoot.GetExp();
        player.AddExp(expToGive);
        return expToGive;
    }

    private IEnumerator KnockCo(Rigidbody2D myRigibody, float knockTime)
    {
        if (myRigibody != null && gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(knockTime);
            if (gameObject.activeInHierarchy)
            {
                myRigibody.linearVelocity = Vector2.zero;
                currentState = EnemyState.idle;
                myRigibody.linearVelocity = Vector2.zero;
            }
        }
    }

    private IEnumerator Hurt()
    {
        PlayHurtSound();

        anim.SetBool("hurt", true);      
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("hurt", false);
        yield return null;
    }
}
