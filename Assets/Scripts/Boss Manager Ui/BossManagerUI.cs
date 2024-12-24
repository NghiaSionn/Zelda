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

    [Header("Hình ảnh")]
    public Image bossImage;

    [Header("Data")]
    public EnemyInfor bossInfo; 

    void Start()
    {
        bossUI.SetActive(false); 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bossUI.SetActive(true);
            UpdateBossUI();
        }
    }

    private void UpdateBossUI()
    {
        
        if (bossInfo != null)
        {
            bossNameText.text = bossInfo.name; 
            bossImage.sprite = bossInfo.image; 
        }
    }
}
