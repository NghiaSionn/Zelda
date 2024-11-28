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


public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    public EnemyState currentState = EnemyState.idle;
    public EnemyType enemyType;

    [Header("Heatlh")]
    public FloatValue maxHealth;
    public float health;
    public int baseAttack;
    public float moveSpeed;

    private Animator anim;

    [Header("Rớt đồ")]
    public LootTable thisLoot;

    public void Awake()
    {
        health = maxHealth.initiaValue;
        anim = GetComponent<Animator>();
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
        if(health > 0)
        {
            StartCoroutine(Hurt());
        }
        if (health <= 0)
        {
            StartCoroutine(Hurt());
            MakeLoot();
            StartCoroutine(Dead());
            this.gameObject.SetActive(false);
            
        }
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
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("hurt", false);
        yield return null;
    }

    private IEnumerator Dead()
    {
        anim.SetBool("dead",true);
        yield return new WaitForEndOfFrame();
        this.gameObject.SetActive(false);
    }
}
