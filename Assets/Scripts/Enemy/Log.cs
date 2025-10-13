using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : Enemy
{
    [Header("Rigibody")]
    public Rigidbody2D myRigidbody;

    [Header("Người bị tấn công")]
    public Transform target;

    [Header("Khoảng cách tấn công")]
    public float chaseRaidus;
    public float attackRadius;

    public Transform homePosition;
    public Animator anim;


    private new void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    void Start()
    {
        currentState = EnemyState.idle;
        
        anim = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        anim.SetBool("wakeUp", true);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        CheckDistance();
    }


    public virtual void CheckDistance()
    {
        if (Vector3.Distance(target.position,
                            transform.position) <= chaseRaidus
           && Vector3.Distance(target.position,
                               transform.position) > attackRadius)
        {
            if (currentState == EnemyState.idle || currentState == EnemyState.walk
                && currentState != EnemyState.stagger)
            {
                Vector3 temp = Vector3.MoveTowards(transform.position,
                                                     target.position,
                                                     moveSpeed * Time.deltaTime);
                changeAnim(temp - transform.position);
                myRigidbody.MovePosition(temp);               
                ChangeState(EnemyState.walk);
                anim.SetBool("wakeUp", true);
            }
        }
        else if (Vector3.Distance(target.position,
                            transform.position) > chaseRaidus)
        {
            anim.SetBool("wakeUp", false);
        }
    }


    private void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("moveX", setVector.x);
        anim.SetFloat("moveY",setVector.y);
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
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
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
 


    public void ChangeState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }
}
