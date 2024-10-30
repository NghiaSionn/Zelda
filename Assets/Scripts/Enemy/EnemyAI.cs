using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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


    [Header("AI settings")]
    public float followRange = 5f;
    public float shootRange = 3f;
    public float attackRange = 3f;
    public bool canWander = true;
    public int wanderRange = 2;
    public float waitTime = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Transform>();
        startingPosition = transform.position;
        waitTimer = waitTime;
    }

    void Update()
    {
        CheckState();

        switch (currentState)
        {
            case EnemyState.idle:
                direction = Vector2.zero;
                break;
            case EnemyState.walk:
                Walk();
                break;
            case EnemyState.wander:
                if (canWander) Wander();
                break;
        }

        UpdateAnimation();
    }

    private bool CheckRaycast()
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Enemy"));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.position - transform.position, Mathf.Infinity, layerMask);
        var hasLineOfSight = hit.collider != null && hit.collider.CompareTag("Player");

        Debug.DrawRay(transform.position, (player.position - transform.position).normalized * followRange,
                      hasLineOfSight ? Color.green : Color.red);

        return hasLineOfSight;
    }

    private void CheckState()
    {
        var distanceToPlayer = Vector2.Distance(transform.position, player.position);
        var distanceToStart = Vector2.Distance(transform.position, startingPosition);

        if (distanceToPlayer <= followRange && CheckRaycast())
        {
            direction = (player.position - transform.position).normalized;
            currentState = EnemyState.walk;
            waitTimer = waitTime;
        }
        else if (canWander)
        {
            if (currentState != EnemyState.wander)
            {
                movespotPosition = wanderspotPositions[Random.Range(0, wanderspotPositions.Count)];
                direction = (movespotPosition - (Vector2)transform.position).normalized;
                currentState = EnemyState.wander;
                waitTimer = waitTime;
            }
        }
        else
        {
            if (waitTimer <= 0)
            {
                if (distanceToStart < 0.2f)
                {
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
    }

    public void InitializeWanderSpots(List<Vector2Int> floorPositions)
    {
        wanderspotPositions = floorPositions;
        movespotPosition = wanderspotPositions[Random.Range(0, wanderspotPositions.Count)];
    }

    private void Walk()
    {
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void Wander()
    {
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, movespotPosition) < 0.2f)
        {
            if (waitTimer <= 0)
            {
                movespotPosition = wanderspotPositions[Random.Range(0, wanderspotPositions.Count)];
                direction = (movespotPosition - (Vector2)transform.position).normalized;
                waitTimer = waitTime;
            }
            else
            {
                direction = Vector2.zero;
                waitTimer -= Time.fixedDeltaTime;
            }
        }
    }

    private void UpdateAnimation()
    {
        spriteRenderer.flipX = direction.x < 0;

        animator.SetFloat("posX", Mathf.Abs(direction.x));
        animator.SetFloat("posY", direction.y);
        animator.SetBool("isWalking", currentState != EnemyState.idle);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}