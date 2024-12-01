using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootBag : Interactable
{
    private bool isLoot = false;

    [Header("Cài đặt túi loot")]
    public LootBagData lootBagData;
    public Inventory playerInventory;
    public SignalSender powerupSignal;

    [Header("Coin Text Manager")]
    public CoinTextManager coinTextManager;

    [Header("Loot UI Manager")]
    public LootUIManager lootUIManager;

    [Header("Thời gian hiển thị và hủy túi")]
    public float uiDisplayDuration = 1.5f;
    public float destroyDelay = 1f;

    void Awake()
    {
        
        coinTextManager = FindObjectOfType<CoinTextManager>();
        lootUIManager = FindObjectOfType<LootUIManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange && !isLoot)
        {
            LootingBag();
        }
    }

  private void LootingBag()
{
    isLoot = true;
    Debug.Log("Đã loot túi!");

    int lootCount = Random.Range(lootBagData.lootCountRange.x, lootBagData.lootCountRange.y + 1);
    Debug.Log($"Nhặt được {lootCount} vật phẩm.");

    List<LootBagData.LootItem> tempItems = new List<LootBagData.LootItem>(lootBagData.lootItems);
    tempItems.Shuffle();

    // Tạo một danh sách mới để chứa các LootItem đã có số lượng được tính sẵn
    List<LootBagData.LootItem> lootItemsToDisplay = new List<LootBagData.LootItem>();

    for (int i = 0; i < lootCount && i < tempItems.Count; i++)
    {
        LootBagData.LootItem lootItem = tempItems[i];
        int quantity = Random.Range(lootItem.quantityRange.x, lootItem.quantityRange.y + 1);

        lootItem.quantity = quantity;  

        lootItemsToDisplay.Add(lootItem);  

        Debug.Log($"Nhặt được {quantity} {lootItem.item.itemName}.");

        // Thêm vật phẩm vào Inventory
        playerInventory.AddItem(lootItem.item, quantity);

        // Nếu là coin, chỉ cập nhật số lượng coin trong CoinTextManager
        if (lootItem.item.itemType == Item.ItemType.Coin)
        {
            coinTextManager.UpdateCoinCount();
        }
    }

    // Hiển thị UI loot với danh sách vật phẩm đã tính số lượng
    lootUIManager.DisplayLoot(lootItemsToDisplay);

    StartCoroutine(DestroyLootBag());
}






    private IEnumerator DestroyLootBag()

    {

        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);

    }

}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
