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
    private float moveX, moveY;
    private float waitTime;


    [Header("AI settings")]
    public float followRange = 5f;
    public float shootRange = 3f;
    public float attackRange = 3f;
    public bool canWander = true;
    public int wanderRange = 2;
    public float startWaitTime = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        waitTime = startWaitTime;
    }

    void Update()
    {
        CheckState();

        switch (currentState)
        {
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

        Debug.DrawRay(transform.position, player.position - transform.position.normalized,
                      hasLineOfSight ? Color.green : Color.red);

        return hasLineOfSight;
    }

    private void CheckState()
    {
        var distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= followRange && CheckRaycast())
        {
            direction = (player.position - transform.position).normalized;
            currentState = EnemyState.walk;
        }
        else if (currentState != EnemyState.wander)
        {
            direction = (movespotPosition - (Vector2)transform.position).normalized;
            currentState = EnemyState.wander;
        }
    }

    public void InitializeWanderSpots(List<Vector2Int> floorPositions)
    {
        wanderspotPositions = floorPositions;
        movespotPosition = wanderspotPositions[Random.Range(0, wanderspotPositions.Count)];
    }

    private void Walk()
    {
        moveX = direction.x;
        moveY = direction.y;

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    private void Wander()
    {
        moveX = direction.x;
        moveY = direction.y;

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, movespotPosition) < 0.2f)
        {
            if (waitTime <= 0)
            {
                movespotPosition = wanderspotPositions[Random.Range(0, wanderspotPositions.Count)];
                direction = (movespotPosition - (Vector2)transform.position).normalized;
                waitTime = startWaitTime;
            }
            else
            {
                direction = Vector2.zero;
                waitTime -= Time.fixedDeltaTime;
            }
        }
    }

    private void UpdateAnimation()
    {
        spriteRenderer.flipX = moveX < 0;

        animator.SetFloat("posX", Mathf.Abs(moveX));
        animator.SetFloat("posY", moveY);
        animator.SetBool("isWalking", currentState == EnemyState.walk);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}