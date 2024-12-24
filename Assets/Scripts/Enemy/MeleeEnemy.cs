using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Log
{
    public Test_Rigidbody2D testRigidbody2D;  

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
    }

    public override void CheckDistance()
    {
        float distanceToPlayer = Vector3.Distance(target.position, transform.position);

        if (distanceToPlayer <= chaseRaidus && distanceToPlayer > attackRadius)
        {
            
            if (testRigidbody2D != null)
                testRigidbody2D.enabled = false; 

            if (currentState == EnemyState.idle || currentState == EnemyState.walk && currentState != EnemyState.stagger)
            {
                Vector3 temp = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                changeAnim(temp - transform.position);
                myRigidbody.MovePosition(temp);
                ChangeState(EnemyState.walk);
                anim.SetBool("moving", true); 
                
            }
            
        }
        

        else if (distanceToPlayer <= chaseRaidus && distanceToPlayer <= attackRadius)
        {
            // Khi ở trong tầm tấn công, thực hiện tấn công
            if (currentState == EnemyState.walk && currentState != EnemyState.stagger)
            {
                StartCoroutine(AttackCo());
            }
        }
        else
        {
            anim.SetBool("moving", false);

            if (testRigidbody2D != null)
                testRigidbody2D.enabled = true; 
        }
    }

    public IEnumerator AttackCo()
    {
        currentState = EnemyState.attack;
        anim.SetBool("attack", true);
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
