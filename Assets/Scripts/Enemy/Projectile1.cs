using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile1 : MonoBehaviour
{
    public float speed = 10f;
    public bool isBound = false;
    public float lifeTime = 5f;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        StartCoroutine(AutoDestroy());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            if (isBound)
            {
                Vector2 normal = ((Vector2)transform.position - other.ClosestPoint(transform.position)).normalized;
                rb.velocity = Vector2.Reflect(rb.velocity, normal);
            }
            else
            {
                StartCoroutine(DestroyProjectile());
            }
        }
        if(other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DestroyProjectile());
        }
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        StartCoroutine(DestroyProjectile());
    }

    private IEnumerator DestroyProjectile()
    {
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Destroy");

        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }
}