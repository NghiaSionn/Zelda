using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class BossManagerUI : MonoBehaviour
{
    [Header("Giao diện UI")]
    public GameObject bossUI;

    [Header("Tên Boss")]
    public TextMeshProUGUI bossNameText;

    [Header("Data")]
    public EnemyInfor bossInfo; 

    public Animator animator;
    public Boss boss;

    void Start()
    {
        boss = FindAnyObjectByType<Boss>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!boss.isDeath)
            {
                StartCoroutine(Start_Panel());
            }
            else
            {
                StartCoroutine(End_Panel());
            }                    
        }
    }

    private void UpdateBossUI()
    {
        
        if (bossInfo != null)
        {
            bossNameText.text = bossInfo.name; 
        }
    }

    IEnumerator Start_Panel()
    {
        animator.Play("start_panel");
        UpdateBossUI();
        yield return null;
    }
    IEnumerator End_Panel()
    {
        animator.Play("end_panel");
        yield return null;
    }
}
