using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{

    private EnemyDungeon enemy;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private Vector2 movespotPosition;
    internal List<Vector2Int> wanderspotPositions;
    private Vector2 direction;
    private Vector2 startingPosition;
    private float waitTimer;


    [Header("AI Settings")]
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private bool canWander = true;
    [SerializeField] private float waitTime = 5f;

    void Start()
    {
        enemy = GetComponent<EnemyDungeon>();
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
            SetChaseState();
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

    private void FixedUpdateMovement()
    {
        if (enemy.currentState == EnemyState.idle) return;

        if(Vector2.Distance(transform.position, player.position) > 1f)
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
        spriteRenderer.flipX = direction.x < 0;

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
        if (other.gameObject.CompareTag("Wall"))
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
    }
}