using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit_brow : MonoBehaviour
{
    public Rigidbody2D rb; // Rigidbody2D của đối tượng
    public float moveSpeed = 3f; // Tốc độ di chuyển
    public float roamTimeMin = 2f; // Thời gian di chuyển ngẫu nhiên (min)
    public float roamTimeMax = 5f; // Thời gian di chuyển ngẫu nhiên (max)
    public Vector2 areaSize = new Vector2(10f, 10f); // Kích thước khu vực di chuyển (X và Y)
    public Animator anim;

    private Vector2 targetPosition;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        // Bắt đầu coroutine di chuyển ngẫu nhiên
        StartCoroutine(MoveRandomly());
    }

    IEnumerator MoveRandomly()
    {
        while (true)
        {
            // Chọn điểm ngẫu nhiên trong khu vực
            Vector2 randomDirection = new Vector2(Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                                                  Random.Range(-areaSize.y / 2f, areaSize.y / 2f));

            targetPosition = (Vector2)transform.position + randomDirection;

            // Debug: Hiển thị vị trí ngẫu nhiên
            Debug.Log($"Target Position: {targetPosition}");
            DrawTargetPosition(targetPosition); // Vẽ điểm mục tiêu

            // Di chuyển tới điểm ngẫu nhiên trong khu vực
            while ((targetPosition - (Vector2)transform.position).magnitude > 0.1f)
            {
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                rb.MovePosition((Vector2)transform.position + direction * moveSpeed * Time.deltaTime);

                // Thay đổi hoạt ảnh tùy theo hướng di chuyển
                changeAnim(direction);

                yield return null;
            }

            rb.velocity = Vector2.zero;
            anim.SetBool("moving", false); // Dừng hoạt ảnh di chuyển khi đến điểm

            // Đợi thời gian ngẫu nhiên trước khi di chuyển tới điểm khác
            float waitTime = Random.Range(roamTimeMin, roamTimeMax);
            yield return new WaitForSeconds(waitTime);
        }
    }

    // Hàm vẽ điểm mục tiêu bằng Sphere
    private void DrawTargetPosition(Vector2 position)
    {
        Debug.DrawLine(transform.position, position, Color.green, 2f); // Vẽ đường đến mục tiêu
        Debug.DrawRay(position, Vector2.up * 0.5f, Color.red, 2f); // Vẽ ray đỏ tại điểm mục tiêu
    }

    // Hàm respawn lại đối tượng sau khi chết
    public void Respawn(Vector2 spawnPosition)
    {
        transform.position = spawnPosition;
        StartCoroutine(MoveRandomly());
    }

    // Debug khu vực di chuyển (Hiển thị khu vực di chuyển bằng hình chữ nhật)
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.2f); // Màu đỏ mờ cho khu vực
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0)); // Vẽ khu vực
    }

    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
    }

    // Hàm thay đổi hoạt ảnh dựa trên hướng di chuyển
    public void changeAnim(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                SetAnimFloat(Vector2.right);
            }
            else if (direction.x < 0)
            {
                SetAnimFloat(Vector2.left);
            }
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0)
            {
                SetAnimFloat(Vector2.up);
            }
            else if (direction.y < 0)
            {
                SetAnimFloat(Vector2.down);
            }
        }
        anim.SetBool("moving", true); // Bật hoạt ảnh di chuyển khi đang di chuyển
    }
}
