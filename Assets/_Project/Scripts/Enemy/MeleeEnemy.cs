using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Log
{
    // Không cần dùng Test_Rigidbody2D riêng nữa vì Log đã lo phần Random Wander bằng NavMesh
    // public Test_Rigidbody2D testRigidbody2D;  

    void Update()
    {
        CheckDistance();
    }

    public override void CheckDistance()
    {
        // Gọi hàm gốc từ Log.cs để tự động Random Wander và Chase bằng NavMesh
        base.CheckDistance();

        float distanceToPlayer = Vector3.Distance(target.position, transform.position);

        // Kế thừa thêm tính năng tấn công khi Player vào sát tầm
        if (distanceToPlayer <= attackRadius)
        {
            // Dừng NavMeshAgent và tắt hiệu ứng đi bộ để đứng yên đánh
            if (agent.hasPath)
            {
                agent.ResetPath();
            }
            SetMoving(false);

            // Khi ở trong tầm tấn công, thực hiện tấn công
            if (currentState == EnemyState.walk && currentState != EnemyState.stagger)
            {
                StartCoroutine(AttackCo());
            }
        }
    }

    public IEnumerator AttackCo()
    {
        currentState = EnemyState.attack;       
        anim.SetBool("attack", true);
        yield return new WaitForSeconds(0.1f);
        
        // Kế thừa thẳng hàm phát tiếng tấn công từ Enemy.cs gốc
        PlayAttackSound();

        yield return new WaitForSeconds(1f);
        
        currentState = EnemyState.walk;
        anim.SetBool("attack", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRaidus);
    }
}
