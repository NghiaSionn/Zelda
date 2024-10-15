using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Resource : MonoBehaviour
{
    [SerializeField] private ResourceData resourceData;
    private int health;

    private void Start()
    {
        health = resourceData.resourceHealth;
    }

    internal void MineResource(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Debug.Log("StartCoroutine");
            StartCoroutine(DropResource());

            LevelCaveManager.Instance.resourceDataDicts[LevelCaveManager.Instance.currentLevel].Remove(this.transform.position);
            Destroy(this.gameObject);
        }
    }

    private IEnumerator DropResource()
    {
        GameObject resource = Instantiate(resourceData.droppedResource, this.transform.position, Quaternion.identity);

        Rigidbody2D rb = resource.GetComponent<Rigidbody2D>();

        Vector2 randomUpDirection = new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized;
        rb.AddForce(randomUpDirection * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);

        Debug.Log("Here");

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
    }

    private void OnMouseDown()
    {
        MineResource(10);
    }
}