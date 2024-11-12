using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public float attractRange = 3f;
    public float attractSpeed = 5f;
    private bool canAttract = false;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        StartCoroutine(DropItem());
    }

    private void Update()
    {
        if (canAttract)
        {
            AttractToPlayer();
        }
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

    private IEnumerator DropItem()
    {
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();

        Vector2 randomUpDirection = new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized;
        rb.AddForce(randomUpDirection * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;

        canAttract = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && canAttract)
        {
            SoundManager.Instance.PlaySound3D("pickitem", transform.position);
            Debug.Log($"Add {this.name}");
            Destroy(this.gameObject);
        }
    }
}
