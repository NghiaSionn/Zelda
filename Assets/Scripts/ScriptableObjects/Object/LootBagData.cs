using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LootBagData : ScriptableObject
{
    [System.Serializable]
    public class LootItem
    {
        public Item item;
        public Vector2Int quantityRange;
    }

    public string bagName;
    public LootItem[] lootItems;
    public Vector2Int lootCountRange;
}