using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Inventory : ScriptableObject
{
    public Item currentItem;
    public List<Item> items = new List<Item>();

    public int numberOfKeys;
    public int coins;
    public int meats;
    public int logs;
    public int fish;

    public void AddItem(Item itemToAdd, int amount)
    {
        bool itemExists = false;
        foreach (Item item in items)
        {
            if (item.itemName == itemToAdd.itemName)
            {
                item.quantity += amount;
                itemExists = true;
                break;
            }
        }

        if (!itemExists)
        {

            itemToAdd.quantity = amount;
            items.Add(itemToAdd);
        }


        switch (itemToAdd.itemType)
        {
            case Item.ItemType.Coin:
                coins += amount;
                break;
            case Item.ItemType.Meat:
                meats += amount;
                break;
            case Item.ItemType.Log:
                logs += amount;
                break;
            case Item.ItemType.Fish:
                fish += amount;
                break;
        }
    }

    
}
