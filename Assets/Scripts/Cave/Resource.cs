using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log($"{resourceData.resourceName} - {health}");

        if (health <= 0)
        {
            Debug.Log($"{resourceData.resourceName} đã hủy.");
            Destroy(this.gameObject);
        }
    }
}