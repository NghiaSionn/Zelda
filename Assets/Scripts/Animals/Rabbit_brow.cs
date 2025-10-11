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
        if (isJumping) return;

        float distance = Vector2.Distance(transform.position, targetPosition);
        if (distance <= 0.1f && !isAtDestination)
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
            // Random điểm trong phạm vi di chuyển
            Vector2 randomOffset = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );
            targetPosition = (Vector2)transform.position + randomOffset;
            isAtDestination = false;

            yield return StartCoroutine(JumpToTarget(targetPosition));

            // Đợi 1 lúc rồi đi tiếp
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

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            // Đường cong nhảy parabol
            float xProgress = Mathf.Clamp01(t);
            float yOffset = jumpHeight * (4f * xProgress * (1f - xProgress));

            Vector2 newPosition = Vector2.Lerp(startPosition, target, t);
            newPosition.y += yOffset;

            // Hướng di chuyển chính xác (dựa theo newPosition)
            Vector2 direction = (newPosition - (Vector2)transform.position).normalized;
            if (direction.sqrMagnitude > 0.01f)
                ChangeAnim(direction);

            rb.MovePosition(newPosition);
            yield return null;
        }

        rb.MovePosition(target);
        anim.SetBool("moving", false);
        isJumping = false;
        isAtDestination = true;
    }

    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
        lastDirection = setVector; // Lưu hướng cuối
    }

    private void ChangeAnim(Vector2 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY || Mathf.Approximately(absX, absY))
            SetAnimFloat(direction.x > 0 ? Vector2.right : Vector2.left);
        else
            SetAnimFloat(direction.y > 0 ? Vector2.up : Vector2.down);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0));
    }
}
