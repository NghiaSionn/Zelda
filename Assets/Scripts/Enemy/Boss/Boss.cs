using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Enemy")]
    public EnemyState currentState = EnemyState.idle;
    public EnemyType enemyType;

    [Header("Heatlh")]
    public SignalSender HealthSignal;
    public FloatValue maxHealth;
    public float health;
    public string enemyName;
    public int baseAttack;
    public float moveSpeed;

    private Animator anim;

    [Header("Rớt đồ")]
    public LootTable thisLoot;

    [Header("Ui Boss")]
    public GameObject bossUI;

    public SpawnArea spawnArea;

    public void Awake()
    {
        anim = GetComponent<Animator>();
        health = maxHealth.initiaValue;
    }


    public void Knock(Rigidbody2D myRigibody, float knockTime, float damage)
    {
        if (gameObject.activeInHierarchy)
        {
            currentState = EnemyState.stagger;
            StartCoroutine(KnockCo(myRigibody, knockTime));
            TakeDamage(damage);
        }
    }

    private void MakeLoot()
    {
        if (thisLoot != null)
        {
            GameObject current = thisLoot.LootPowerup();

            if (current != null)
            {
                Instantiate(current.gameObject, transform.position, Quaternion.identity);
            }
        }
    }

    protected virtual void TakeDamage(float damage)
    {
        
        maxHealth.RuntimeValue -= damage;
        HealthSignal.Raise();
        
        if(maxHealth.RuntimeValue > 0)
        {
            StartCoroutine(Hurt());
        }

        if (maxHealth.RuntimeValue < 0)
        {
            StartCoroutine(Hurt());
            Exp();
            StartCoroutine(UIBoss());
            MakeLoot();
            
            spawnArea.EnemyDied(this.gameObject);
           
        }
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
                myRigibody.velocity = Vector2.zero;
                currentState = EnemyState.idle;
                myRigibody.velocity = Vector2.zero;
            }
        }
    }

    private IEnumerator Hurt()
    {
        anim.SetBool("hurt", true);
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("hurt", false);
        yield return null;
    }

    private IEnumerator UIBoss()
    {
        bossUI.SetActive(false);
        yield return new WaitForSeconds(3f);
    }
}
