using System.Collections;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("Đường đi của NPC")]
    [SerializeField] private Transform[] waypoints;

    [Header("Tốc độ di chuyển")]
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D rb;
    private Collider2D npcCollider;
    public int currentWaypointIndex = 0;
    public int targetWaypointIndex = 0;
    private bool isMoving = true;

    private Animator anim;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        npcCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        // Đặt vị trí ban đầu cho NPC tại waypoint đầu tiên
        transform.position = waypoints[currentWaypointIndex].position;
    }

    void Update()
    {
        if (isMoving)
        {
            Move();
        }
    }

    private void Move()
    {
        Vector2 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        changeAnim(direction);
        anim.SetBool("moving", true);

        transform.position = Vector2.MoveTowards(transform.position,
            waypoints[currentWaypointIndex].position,
            moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            anim.SetBool("moving", false);

            if (currentWaypointIndex == targetWaypointIndex)
            {
                StartCoroutine(SelectNextWaypoint());
            }
            else
            {
                if (currentWaypointIndex < targetWaypointIndex)
                {
                    currentWaypointIndex++;
                }
                else
                {
                    currentWaypointIndex--;
                }
            }
        }
    }

    private IEnumerator SelectNextWaypoint()
    {
        isMoving = false;

        yield return new WaitForSeconds(1f);

        do
        {
            targetWaypointIndex = Random.Range(0, waypoints.Length);
        } while (targetWaypointIndex == currentWaypointIndex);

        isMoving = true;
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
            {
                SetAnimFloat(Vector2.right);
            }
            else if (direction.x < 0)
            {
                SetAnimFloat(Vector2.left);
            }
        }
        else
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
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NPC") )
        {
            Debug.Log("Va chạm với NPC khác, tắt collider");
            npcCollider.enabled = false; 
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            Debug.Log("Rời khỏi va chạm với NPC khác, bật collider");
            npcCollider.enabled = true; 
        }
    }
}
