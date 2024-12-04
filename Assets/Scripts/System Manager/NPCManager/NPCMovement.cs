using System.Collections;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("Đường đi của NPC")]
    [SerializeField] private Transform[] waypoints;

    [Header("Tốc độ di chuyển")]
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D rb;
    public int currentWaypointIndex = 0; // Điểm hiện tại
    public int targetWaypointIndex = 0; // Điểm ngẫu nhiên được chọn
    private bool isMoving = true;

    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        // Tính toán hướng di chuyển từ vị trí hiện tại đến waypoint kế tiếp
        Vector2 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;

        // Cập nhật hoạt ảnh hướng di chuyển
        changeAnim(direction);

        // Kích hoạt hoạt ảnh di chuyển
        anim.SetBool("moving", true);

        // Di chuyển NPC đến waypoint tiếp theo
        transform.position = Vector2.MoveTowards(transform.position,
            waypoints[currentWaypointIndex].position,
            moveSpeed * Time.deltaTime);

        // Kiểm tra nếu NPC đã đến gần waypoint
        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            anim.SetBool("moving", false); // Tắt hoạt ảnh di chuyển

            // Nếu đã đạt đến điểm ngẫu nhiên, ngừng di chuyển và chọn waypoint mới
            if (currentWaypointIndex == targetWaypointIndex)
            {
                StartCoroutine(SelectNextWaypoint());
            }
            else
            {
                // Tiếp tục đi đến waypoint kế tiếp theo hướng (lên hoặc xuống)
                if (currentWaypointIndex < targetWaypointIndex)
                {
                    currentWaypointIndex++; // Tiến lên
                }
                else
                {
                    currentWaypointIndex--; // Quay ngược lại
                }
            }
        }
    }

    private IEnumerator SelectNextWaypoint()
    {
        isMoving = false; // Dừng tạm thời

        // Chờ 1 giây trước khi chọn waypoint mới
        yield return new WaitForSeconds(1f);

        // Chọn waypoint ngẫu nhiên trong mảng
        do
        {
            targetWaypointIndex = Random.Range(0, waypoints.Length);
        } while (targetWaypointIndex == currentWaypointIndex); // Tránh chọn lại điểm hiện tại

        // Kích hoạt lại di chuyển
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
                SetAnimFloat(Vector2.right); // Di chuyển sang phải
            }
            else if (direction.x < 0)
            {
                SetAnimFloat(Vector2.left); // Di chuyển sang trái
            }
        }
        else
        {
            if (direction.y > 0)
            {
                SetAnimFloat(Vector2.up); // Di chuyển lên
            }
            else if (direction.y < 0)
            {
                SetAnimFloat(Vector2.down); // Di chuyển xuống
            }
        }
    }
}
