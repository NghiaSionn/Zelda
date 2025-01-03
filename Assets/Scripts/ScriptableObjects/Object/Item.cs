using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;


[CreateAssetMenu]
public class Item : ScriptableObject
{
    [Header("Ảnh vật phẩm")]
    public Sprite itemSprite;

    [Header("Tên vật phẩm")]
    public string itemName;

    [Header("Mô tả vật phẩm")]
    public string itemDescription;

    [Header("Số lượng")]
    public int quantity;

    public bool usable;
    public bool unique;
    public bool isKey;

    public UnityEvent thisEvent;

    public void Use()
    {
        Debug.Log("vật phẩm đã đc dùng");
        thisEvent.Invoke();
    }


    public enum ItemType { Coin, Meat, Log, Fish, Other }
    [Header("Thể loại vật phẩm")]  
    public ItemType itemType;

    public enum ItemUseType { Other,Healing,Mana,Buff, }
    [Header("Thể loại vật phẩm đc dùng")]
    public ItemUseType itemUseType;
    public int healAmount;
    public int manaAmount;

    [Header("Vật phẩm để mua")]
    //public bool itemBuys;
    public int priceItems;
    public int quantityBuys;
    public int maxQuantity;
}
