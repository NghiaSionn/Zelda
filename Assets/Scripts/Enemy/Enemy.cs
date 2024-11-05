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
    retreat
}

public enum EnemyType
{
    melee,
    ranged
}


public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    public EnemyState currentState = EnemyState.idle;
    public EnemyType enemyType = EnemyType.melee;
    public FloatValue maxHealth;
    public float health;
    public string enemyName;
    public int baseAttack;
    public float moveSpeed;


    public void Awake()
    {
        health = maxHealth.initiaValue;
    }


    public void Knock(Rigidbody2D myRigibody, float knockTime, float damage)
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(KnockCo(myRigibody, knockTime));
            TakeDamage(damage);
        }
    }


    protected virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
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
}
