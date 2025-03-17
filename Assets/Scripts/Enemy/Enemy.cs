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

    public GameObject enemyPrefab; 
    public SpawnArea spawnArea;

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
        if (health > 0)
        {
            StartCoroutine(Hurt());
        }
        else
        {
            Exp();
            MakeLoot();
            //spawnArea.EnemyDied(this.gameObject);
            StartCoroutine(DeadAnim());
            //StartCoroutine(spawnArea.RespawnEnemy(this.gameObject));
                                 
        }
    }

    IEnumerator DeadAnim()
    {
        anim.SetTrigger("dead");

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        yield return new WaitForSeconds(0.5f);
        spawnArea.EnemyDied(this.gameObject);
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
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("hurt", false);
        yield return null;
    }

}
