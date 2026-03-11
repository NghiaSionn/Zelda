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
    public int minExp;
    public int maxExp;
    public GameObject LootPowerup()
    {
        int cumProb = 0;
        int currentPob = Random.Range(0, 101);  

        
        
         
        for (int i = 0; i < loots.Length; i++)
        {
            cumProb += loots[i].lootChance;

            

            if (currentPob <= cumProb) 
            {
                return loots[i].thisLoot;
            }
        }

        
        return null;
    }

    public int GetExp()
    {
        int currentExp;        
        currentExp = Random.Range(minExp, maxExp);
        Debug.Log($"Exp nhận đc: {currentExp}");
        return currentExp;
    }
}

