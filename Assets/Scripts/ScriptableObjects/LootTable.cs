using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loot
{
    public GameObject thisLoot;
    public int lootChance;

}


[CreateAssetMenu]
public class LootTable : ScriptableObject
{
    public Loot[] loots;

    public GameObject LootPowerup()
    {
        int cumProb = 0;
        int currentPob = Random.Range(0, 101);  

        Debug.Log("Current probability: " + currentPob); 

        for (int i = 0; i < loots.Length; i++)
        {
            cumProb += loots[i].lootChance;

            Debug.Log("Cumulative probability: " + cumProb); 

            if (currentPob <= cumProb) 
            {
                return loots[i].thisLoot;
            }
        }

        Debug.LogWarning("Không tìm thấy vật phẩm loot!"); 
        return null;
    }
}

