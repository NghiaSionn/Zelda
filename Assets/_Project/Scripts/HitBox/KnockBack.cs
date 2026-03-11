using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using DG.Tweening; 

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
        // Tính lực đẩy thực tế (Giảm lực về 0 nếu là đòn đánh 1 và 2 của Combo kiếm từ Player)
        float actualThrust = thrust;
        if (this.gameObject.CompareTag("Skill") && (other.CompareTag("enemy") || other.CompareTag("Boss")))
        {
            PlayerMovement actionPlayer = FindObjectOfType<PlayerMovement>();
            if (actionPlayer != null)
            {
                int combo = actionPlayer.GetCurrentAttackCombo();
                if (combo == 1 || combo == 2)
                {
                    actualThrust = 0f;
                }
            }
        }

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
                Vector2 difference = (other.transform.position - transform.position).normalized * actualThrust;
                if (actualThrust > 0f)
                {
                    hit.DOMove(hit.transform.position + new Vector3(difference.x, difference.y, 0), knockTime);
                }

                if (other.gameObject.CompareTag("enemy"))
                {
                    var enemy = other.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        CameraShakeManager.instance.CameraShake(impulseSource);
                        enemy.currentState = EnemyState.stagger;
                        enemy.Knock(hit, knockTime, damage);
                    }

                    var boss = other.GetComponent<Boss>();
                    if (boss != null)
                    {
                        CameraShakeManager.instance.CameraShake(impulseSource);
                        boss.currentState = EnemyState.stagger;
                        boss.Knock(hit, knockTime, damage);
                    }
                }

                if (other.gameObject.CompareTag("Animal"))
                {
                    var animal = other.GetComponent<Animals>();
                    if (animal != null)
                    {
                        CameraShakeManager.instance.CameraShake(impulseSource);
                        animal.currentState = EnemyState.stagger;
                        animal.Knock(hit, knockTime, damage);
                    }
                }

                // Thêm xử lý cho Player khi bị skill đánh trúng
                if (other.gameObject.CompareTag("Player"))
                {
                    var player = other.GetComponent<PlayerMovement>();
                    if (player != null && player.currentState != PlayerState.stagger)
                    {
                        CameraShakeManager.instance.CameraShake(impulseSource);
                        player.Knock(knockTime,damage, difference.normalized * actualThrust);
                    }
                }
            }
        }

        // Xử lý chung cho enemy và Player
        if (other.gameObject.CompareTag("enemy") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Animal"))
        {
            // Ngăn kẻ địch giết nhau
            if (other.gameObject.CompareTag("enemy") && gameObject.CompareTag("enemy")) return;

            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                Vector2 difference = (other.transform.position - transform.position).normalized * actualThrust;
                if (actualThrust > 0f)
                {
                    hit.DOMove(hit.transform.position + new Vector3(difference.x, difference.y, 0), knockTime);
                }

                if (other.gameObject.CompareTag("enemy") && other.isTrigger)
                {
                    var enemy = other.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.currentState = EnemyState.stagger;
                        enemy.Knock(hit, knockTime, damage);
                    }

                    // Xử lý Boss
                    var boss = other.GetComponent<Boss>();
                    if (boss != null)
                    {
                        boss.currentState = EnemyState.stagger;
                        boss.Knock(hit, knockTime, damage);
                    }
                }

                if (other.gameObject.CompareTag("Animal"))
                {
                    var animal = other.GetComponent<Animals>();
                    if (animal != null)
                    {
                        animal.Knock(hit, knockTime, damage);
                    }
                }

                if (other.gameObject.CompareTag("Player"))
                {
                    var player = other.GetComponent<PlayerMovement>();
                    if (player != null && player.currentState != PlayerState.stagger)
                    {
                        CameraShakeManager.instance.CameraShake(impulseSource);
                        player.Knock(knockTime, damage, difference.normalized * actualThrust);
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