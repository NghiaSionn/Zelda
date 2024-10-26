using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : Enemy
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform target;
    private float moveX, moveY;
    private bool isMoving;
    private Vector3 startPosition;
    private bool hasLineOfSight = false;
    public float followRange = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        startPosition = transform.position;
    }

    void Update()
    {
        CheckDistance();

        spriteRenderer.flipX = moveX < 0;

        animator.SetFloat("posX", Mathf.Abs(moveX));
        animator.SetFloat("posY", moveY);
        animator.SetBool("isMoving", isMoving);
    }

    private void CheckDistance()
    {
        CheckRaycast();
        var distanceToPlayer = Vector2.Distance(transform.position, target.position);
        var distanceToStart = Vector2.Distance(transform.position, startPosition);
        Vector2 direction;

        if (distanceToPlayer <= followRange && hasLineOfSight)
        {
            direction = (target.position - transform.position).normalized;
            isMoving = true;
        }
        else
        {
            direction = (startPosition - transform.position).normalized;
            isMoving = distanceToStart > 0.1f;
        }

        moveX = direction.x;
        moveY = direction.y;
    }

    private void CheckRaycast()
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Enemy"));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            hasLineOfSight = hit.collider.CompareTag("Player");
            Debug.DrawRay(transform.position, target.position - transform.position.normalized,
                          hasLineOfSight ? Color.green : Color.red);
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 direction = new Vector2(moveX, moveY).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}