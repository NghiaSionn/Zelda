//using System.Collections;
//using UnityEngine;

//public class BossSkill : MonoBehaviour
//{
//    public Transform player; // Người chơi
//    public float attackRange = 5f; // Khoảng cách tấn công
//    public float skillRange = 3f; // Khoảng cách để sử dụng skill
    
//    public Animator animator; 

//    private float distanceToPlayer; 
//    private bool isAttacking; 

//    void Start()
//    {
//        player = GameObject.FindWithTag("Player").transform; // Tìm kiếm người chơi trong scene
//        animator = GetComponent<Animator>(); // Gán animator cho boss
//    }

//    void Update()
//    {
//        // Tính khoảng cách đến người chơi
//        distanceToPlayer = Vector2.Distance(transform.position, player.position);

//        if (distanceToPlayer <= attackRange)
//        {
//            // Nếu khoảng cách đến người chơi nhỏ hơn tầm tấn công
//            AttackPlayer();
//        }
//        else
//        {
//            // Nếu người chơi ra ngoài tầm tấn công
//            MoveTowardsPlayer();
//        }
//    }

//    // Hàm để boss di chuyển đến người chơi
//    void MoveTowardsPlayer()
//    {
//        if (distanceToPlayer > skillRange)
//        {
//            // Nếu không trong tầm skill, di chuyển lại gần người chơi
//            Vector2 direction = (player.position - transform.position).normalized;
//            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
//            animator.SetFloat("moveX", direction.x); // Chuyển động hướng X
//            animator.SetFloat("moveY", direction.y); // Chuyển động hướng Y
//            animator.SetBool("isMoving", true);
//        }
//        else
//        {
//            // Nếu trong tầm skill, dừng lại và chuẩn bị tấn công
//            animator.SetBool("isMoving", false);
//        }
//    }

//    // Hàm tấn công người chơi
//    void AttackPlayer()
//    {
//        if (!isAttacking)
//        {
//            isAttacking = true;

//            // Nếu người chơi trong tầm skill, dùng skill mạnh
//            if (distanceToPlayer <= skillRange)
//            {
//                animator.SetTrigger("attack1"); // Phát animation attack1
//                Debug.Log("Sử dụng skill mạnh!");
//            }
//            else
//            {
//                animator.SetTrigger("attack2"); // Phát animation attack2
//                Debug.Log("Sử dụng skill nhẹ!");
//            }

//            // Đặt lại trạng thái tấn công sau một khoảng thời gian (giả sử thời gian tấn công là 1 giây)
//            StartCoroutine(ResetAttack());
//        }
//    }

//    // Coroutine để đặt lại trạng thái tấn công
//    IEnumerator ResetAttack()
//    {
//        yield return new WaitForSeconds(1f);
//        isAttacking = false;
//    }
//}
