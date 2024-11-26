using System.Collections;
using UnityEngine;

public class Test_Rigidbody2D : MonoBehaviour
{
    public Rigidbody2D rb;
    public float roamTimeMin = 2f;
    public float roamTimeMax = 5f;
    public Vector2 areaSize = new Vector2(10f, 10f);
    public float moveSpeed = 2f;
    public Animator anim;

    private Vector2 targetPosition;
    private bool isAtDestination = false;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (anim == null)
            anim = GetComponent<Animator>();

        StartCoroutine(MoveRandomly());
    }

    void Update()
    {
        // Tính toán hướng di chuyển
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Raycast để kiểm tra chướng ngại vật phía trước
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, LayerMask.GetMask("Obstacle"));

        if (hit.collider != null) // Nếu va chạm với vật cản
        {
            Debug.Log("Đụng vật cản: " + hit.collider.name);
            ChooseNewTarget(); // Chọn điểm mới
        }
        else
        {
            // Di chuyển bình thường
            rb.velocity = direction * moveSpeed;
            anim.SetBool("moving", true);
            changeAnim(direction);
        }

        // Di chuyển nhân vật
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            rb.velocity = direction * moveSpeed;
            anim.SetBool("moving", true);
            changeAnim(direction);
        }
        else
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("moving", false);
            isAtDestination = true;
        }
    }

    IEnumerator MoveRandomly()
    {
        while (true)
        {
            // Tạo vị trí ngẫu nhiên trong vùng chỉ định
            Vector2 randomPosition = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            targetPosition = (Vector2)transform.position + randomPosition;

            // Chờ cho đến khi đến được đích hoặc hết thời gian chờ
            isAtDestination = false;
            float waitTime = Random.Range(roamTimeMin, roamTimeMax);
            yield return new WaitUntil(() => isAtDestination || waitTime <= 0f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
    }

    public void changeAnim(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
                SetAnimFloat(Vector2.right);
            else if (direction.x < 0)
                SetAnimFloat(Vector2.left);
        }
        else
        {
            if (direction.y > 0)
                SetAnimFloat(Vector2.up);
            else if (direction.y < 0)
                SetAnimFloat(Vector2.down);
        }

        Debug.Log("moveX: " + direction.x + ", moveY: " + direction.y);
    }

    void ChooseNewTarget()
    {
        Vector2 randomPosition = new Vector2(
            Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
            Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
        );
        targetPosition = (Vector2)transform.position + randomPosition;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Interactive")) 
        {
            Debug.Log("Va chạm với vật cản: " + collision.collider.name);
            ChooseNewTarget(); // Chọn một điểm mới để né vật cản
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0));
    }
}
