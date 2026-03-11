using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LootBagData;

[CreateAssetMenu]
public class LootBagData : ScriptableObject
{
    [System.Serializable]
    public class LootItem
    {
        public Item item;
        public Vector2Int quantityRange;
        public int quantity;
    }

    public string bagName;
    public LootItem[] lootItems;
    public Vector2Int lootCountRange;

    
}

