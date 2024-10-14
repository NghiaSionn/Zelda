using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData_")]
public class ResourceData : ScriptableObject
{
    public string resourceName;
    public int resourceHealth;
    public GameObject resourcePrefab;
    [Range(1, 11)] public int resoureChance;
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
