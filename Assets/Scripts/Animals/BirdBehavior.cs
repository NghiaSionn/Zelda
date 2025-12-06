using System.Collections;
using UnityEngine;

public class BirdBehavior : MonoBehaviour
{
    [Header("Cấu hình di chuyển mặt đất")]
    public float moveSpeed = 1.5f;
    public Vector2 areaSize = new Vector2(5f, 3f);
    public float roamTimeMin = 2f;
    public float roamTimeMax = 4f;
    [Header("Cấu hình nhảy")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;
    [Header("Cấu hình bay lên trời")]
    private float flyTime;
    private float flySpeed;
    public float flyAngle = 45f;
    [Header("Cấu hình ăn")]
    private float eatDurationMin = 1f;
    private float eatDurationMax = 2f;
    private float eatChance = 0.2f;
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private BirdAudioManager birdAudioManager;
    private Vector2 targetPosition;
    public bool isFlying = false;
    private bool isAtDestination = false;
    private bool isEating = false;
    private Vector2 lastDirection;
    private SpriteRenderer spriteRenderer;

    [Header("Cấu hình spawn")]
    public bool spawnFromSky = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        birdAudioManager = GetComponent<BirdAudioManager>();
        rb.freezeRotation = true;
        flyTime = Random.Range(10f, 50f);
        flySpeed = Random.Range(8f, 15f);
        lastDirection = Vector2.right;

        if (spawnFromSky)
        {
            StartCoroutine(FlyInFromSky());
        }
        else
        {
            StartCoroutine(MoveRandomly());
            StartCoroutine(HandleFlying());
        }
    }

    void Update()
    {
        if (isFlying || isEating)
        {
            if (isAtDestination || isEating)
            {
                ChangeAnim(lastDirection); 
            }
            return;
        }

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetPosition);
        if (distance > 0.1f)
        {
            // Đang di chuyển, để coroutine xử lý
        }
        else
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("moving", false);
            isAtDestination = true;
            ChangeAnim(lastDirection); 
        }
    }

    IEnumerator MoveRandomly()
    {
        while (!isFlying)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );
            targetPosition = (Vector2)transform.position + randomPosition;
            isAtDestination = false;
            yield return StartCoroutine(MoveToTarget(targetPosition));
            yield return StartCoroutine(HandleEating());
            if (!isFlying)
            {
                float waitTime = Random.Range(roamTimeMin, roamTimeMax);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    IEnumerator MoveToTarget(Vector2 target)
    {
        anim.SetBool("moving", true);
        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;
        Vector2 initialDirection = (target - startPosition).normalized; 
        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;
            float xProgress = Mathf.Clamp01(t);
            float yOffset = jumpHeight * (4f * xProgress * (1f - xProgress));
            Vector2 newPosition = Vector2.Lerp(startPosition, target, t);
            newPosition.y += yOffset;
            rb.MovePosition(newPosition);
            ChangeAnim(initialDirection); 
            yield return null;
        }
        rb.MovePosition(target);
        isAtDestination = true;
        anim.SetBool("moving", false);
        ChangeAnim(initialDirection); 
        lastDirection = initialDirection; 
    }

    IEnumerator HandleEating()
    {
        if (Random.value < eatChance && !isFlying)
        {
            isEating = true;
            anim.SetBool("eating", true);
            rb.velocity = Vector2.zero;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            float eatDuration = Random.Range(eatDurationMin, eatDurationMax);
            yield return new WaitForSeconds(eatDuration);
            anim.SetBool("eating", false);
            rb.gravityScale = originalGravity;
            isEating = false;
            ChangeAnim(lastDirection); 
        }
    }

    IEnumerator HandleFlying()
    {
        yield return new WaitForSeconds(flyTime);
        boxCollider.enabled = false;
        isFlying = true;
        anim.SetBool("flying",true);
        spriteRenderer.sortingLayerName = "Item";
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        birdAudioManager?.PlayFlyingSound();
        bool facingRight = Mathf.Approximately(transform.eulerAngles.y, 0f);
        float angle = flyAngle * Mathf.Deg2Rad;
        Vector2 flyDir = facingRight
            ? new Vector2(Mathf.Sin(angle), Mathf.Cos(angle))
            : new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
        rb.velocity = flyDir * flySpeed;
        Destroy(gameObject, 15f);
        
    }

    IEnumerator FlyInFromSky()
    {
        isFlying = true;
        isEating = false;
        anim.SetBool("flying", true);
        spriteRenderer.sortingLayerName = "Item";

        if (boxCollider != null)
            boxCollider.enabled = false;

        // 🔹 Lưu vị trí spawn gốc (từ SpawnArea)
        Vector2 groundPos = transform.position;

        // 🔹 Đặt chim lên cao trước
        Vector2 startPos = groundPos + new Vector2(
            Random.Range(-3f, 3f),   // lệch ngang
            Random.Range(8f, 12f)    // bay cao
        );
        transform.position = startPos;

        // 🔹 Mục tiêu: hạ xuống gần vị trí spawn ban đầu
        Vector2 targetPos = groundPos + new Vector2(
            Random.Range(-1f, 1f),
            0f  // Hạ cánh đúng độ cao spawn
        );

        float descendTime = Random.Range(2f, 3f);
        float elapsed = 0f;

        while (elapsed < descendTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / descendTime;
            transform.position = Vector2.Lerp(startPos, targetPos, t);

            // Hướng bay dựa trên hướng di chuyển
            Vector2 flyDirection = (targetPos - startPos).normalized;
            ChangeAnim(flyDirection);

            yield return null;
        }

        transform.position = targetPos;
        birdAudioManager?.PlayTakeOffSound();
        anim.SetBool("flying", false);
        isFlying = false;

        if (spriteRenderer != null)
            spriteRenderer.sortingLayerName = "Player";

        if (boxCollider != null)
            boxCollider.enabled = true;

        StartCoroutine(MoveRandomly());
        StartCoroutine(HandleFlying());
    }


    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
        lastDirection = setVector; 
    }

    private void ChangeAnim(Vector2 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);
        if (absX > absY || Mathf.Approximately(absX, absY))
        {
            SetAnimFloat(direction.x > 0 ? Vector2.right : Vector2.left);
        }
        else
        {
            SetAnimFloat(direction.y > 0 ? Vector2.up : Vector2.down);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0));
    }
}