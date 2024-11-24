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
        if (this.gameObject.CompareTag("Skill"))
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

                // Thêm xử lý cho Player khi bị skill đánh trúng
                if (other.gameObject.CompareTag("Player"))
                {
                    var player = other.GetComponent<PlayerMovement>();
                    if (player != null && player.currentState != PlayerState.stagger)
                    {
                        CameraShakeManager.instance.CameraShake(impulseSource);
                        player.currentState = PlayerState.stagger;
                        player.Knock(knockTime, damage);
                    }
                }
            }
        }

        // Xử lý chung cho enemy và Player
        if (other.gameObject.CompareTag("enemy") || other.gameObject.CompareTag("Player"))
        {
            // Ngăn kẻ địch giết nhau
            if (other.gameObject.CompareTag("enemy") && gameObject.CompareTag("enemy")) return;

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
                    var player = other.GetComponent<PlayerMovement>();
                    if (player != null && player.currentState != PlayerState.stagger)
                    {
                        CameraShakeManager.instance.CameraShake(impulseSource);
                        player.currentState = PlayerState.stagger;
                        player.Knock(knockTime, damage);
                    }
                }
            }
        }

        // Đánh tài nguyên
        if (other.gameObject.CompareTag("Ores"))
        {
            other.GetComponent<Ore>().MineOre(1);
            Debug.Log("Hit resource");
        }
    }

}
