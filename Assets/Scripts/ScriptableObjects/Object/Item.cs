using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Item : ScriptableObject
{
    [Header("Ảnh vật phẩm")]
    public Sprite itemSprite;

    [Header("Tên vật phẩm")]
    public string itemName;

    [Header("Mô tả vật phẩm")]
    public string itemDescription;

    public bool isKey;

    [Header("Số lượng")]
    public int quantity;

    public enum ItemType { Coin, Meat, Log, Fish,Other } 
    public ItemType itemType; 
}
