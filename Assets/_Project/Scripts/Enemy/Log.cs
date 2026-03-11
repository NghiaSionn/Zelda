using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
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

    [Header("NavMesh Wander Cài đặt")]
    public float wanderRadius = 5f;
    public float roamTimeMin = 2f;
    public float roamTimeMax = 5f;
    public float minDistanceToEdge = 1f;

    protected NavMeshAgent agent;
    protected float wanderTimer;
    protected float wanderWaitTime;
   
    private new void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    protected virtual void Start()
    {
        currentState = EnemyState.idle;
        
        anim = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        
        // Setup Agent cho 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        // Gán speed của NavMesh bằng với speed của Enemy
        agent.speed = moveSpeed;

        // Tự động đồng bộ attackRadius với stoppingDistance để quái vật có thể vung chém ngay khi Agent dừng lại
        if (agent.stoppingDistance > attackRadius)
        {
            attackRadius = agent.stoppingDistance;
        }
        else
        {
            agent.stoppingDistance = attackRadius;
        }

        anim.SetBool("wakeUp", true);
        ResetWaitTime();
    }

    void FixedUpdate()
    {
        CheckDistance();
    }

    public virtual void CheckDistance()
    {
        // Chặn luồng tìm đường di chuyển nếu quái vật đang bị Stun (Stagger)
        if (currentState == EnemyState.stagger)
        {
            if (agent.hasPath)
            {
                agent.ResetPath();
            }
            SetMoving(false);
            return;
        }

        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (distanceToTarget <= chaseRaidus && distanceToTarget > attackRadius)
        {
            // --- CHASE STATE ---
            ChangeState(EnemyState.walk);
            anim.SetBool("wakeUp", true);
            SetMoving(true);
            
            // Dùng NavMesh để rượt đổi mục tiêu
            agent.SetDestination(target.position);
            UpdateAnimationDirection();
        }
        else if (distanceToTarget > chaseRaidus)
        {
            // --- WANDER STATE (Idle) ---
            anim.SetBool("wakeUp", false);
            WanderMechanisms();
        }
    }

    protected virtual void WanderMechanisms()
    {
        wanderTimer += Time.deltaTime;

        // Khi lang thang thì nếu đến đích hoặc vô tình mất đường đi
        if (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath)
        {
            SetMoving(false);

            if (wanderTimer >= wanderWaitTime)
            {
                ChooseNewWanderTarget();
                ResetWaitTime();
                wanderTimer = 0;
            }
        }
        else
        {
            SetMoving(true);
            UpdateAnimationDirection();
            ChangeState(EnemyState.wander);
        }
    }

    protected void ChooseNewWanderTarget()
    {
        // Thử tìm điểm tối đa 30 lần
        for (int i = 0; i < 30; i++)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            if (newPos != Vector3.zero)
            {
                // Tránh né mép tường
                NavMeshHit edgeHit;
                if (NavMesh.FindClosestEdge(newPos, out edgeHit, -1))
                {
                    if (edgeHit.distance < minDistanceToEdge)
                        continue;
                }

                // Check đường đi hợp lệ
                NavMeshPath path = new NavMeshPath();
                if (agent.CalculatePath(newPos, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetDestination(newPos);
                    break;
                }
            }
        }
    }

    protected void ResetWaitTime()
    {
        wanderWaitTime = Random.Range(roamTimeMin, roamTimeMax);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector2 randomDirection = Random.insideUnitCircle * dist;
        Vector3 randomPoint = origin + new Vector3(randomDirection.x, randomDirection.y, 0f);

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomPoint, out navHit, 1.0f, layermask))
        {
            return navHit.position;
        }
        return Vector3.zero;
    }

    protected void UpdateAnimationDirection()
    {
        Vector2 direction = new Vector2(agent.velocity.x, agent.velocity.y).normalized;
        if (direction != Vector2.zero)
        {
            changeAnim(direction);
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
                SetAnimFloat(Vector2.right);
            else if (direction.x < 0)
                SetAnimFloat(Vector2.left);
        }
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0)
                SetAnimFloat(Vector2.up);
            else if (direction.y < 0)
                SetAnimFloat(Vector2.down);
        }
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
        
        if (Application.isPlaying && agent != null && agent.hasPath)
        {
            Gizmos.color = Color.red; 
            Gizmos.DrawSphere(agent.destination, 0.2f); 
        }
    }
}
