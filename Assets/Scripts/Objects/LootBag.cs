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

        // Tạo danh sách tạm thời và shuffle
        List<LootBagData.LootItem> tempItems = new List<LootBagData.LootItem>(lootBagData.lootItems);
        tempItems.Shuffle();

        // Gọi hàm DisplayLoot của LootUIManager để hiển thị UI
        lootUIManager.DisplayLoot(tempItems.GetRange(0, lootCount));

        for (int i = 0; i < lootCount && i < tempItems.Count; i++)
        {
            LootBagData.LootItem lootItem = tempItems[i];
            Item selectedItem = lootItem.item;
            int quantity = Random.Range(lootItem.quantityRange.x, lootItem.quantityRange.y + 1);

            if (selectedItem.itemType == Item.ItemType.Coin)
            {
                // Cập nhật UI tiền
                playerInventory.coins += quantity;
                coinTextManager.UpdateCoinCount();
            }
            else
            {
                // Thêm vào Inventory
                playerInventory.AddItem(selectedItem);

                switch (selectedItem.itemType)
                {
                    case Item.ItemType.Log:
                        playerInventory.logs += quantity;
                        break;
                    case Item.ItemType.Meat:
                        playerInventory.meats += quantity;
                        break;
                }
            }
        }

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
