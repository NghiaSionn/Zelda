using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Nh?n input di chuy?n
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Tính toán vector di chuy?n
        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // Di chuy?n nhân v?t
        rb.velocity = movement * moveSpeed;

        // C?p nh?t Animator
        UpdateAnimator(movement);
    }

    void UpdateAnimator(Vector2 movement)
    {
        // Ki?m tra xem nhân v?t có ðang di chuy?n hay không
        bool isMoving = movement.magnitude > 0;
        animator.SetBool("moving", isMoving);

        // Xác ð?nh hý?ng di chuy?n
        if (isMoving)
        {
            animator.SetFloat("moveX", movement.x);
            animator.SetFloat("moveY", movement.y);
        }
    }
}