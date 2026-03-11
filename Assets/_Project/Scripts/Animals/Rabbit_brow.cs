using System.Collections;
using UnityEngine;

public class Rabbit_brow : MonoBehaviour
{
    [Header("Cấu hình di chuyển")]
    public float moveSpeed = 1.5f;
    public Vector2 areaSize = new Vector2(5f, 3f);
    public float roamTimeMin = 2f;
    public float roamTimeMax = 4f;

    [Header("Cấu hình nhảy")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;

    private Rigidbody2D rb;
    private Animator anim;

    private Vector2 targetPosition;
    private bool isJumping = false;
    private bool isAtDestination = false;
    private Vector2 lastDirection = Vector2.right; // Hướng mặc định

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;

        StartCoroutine(MoveRandomly());
    }

    void Update()
    {
        // Nếu đang nhảy thì không xử lý idle
        if (isJumping) return;

        // Khi đã tới đích, set anim idle
        float distance = Vector2.Distance(transform.position, targetPosition);
        if (distance <= 0.05f && !isAtDestination)
        {
            isAtDestination = true;
            anim.SetBool("moving", false);
            ChangeAnim(lastDirection); // Giữ hướng idle
        }
    }

    IEnumerator MoveRandomly()
    {
        while (true)
        {
            // Random điểm trong vùng di chuyển
            Vector2 randomOffset = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );
            targetPosition = (Vector2)transform.position + randomOffset;
            isAtDestination = false;

            // Nhảy đến vị trí mới
            yield return StartCoroutine(JumpToTarget(targetPosition));

            // Chờ một khoảng thời gian ngẫu nhiên trước khi di chuyển tiếp
            float waitTime = Random.Range(roamTimeMin, roamTimeMax);
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator JumpToTarget(Vector2 target)
    {
        isJumping = true;
        anim.SetBool("moving", true);

        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        // Tính hướng di chuyển trước khi nhảy
        Vector2 direction = (target - startPosition).normalized;
        ChangeAnim(direction);

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            // Đường cong parabol
            float xProgress = Mathf.Clamp01(t);
            float yOffset = jumpHeight * (4f * xProgress * (1f - xProgress));

            Vector2 newPosition = Vector2.Lerp(startPosition, target, t);
            newPosition.y += yOffset;

            rb.MovePosition(newPosition);
            yield return null;
        }

        // Đảm bảo cập nhật chính xác khi kết thúc
        rb.MovePosition(target);
        isJumping = false;
        isAtDestination = true;
        anim.SetBool("moving", false);
        ChangeAnim(direction);
        lastDirection = direction;
    }

    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
        lastDirection = setVector;
    }

    private void ChangeAnim(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            direction = lastDirection;
        }

        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY || Mathf.Approximately(absX, absY))
            SetAnimFloat(direction.x > 0 ? Vector2.right : Vector2.left);
        else
            SetAnimFloat(direction.y > 0 ? Vector2.up : Vector2.down);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.3f);
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0));
    }
}
