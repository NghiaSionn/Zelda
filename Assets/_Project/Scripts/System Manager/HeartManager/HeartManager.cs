using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    public Image[] hearts; // Mảng các hình ảnh tim
    public Sprite fullHearts;
    public Sprite halfFullHeart;
    public Sprite emptyHeart;
    public FloatValue heartContainers; // Số lượng container tim tối đa
    public FloatValue playerCurrentHealth; // Máu hiện tại của người chơi

    void Start()
    {
        InitHearts();
    }

    public void InitHearts()
    {
        // Duyệt qua tất cả các tim trong mảng hearts
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < heartContainers.initiaValue)
            {
                // Hiển thị các tim trong phạm vi container và đặt full
                hearts[i].gameObject.SetActive(true);
                hearts[i].sprite = fullHearts;
            }
            else
            {
                // Ẩn các tim vượt quá số lượng container
                hearts[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateHearts()
    {
        // Tính số tim dựa trên máu hiện tại (mỗi tim = 2 máu)
        float tempHealth = playerCurrentHealth.RuntimeValue / 2f;

        // Duyệt qua tất cả các tim trong mảng hearts
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < heartContainers.initiaValue)
            {
                // Hiển thị tim trong phạm vi container
                hearts[i].gameObject.SetActive(true);

                if (i <= tempHealth - 1)
                {
                    // Full Heart
                    hearts[i].sprite = fullHearts;
                }
                else if (i >= tempHealth)
                {
                    // Empty Heart
                    hearts[i].sprite = emptyHeart;
                }
                else
                {
                    // Half Full Heart
                    hearts[i].sprite = halfFullHeart;
                }
            }
            else
            {
                // Ẩn các tim vượt quá số lượng container
                hearts[i].gameObject.SetActive(false);
            }
        }
    }
}