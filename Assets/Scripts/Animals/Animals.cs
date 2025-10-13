using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animals : MonoBehaviour
{
    [Header("Trạng thái")]
    public EnemyState currentState = EnemyState.idle;
    [Header("Health")]
    public FloatValue maxHealth;
    public float health;
    [Header("Rớt đồ")]
    public LootTable thisLoot;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        health = maxHealth.initiaValue;
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogWarning($"{name} chưa có BoxCollider2D — vui lòng thêm để xử lý va chạm.");
        }
    }

    public void Knock(Rigidbody2D myRigibody, float knockTime, float damage)
    {
        if (gameObject.activeInHierarchy)
        {
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
        health -= damage;
        if (health > 0)
        {
            StartCoroutine(Hurt());
        }
        else
        {
            StartCoroutine(Hurt());
            Exp();
            MakeLoot();
        }
    }

    public int Exp()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        int expToGive = thisLoot.GetExp();
        player.AddExp(expToGive);
        return expToGive;
    }

    private IEnumerator Hurt()
    {
        anim.SetBool("hurt", true);
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("hurt", false);
        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Animal"))
        {
            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Animal"))
        {
            if (boxCollider != null)
            {
                boxCollider.isTrigger = false;
            }
        }
    }
}