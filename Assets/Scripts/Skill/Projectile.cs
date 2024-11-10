using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Tốc độ của skill")]
    public float speed = 10f;

    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.velocity = direction * speed;
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("enemy") ||  other.gameObject.CompareTag("Tree"))
        {
            
            Destroy(gameObject);
        }

        if(other.gameObject.CompareTag("Interactive"))
        {
            Destroy(gameObject);
        }
    }

   


}