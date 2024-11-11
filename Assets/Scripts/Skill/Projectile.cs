using Cinemachine;
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

    private CinemachineImpulseSource impulseSource;

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
        
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        rb.velocity = direction * speed;
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("breakable"))
        {
            Destroy(gameObject);
            other.gameObject.GetComponent<Pot>().Smash();
        }


        if (other.gameObject.CompareTag("Interactive") || other.gameObject.CompareTag("Tree") 
            || other.gameObject.CompareTag("House") || other.gameObject.CompareTag("enemy"))
        {
            CameraShakeManager.instance.CameraShake(impulseSource);
            Destroy(gameObject);
        }
    }

   


}