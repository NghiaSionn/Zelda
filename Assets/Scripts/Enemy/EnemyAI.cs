using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : Enemy
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private Vector2 movespotPosition;
    private List<Vector2Int> wanderspotPositions;
    private Vector2 direction;
    private Vector2 startingPosition;
    private float waitTimer;

    [Header("AI Settings")]
    [SerializeField] private float followRange = 5f;
    [SerializeField] private float shootRange = 3f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private bool canWander = true;
    [SerializeField] private float waitTime = 5f;

    void Start()
    {
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

        if (distanceToPlayer <= followRange && CheckLineOfSight())
        {
            SetFollowState();
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.position - transform.position, Mathf.Infinity, layerMask);
        bool hasLineOfSight = hit.collider != null && hit.collider.CompareTag("Player");

        Debug.DrawRay(transform.position, (player.position - transform.position).normalized * followRange * followRange,
                      hasLineOfSight ? Color.green : Color.red);

        return hasLineOfSight;
    }

    private void SetFollowState()
    {
        direction = (player.position - transform.position).normalized;
        currentState = EnemyState.walk;
        waitTimer = waitTime;
    }

    private void SetWanderState()
    {
        if (currentState == EnemyState.idle)
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
            currentState = EnemyState.idle;
            waitTimer = waitTime;
        }
        else if (currentState != EnemyState.wander)
        {
            SelectNewWanderSpot();
        }
    }

    private void SelectNewWanderSpot()
    {
        movespotPosition = wanderspotPositions[Random.Range(0, wanderspotPositions.Count)];
        direction = (movespotPosition - (Vector2)transform.position).normalized;
        currentState = EnemyState.wander;
        waitTimer = waitTime;
    }

    private void SetReturnState(float distanceToStart)
    {
        if (waitTimer <= 0)
        {
            if (distanceToStart < 0.2f)
            {
                direction = Vector2.zero;
                currentState = EnemyState.idle;
            }
            else
            {
                direction = (startingPosition - (Vector2)transform.position).normalized;
                currentState = EnemyState.walk;
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
        if (currentState == EnemyState.idle) return;

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void FixedUpdateAnimation()
    {
        spriteRenderer.flipX = direction.x < 0;

        animator.SetFloat("posX", Mathf.Abs(direction.x));
        animator.SetFloat("posY", direction.y);
        animator.SetBool("isWalking", currentState != EnemyState.idle);
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
        Gizmos.DrawWireSphere(transform.position, followRange);

        if (currentState == EnemyState.wander)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(movespotPosition, 0.2f);
        }
    }
}