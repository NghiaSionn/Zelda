using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animals : MonoBehaviour
{
    [Header("Heatlh")]
    public FloatValue maxHealth;
    public float health;

    [Header("Rớt đồ")]
    public LootTable thisLoot;

    private Animator anim;

    
    private void Awake()
    {
        health = maxHealth.initiaValue;
        anim = GetComponent<Animator>();
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
        if (health <= 0)
        {
            StartCoroutine(Hurt());
            MakeLoot();
            this.gameObject.SetActive(false);
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
