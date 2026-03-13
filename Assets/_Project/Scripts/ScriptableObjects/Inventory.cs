using System;
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

    public event Action OnItemAdded;
    public event Action OnCoinsChanged;

    /// <summary>
    /// Gọi khi bắt đầu game — Reset tất cả runtime value và đọc giá trị đầu từ Coin item.quantity.
    /// Giúp "chỉnh sửạ Coin quantity trong Inspector" có tác dụng.
    /// </summary>
    public void Initialize()
    {
        coins = 0;
        meats = 0;
        logs = 0;
        fish = 0;
        numberOfKeys = 0;

        // Đọc giá trị khởi đầu của coins từ Coin item trong số vật phẩm được đăng ký trong Inspector
        foreach (Item item in items)
        {
            if (item != null && item.itemType == Item.ItemType.Coin)
            {
                coins = item.quantity;
                break;
            }
        }

        OnCoinsChanged?.Invoke();
    }
    public void AddItem(Item itemToAdd, int amount)
    {
        // Cộng vào bộ đếm riêng dựa theo loại (1 lần duy nhất)
        switch (itemToAdd.itemType)
        {
            case Item.ItemType.Coin:
                coins += amount;
                OnCoinsChanged?.Invoke();
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

        // Tìm item đã có trong list và cập nhật quantity
        bool itemExists = false;
        foreach (Item item in items)
        {
            if (item.itemName == itemToAdd.itemName)
            {
                item.quantity += amount;

                // Đồng bộ quantity của Coin item với coins để không bị lệch
                if (item.itemType == Item.ItemType.Coin)
                {
                    item.quantity = coins;
                }

                itemExists = true;
                break;
            }
        }

        // Nếu chưa có thì thêm mới vào list
        if (!itemExists)
        {
            itemToAdd.quantity = (itemToAdd.itemType == Item.ItemType.Coin) ? coins : amount;
            items.Add(itemToAdd);
        }

        OnItemAdded?.Invoke();
    }
    
    public void RemoveItem(Item itemToRemove)
    {
        if (items.Contains(itemToRemove))
        {
            items.Remove(itemToRemove);
        }
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;
        if (coins < 0) coins = 0; 
        OnCoinsChanged?.Invoke();
    }
}
