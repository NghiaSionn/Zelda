using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    [Header("KnockBack")]
    public float thrust;
    public float knockTime;
    public float damage;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("breakable") && this.gameObject.CompareTag("Player"))
        {
            other.GetComponent<Pot>().Smash();
        }
        if (other.gameObject.CompareTag("enemy") || other.gameObject.CompareTag("Player"))
        {
            // ngăn kẻ địch giết nhau
            if (other.gameObject.CompareTag("Enemy") && gameObject.CompareTag("Enemy")) return;


            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                if (other.gameObject.CompareTag("enemy") && other.isTrigger)
                {
                    hit.GetComponent<Enemy>().currentState = EnemyState.stagger;
                    other.GetComponent<Enemy>().Knock(hit, knockTime, damage);
                }
                if (other.gameObject.CompareTag("Player"))
                {
                    if (other.GetComponent<PlayerMovement>().currentState != PlayerState.stagger)
                    {
                        hit.GetComponent<PlayerMovement>().currentState = PlayerState.stagger;
                        other.GetComponent<PlayerMovement>().Knock(knockTime, damage);
                    }
                }
            }
        }

        if (other.gameObject.CompareTag("Ores"))
        {
            other.GetComponent<Resource>().MineResource(1);
            Debug.Log("Hit resource");
        }
    }
}
