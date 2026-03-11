using System.Collections;
using UnityEngine;

public class BossFrogger : Boss
{
    [Header("Cài đặt cơ bản")]
    public float chaseRadius = 5f;  
    public float attackRadius = 2f;
    public Vector2 minimumAttackDistanceSize = new Vector2(3f, 3f);
    public float attackCooldown = 1f;  

    [Header("Tham chiếu")]
    public Transform target;
    public Animator anim;
    private Rigidbody2D myRigidbody;

    private bool isAttacking = false;  

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        anim = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        CheckDistance();
    }

    private void CheckDistance()
    {
        
        float distanceToPlayerX = Mathf.Abs(target.position.x - transform.position.x);
        float distanceToPlayerY = Mathf.Abs(target.position.y - transform.position.y);

        
        if (distanceToPlayerX <= chaseRadius && distanceToPlayerY <= chaseRadius)
        {
            
            if (distanceToPlayerX > minimumAttackDistanceSize.x / 2 || distanceToPlayerY > minimumAttackDistanceSize.y / 2)
            {
                
                Vector2 direction = (target.position - transform.position).normalized;
                MoveTowardsPlayer(direction);
                changeAnim(direction);
                ChangeState(EnemyState.walk);
            }
            else
            {
                
                anim.SetBool("moving", false);
                ChangeState(EnemyState.idle);
            }

            
            if (distanceToPlayerX <= attackRadius && distanceToPlayerY <= attackRadius && !isAttacking)
            {
                RandomAttack();
            }
        }
        else
        {
            
            anim.SetBool("moving", false);
            ChangeState(EnemyState.idle);
        }
    }


    private void MoveTowardsPlayer(Vector2 direction)
    {
        
        Vector2 newPosition = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        myRigidbody.MovePosition(newPosition);
    }

    public void changeAnim(Vector2 direction)
    {
        
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            anim.SetFloat("moveX", direction.x > 0 ? 1 : -1);
            anim.SetFloat("moveY", 0);
        }
        else
        {
            anim.SetFloat("moveY", direction.y > 0 ? 1 : -1);
            anim.SetFloat("moveX", 0);
        }

        anim.SetBool("moving", true);  
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }

    private void RandomAttack()
    {
        if (isAttacking) return;

        // Random chọn đòn tấn công
        int randomAttack = Random.Range(0, 2); 
        float randomCooldown = Random.Range(3f, 5f); 

        if (randomAttack == 0)
        {
            StartCoroutine(Attack1(randomCooldown));
        }
        else
        {
            StartCoroutine(Attack2(randomCooldown));
        }
    }



    private IEnumerator Attack1(float cooldown)
    {
        

        isAttacking = true;
        currentState = EnemyState.attack;
        anim.SetBool("attack1", true);

        yield return new WaitForSeconds(1f); 

        anim.SetBool("attack1", false);
        currentState = EnemyState.walk;

        yield return new WaitForSeconds(cooldown); 

        isAttacking = false;
    }

    private IEnumerator Attack2(float cooldown)
    {
        isAttacking = true;
        currentState = EnemyState.attack;
        anim.SetBool("attack2", true);

        yield return new WaitForSeconds(1f); 

        anim.SetBool("attack2", false);
        currentState = EnemyState.walk;

        yield return new WaitForSeconds(cooldown); 

        isAttacking = false;
    }




    private void OnDrawGizmosSelected()
    {
       
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.blue;
        Vector3 squareSize = new Vector3(minimumAttackDistanceSize.x, minimumAttackDistanceSize.y, 0f);
        Gizmos.DrawWireCube(transform.position, squareSize);
    }
}
