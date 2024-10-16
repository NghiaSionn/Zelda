using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OreData_")]
public class OreData : ScriptableObject
{
    public string oreName = "";
    public int oreHealth = 1;
    public GameObject orePrefab;
    public GameObject droppedResource;
    [Range(1, 10)] public int oreChance;
    public int allowLevel = 1;

    public bool IsLevelAllow(int currentLevel) => currentLevel >= allowLevel;

    private void OnValidate()
    {
        if (allowLevel < 1)
        {
            Debug.LogWarning($"allowLevel phải ít nhất là 1. Current value: {allowLevel}", this);
            allowLevel = 1;
        }
    }
}
