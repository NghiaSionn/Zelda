using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyState
{
    idle,
    walk,
    attack,
    stagger
}


public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    public EnemyState currentState;
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
        StartCoroutine(KnockCo(myRigibody, knockTime));
        TakeDamage(damage);
    }


    private void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }


    private IEnumerator KnockCo(Rigidbody2D myRigibody, float knockTime)
    {
        if (myRigibody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigibody.velocity = Vector2.zero;
            currentState = EnemyState.idle;
            myRigibody.velocity = Vector2.zero;
        }
    }
}
