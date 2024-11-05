using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public bool isBound = false;
    public float lifeTime = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, lifeTime);
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
                Destroy(gameObject);
            }
        }
    }
}