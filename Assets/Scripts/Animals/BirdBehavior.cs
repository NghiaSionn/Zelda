using System.Collections;
using UnityEngine;

public class BirdBehavior : MonoBehaviour
{
    [Header("Cấu hình di chuyển mặt đất")]
    public float moveSpeed = 1.5f;
    public Vector2 areaSize = new Vector2(5f, 3f);
    public float roamTimeMin = 2f;
    public float roamTimeMax = 4f;

    [Header("Cấu hình bay lên trời")]
    public float flyTime;
    public float flySpeed;
    public float flyAngle = 45f;

    [Header("Cấu hình ăn")]
    public float eatDurationMin = 2f; // thời gian ăn tối thiểu
    public float eatDurationMax = 4f; // thời gian ăn tối đa
    public float eatChance = 0.5f;    // 50% cơ hội ăn

    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private BirdAudioManager birdAudioManager;

    private Vector2 targetPosition;
    public bool isFlying = false;
    private bool isAtDestination = false;
    private bool isEating = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        birdAudioManager = GetComponent<BirdAudioManager>();
        rb.freezeRotation = true;

        // Random thời gian và tốc độ bay
        flyTime = Random.Range(10f, 30f);
        flySpeed = Random.Range(8f, 15f);

        StartCoroutine(MoveRandomly());
        StartCoroutine(HandleFlying());
    }

    void Update()
    {
        if (isFlying || isEating) return; 

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            rb.velocity = direction * moveSpeed;
            anim.SetBool("moving", true);
            ChangeAnim(direction);
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
        while (!isFlying)
        {
            Vector2 randomPosition = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            targetPosition = (Vector2)transform.position + randomPosition;
            isAtDestination = false;

            
            yield return new WaitUntil(() => isAtDestination);

            
            yield return StartCoroutine(HandleEating());

            
            if (!isFlying)
            {
                float waitTime = Random.Range(roamTimeMin, roamTimeMax);
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    IEnumerator HandleEating()
    {
        if (Random.value < eatChance && !isFlying)
        {
            isEating = true;
            anim.SetTrigger("eating");

            
            rb.velocity = Vector2.zero;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f; 

            float eatDuration = Random.Range(eatDurationMin, eatDurationMax);
            yield return new WaitForSeconds(eatDuration);

            rb.gravityScale = originalGravity;
            isEating = false;
            anim.SetBool("moving", false);
        }
    }


    IEnumerator HandleFlying()
    {
        yield return new WaitForSeconds(flyTime);

        boxCollider.enabled = false;
        isFlying = true;
        anim.SetTrigger("flying");
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        birdAudioManager?.PlayFlyingSound();

        bool facingRight = Mathf.Approximately(transform.eulerAngles.y, 0f);
        float angle = flyAngle * Mathf.Deg2Rad;
        Vector2 flyDir = facingRight
            ? new Vector2(Mathf.Sin(angle), Mathf.Cos(angle))
            : new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));

        rb.velocity = flyDir * flySpeed;

        //Debug.Log($"🕊️ Bird flew away after {flyTime:F1}s | speed: {flySpeed:F1}");

        Destroy(gameObject, 30f);
    }

    void OnBecameInvisible()
    {
        if (isFlying)
            Destroy(gameObject);
    }

    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY", setVector.y);
    }

    private void ChangeAnim(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
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
