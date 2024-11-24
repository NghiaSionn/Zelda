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

    public void AddItem(Item itemToAdd)
    {
        // Kiểm tra xem vật phẩm có tồn tại trong inventory chưa
        bool itemExists = false;
        foreach (Item item in items)
        {
            if (item.itemName == itemToAdd.itemName)
            {
                item.quantity++;
                itemExists = true;
                break;
            }
        }

        // Nếu vật phẩm chưa tồn tại, thêm mới vào inventory
        if (!itemExists)
        {
            items.Add(itemToAdd);
            itemToAdd.quantity = 1;

            // Kiểm tra itemType và cập nhật số lượng log/meat
            switch (itemToAdd.itemType)
            {
                case Item.ItemType.Log:
                    logs++;
                    break;
                case Item.ItemType.Meat:
                    meats++;
                    break;
            }
        }
    }
}
