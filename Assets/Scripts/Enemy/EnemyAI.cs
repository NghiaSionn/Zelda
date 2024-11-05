using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private Enemy enemy;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private Vector2 movespotPosition;
    internal List<Vector2Int> wanderspotPositions;
    private Vector2 direction;
    private Vector2 startingPosition;
    private float waitTimer;
    private float lastAttackTimer = 0f;


    [Header("AI Settings")]
    [SerializeField] private bool canWander = true;
    [SerializeField] private float waitTime = 5f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 2f;
    [Header("Range Settings")]
    [SerializeField] private float retreatRange = 3f;
    [SerializeField] private GameObject projectilePrefab;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Transform>();

        startingPosition = transform.position;
        waitTimer = waitTime;
    }

    void FixedUpdate()
    {
        CheckState();
        FixedUpdateMovement();
        FixedUpdateAnimation();
    }

    private void CheckState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceToStart = Vector2.Distance(transform.position, startingPosition);

        if (distanceToPlayer <= chaseRange && CheckLineOfSight())
        {
            if (enemy.enemyType == EnemyType.ranged)
            {
                if (distanceToPlayer < retreatRange)
                {
                    SetRetreatState();
                }
                else if (distanceToPlayer <= attackRange)
                {
                    SetAttackState();
                }
                else
                {
                    SetChaseState();
                }
            }
            else if (enemy.enemyType == EnemyType.melee)
            {
                if (distanceToPlayer <= attackRange)
                {
                    SetAttackState();
                }
                else if (enemy.currentState != EnemyState.attack)
                {
                    SetChaseState();
                }
            }
        }
        else if (canWander)
        {
            SetWanderState();
        }
        else
        {
            SetReturnState(distanceToStart);
        }
    }

    private bool CheckLineOfSight()
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Enemy"));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.position - transform.position, chaseRange, layerMask);
        bool hasLineOfSight = hit.collider != null && hit.collider.CompareTag("Player");
        return hasLineOfSight;
    }

    private void SetChaseState()
    {
        direction = (player.position - transform.position).normalized;
        enemy.currentState = EnemyState.chase;
        waitTimer = waitTime;
    }

    private void SetWanderState()
    {
        if (enemy.currentState == EnemyState.idle)
        {
            if (waitTimer <= 0)
            {
                SelectNewWanderSpot();
            }
            else
            {
                direction = Vector2.zero;
                waitTimer -= Time.fixedDeltaTime;
            }
            return;
        }

        if (Vector2.Distance(transform.position, movespotPosition) < 0.2f)
        {
            direction = Vector2.zero;
            enemy.currentState = EnemyState.idle;
            waitTimer = waitTime;
        }
        else if (enemy.currentState != EnemyState.wander)
        {
            SelectNewWanderSpot();
        }
    }

    private void SelectNewWanderSpot()
    {
        movespotPosition = wanderspotPositions[Random.Range(0, wanderspotPositions.Count)];
        direction = (movespotPosition - (Vector2)transform.position).normalized;
        enemy.currentState = EnemyState.wander;
        waitTimer = waitTime;
    }

    private void SetReturnState(float distanceToStart)
    {
        if (waitTimer <= 0)
        {
            if (distanceToStart < 0.2f)
            {
                direction = Vector2.zero;
                enemy.currentState = EnemyState.idle;
            }
            else
            {
                direction = (startingPosition - (Vector2)transform.position).normalized;
                enemy.currentState = EnemyState.chase;
            }
        }
        else
        {
            direction = Vector2.zero;
            waitTimer -= Time.fixedDeltaTime;
        }
    }

    private void SetRetreatState()
    {
        direction = -(player.position - transform.position).normalized;
        enemy.currentState = EnemyState.retreat;
    }

    private void SetAttackState()
    {
        if (enemy.currentState == EnemyState.attack) return;
        if (Time.time < lastAttackTimer + attackCooldown) return;
        enemy.currentState = EnemyState.attack;

        if (enemy.enemyType == EnemyType.melee)
        {
            StartCoroutine(MeleeAttack());
        }
        else if (enemy.enemyType == EnemyType.ranged)
        {
            StartCoroutine(RangeAttack());
        }
    }

    private IEnumerator MeleeAttack()
    {
        animator.SetTrigger("attackMelee");
        lastAttackTimer = Time.time;

        var length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length + 0.5f);

        enemy.currentState = EnemyState.idle;
    }

    private IEnumerator RangeAttack()
    {
        animator.SetTrigger("attackRanged");
        lastAttackTimer = Time.time;

        var length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(length + 0.5f);

        Vector2 shootDirection = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        projectileRb.velocity = shootDirection * projectile.GetComponent<Projectile>().speed;

        yield return null;

        enemy.currentState = EnemyState.idle;
    }

    private void FixedUpdateMovement()
    {
        if (enemy.currentState == EnemyState.idle || enemy.currentState == EnemyState.attack)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (enemy.currentState == EnemyState.retreat || Vector2.Distance(transform.position, player.position) > 1f)
        {
            rb.MovePosition(rb.position + direction * enemy.moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void FixedUpdateAnimation()
    {
        if (enemy.currentState != EnemyState.retreat)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
        else
        {
            spriteRenderer.flipX = direction.x > 0;
        }

        animator.SetFloat("posX", Mathf.Abs(direction.x));
        animator.SetFloat("posY", direction.y);
        animator.SetBool("isWalking", enemy.currentState != EnemyState.idle);
    }

    public void InitializeWanderSpots(List<Vector2Int> positions)
    {
        wanderspotPositions = positions;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall") && canWander)
        {
            movespotPosition = transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        if (enemy.currentState == EnemyState.wander)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(movespotPosition, 0.2f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}