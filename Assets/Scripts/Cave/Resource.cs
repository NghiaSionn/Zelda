using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private float attractRange = 3f;
    [SerializeField] private float attractSpeed = 5f;
    private Transform player;
    private bool hasBeenCollected = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        StartCoroutine(DropResource());
    }

    private void Update()
    {
        AttractToPlayer();
    }

    private void AttractToPlayer()
    {
        float distanceToPlayer = Vector2.Distance(this.transform.position, player.position);

        if (distanceToPlayer <= attractRange)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.position,
                                                    attractSpeed * Time.deltaTime);
        }
    }

    private IEnumerator DropResource()
    {
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();

        Vector2 randomUpDirection = new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized;
        rb.AddForce(randomUpDirection * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !hasBeenCollected)
        {
            hasBeenCollected = true;
            Debug.Log($"Add {this.name}");
            Destroy(this.gameObject);
        }
    }
}
