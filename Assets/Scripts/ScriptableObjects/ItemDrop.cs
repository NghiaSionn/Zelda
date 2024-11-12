using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDrop_")]
public class ItemDrop : ScriptableObject
{
    public GameObject itemPrefab;
    [Range(0, 100)] public float dropChance;
}
