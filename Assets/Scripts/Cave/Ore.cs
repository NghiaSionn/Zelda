using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ore : MonoBehaviour
{
    [SerializeField] private OreData oreData;
    private int health;

    private void Start()
    {
        health = oreData.oreHealth;
    }

    internal void MineOre(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Instantiate(oreData.droppedResource, this.transform.position, Quaternion.identity);

            CaveLevelManager.Instance.oreDataDicts[CaveLevelManager.Instance.currentLevel]
                                     .Remove(this.transform.position);
            Destroy(this.gameObject);
        }
    }

    private void OnMouseDown()
    {
        MineOre(10);
    }
}