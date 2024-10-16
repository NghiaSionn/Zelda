using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DropResource());
    }
    private IEnumerator DropResource()
    {
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();

        Vector2 randomUpDirection = new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized;
        rb.AddForce(randomUpDirection * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        Debug.Log("Here");

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
    }
}
