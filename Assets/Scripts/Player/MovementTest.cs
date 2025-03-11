using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float runSpeed = 4f;
    public Animator animator;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Nhận input di chuyển
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Tính toán vector di chuyển
        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // Kiểm tra nút Shift để chạy
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Chọn tốc độ di chuyển dựa trên trạng thái chạy
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        // Di chuyển nhân vật
        rb.velocity = movement * currentSpeed;

        // Cập nhật Animator
        UpdateAnimator(movement, isRunning);
    }

    void UpdateAnimator(Vector2 movement, bool isRunning)
    {
        // Kiểm tra xem nhân vật có đang di chuyển hay không
        bool isMoving = movement.magnitude > 0;
        animator.SetBool("moving", isMoving);

        // Kiểm tra xem nhân vật có đang chạy hay không
        animator.SetBool("running", isRunning);

        // Xác định hướng di chuyển
        if (isMoving)
        {
            animator.SetFloat("moveX", movement.x);
            animator.SetFloat("moveY", movement.y);
        }
    }
}