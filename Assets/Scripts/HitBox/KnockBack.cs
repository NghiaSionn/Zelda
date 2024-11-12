using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    [Header("KnockBack")]
    public float thrust;
    public float knockTime;
    public float damage;

    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Phá bình 
        if (other.gameObject.CompareTag("breakable") && this.gameObject.CompareTag("Player"))
        {
            other.GetComponent<Pot>().Smash();
        }

        // Giết kẻ địch bằng skill
        if (this.gameObject.CompareTag("Skill") && other.gameObject.CompareTag("enemy"))
        {
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();



            if (hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);

                if (other.gameObject.CompareTag("enemy"))
                {
                    CameraShakeManager.instance.CameraShake(impulseSource);
                    hit.GetComponent<Enemy>().currentState = EnemyState.stagger;
                    other.GetComponent<Enemy>().Knock(hit, knockTime, damage);
                }
            }
        }


        if (other.gameObject.CompareTag("enemy") || other.gameObject.CompareTag("Player"))
        {
            // ngăn kẻ địch giết nhau
            if (other.gameObject.CompareTag("enemy") && gameObject.CompareTag("enemy")) return;

            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                if (other.gameObject.CompareTag("Enemy") && other.isTrigger)
                {
                    hit.GetComponent<Enemy>().currentState = EnemyState.stagger;
                    other.GetComponent<Enemy>().Knock(hit, knockTime, damage);
                }
                if (other.gameObject.CompareTag("Player"))
                {
                    if (other.GetComponent<PlayerMovement>().currentState != PlayerState.stagger)
                    {
                        CameraShakeManager.instance.CameraShake(impulseSource);
                        hit.GetComponent<PlayerMovement>().currentState = PlayerState.stagger;
                        other.GetComponent<PlayerMovement>().Knock(knockTime, damage);
                    }
                }
                if (other.gameObject.CompareTag("Enemy"))
                {
                    var enemy = other.gameObject.GetComponent<Enemy>();
                    enemy.Knock(hit, knockTime, damage);
                }
            }
        }

        if (other.gameObject.CompareTag("Ores"))
        {
            other.GetComponent<Ore>().MineOre(1);
            Debug.Log("Hit resource");
        }
    }
}
