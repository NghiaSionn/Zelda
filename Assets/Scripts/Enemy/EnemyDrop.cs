using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    public List<ItemDrop> itemDropList;
    public bool guaranteeOneItem = false;

    public void DropItems(Vector3 position)
    {
        bool droppedItem = false;

        foreach (var item in itemDropList)
        {
            if (Random.Range(0f, 100f) <= item.dropChance)
            {
                Instantiate(item.itemPrefab, position, Quaternion.identity);
                droppedItem = true;
            }
        }

        if (guaranteeOneItem && !droppedItem && itemDropList.Count > 0)
        {
            var randomItem = itemDropList[Random.Range(0, itemDropList.Count)];
            Instantiate(randomItem.itemPrefab, position, Quaternion.identity);
        }
    }
}
