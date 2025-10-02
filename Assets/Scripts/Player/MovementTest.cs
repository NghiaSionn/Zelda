using UnityEngine;
using System.Collections;

public class MovementTest : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float runSpeed = 4f;
    public float dashForce = 1f; 
    public Animator animator;

    private Rigidbody2D rb;
    private int currentAttack = 0;
    private bool isAttacking = false;
    private bool queuedAttack = false; 
    private bool isSwordEquipped = false;
    private Vector2 lastMoveDirection = Vector2.right; 

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

            // Lưu lại hướng di chuyển cuối cùng
            if (movement.magnitude > 0)
            {
                lastMoveDirection = movement;
            }

            // Kiểm tra nút Shift để chạy
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float currentSpeed = isRunning ? runSpeed : moveSpeed;

            // Di chuyển nhân vật
            rb.velocity = movement * currentSpeed;

            // Cập nhật Animator
            UpdateAnimator(movement, isRunning);
        
        if(Input.GetKey(KeyCode.Tab))
        {
            isSwordEquipped = true;
        }

        // Kiểm tra input tấn công
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking && isSwordEquipped)
            {
                StartCoroutine(AttackCoroutine());
            }
            else
            {
                queuedAttack = true;
            }
        }
    }

    void UpdateAnimator(Vector2 movement, bool isRunning)
    {
        bool isMoving = movement.magnitude > 0;
        animator.SetBool("moving", isMoving);
        animator.SetBool("running", isRunning);

        if (isMoving)
        {
            animator.SetFloat("moveX", movement.x);
            animator.SetFloat("moveY", movement.y);
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        queuedAttack = false;
        currentAttack = 1;

        // Đòn 1
        animator.SetTrigger("attack1");
        StartCoroutine(DashForward()); // Lướt nhẹ về phía trước
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.1f);

        // Nếu người chơi nhấn tấn công tiếp
        if (queuedAttack)
        {
            queuedAttack = false;
            currentAttack = 2;
            animator.SetTrigger("attack2");
            StartCoroutine(DashForward());
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
        }

        if (queuedAttack)
        {
            queuedAttack = false;
            currentAttack = 3;
            animator.SetTrigger("attack3");
            StartCoroutine(DashForward());
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 0.1f);
        }

        // Chờ một khoảng trước khi có thể tấn công lại
        yield return new WaitForSeconds(0.5f);

        // Kết thúc tấn công
        isAttacking = false;
        queuedAttack = false;
        currentAttack = 1;
    }

    IEnumerator DashForward()
    {
        float dashDuration = 0.15f; 
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            rb.velocity = lastMoveDirection * dashForce;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero; 
    }


}
