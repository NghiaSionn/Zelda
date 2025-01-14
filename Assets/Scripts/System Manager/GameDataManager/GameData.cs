using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public string sceneName;
    public float playerHealth; 
    public float playerExp; 
    public int numberOfKeys;
    public int coins;
    public int meats;
    public int logs;
    public int fish;
    public int dayCount;
    public string currentTimeString;
    public bool isRaining;
    public string rainEndTimeString;
    public List<ItemData> items;
}

// ItemData.cs - Lớp lưu trữ thông tin item
[Serializable]
public class ItemData
{
    public string itemName;
    public string spriteBase64;
    public string itemDescription;
    public int quantity;
    public bool usable;
    public bool unique;
    public bool isKey;
    public Item.ItemType itemType;
    public Item.ItemUseType itemUseType;
    public int healAmount;
    public int manaAmount;
    public int priceItems;
    public int quantityBuys;
    public int maxQuantity;
}