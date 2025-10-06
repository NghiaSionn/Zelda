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
    public float flyTime = 25f;
    public float flySpeed = 5f;
    public float flyAngle = 45f; 

    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private Vector2 targetPosition;
    public bool isFlying = false;
    private bool isAtDestination = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb.freezeRotation = true;

        StartCoroutine(MoveRandomly());
        StartCoroutine(HandleFlying());
    }

    void Update()
    {
        if (isFlying) return;

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
            float waitTime = Random.Range(roamTimeMin, roamTimeMax);
            yield return new WaitUntil(() => isAtDestination);
            yield return new WaitForSeconds(waitTime);
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

        
        bool facingRight = Mathf.Approximately(transform.eulerAngles.y, 0f);

        // Tính vector hướng bay (chéo lên theo hướng mặt)
        float angle = flyAngle * Mathf.Deg2Rad;
        Vector2 flyDir = facingRight
            ? new Vector2(Mathf.Sin(angle), Mathf.Cos(angle))   // Bay chéo phải
            : new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle)); // Bay chéo trái

        
        rb.velocity = flyDir * flySpeed;

        // Tự hủy sau 10s
        Destroy(gameObject, 10f);
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
