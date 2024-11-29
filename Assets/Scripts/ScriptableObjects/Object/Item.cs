using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;


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

    public int numberHeld;
    public bool usable;
    public bool unique;
    public bool isKey;

    public enum ItemType { Coin, Meat, Log, Fish,Other } 
    public ItemType itemType; 
}
