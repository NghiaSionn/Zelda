using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightRenderer : MonoBehaviour
{
    private SpriteRenderer sprite;
    private float baseHeight = 0f;   // cao mặc định
    private float currentHeight = 0f; // thay đổi khi vào nước

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Công thức classic: -y + height
        int order = Mathf.RoundToInt(-(transform.position.y + currentHeight) * 100);
        sprite.sortingOrder = order;
    }

    public void SetHeight(float h)
    {
        currentHeight = h;
    }

    // Ví dụ khi vào vùng nước
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Water"))
        {
            SetHeight(-0.3f); // chìm 0.3 đơn vị
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Water"))
        {
            SetHeight(0f); // trở lại bình thường
        }
    }
}
