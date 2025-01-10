using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<Item> shopItems;
    public List<ShopSlots> shopSlots;
    public ShopSlots selectedSlot;
    public Inventory inventory;
    public CoinTextManager coinTextManager;
    public WorldTime worldTime; // Tham chiếu đến WorldTime

    void Start()
    {
        coinTextManager.UpdateCoinCount();

        if (shopItems.Count > shopSlots.Count)
        {
            Debug.LogError("Số lượng item nhiều hơn số lượng slot!");
            return;
        }

        for (int i = 0; i < shopItems.Count; i++)
        {
            shopSlots[i].itemBuys = shopItems[i];
            shopSlots[i].UpdateSlot();
        }
    }

    private void OnEnable()
    {
        inventory.OnCoinsChanged += UpdateShopUI;
        worldTime.WorldDayChange += OnNewDay; 

    }

    private void OnDisable()
    {
        inventory.OnCoinsChanged -= UpdateShopUI;
        worldTime.WorldDayChange -= OnNewDay; 
        
    }

    private void UpdateShopUI()
    {
        for (int i = 0; i < shopSlots.Count; i++)
        {
            shopSlots[i].UpdateSlot();
        }
    }

    public void SelectItem(ShopSlots slot)
    {
        if (selectedSlot != null)
        {
            selectedSlot.HideDescription();
        }

        selectedSlot = slot;

        if (selectedSlot != null)
        {
            selectedSlot.ShowDescription();
        }
    }

    public void BuyItem()
    {
        if (selectedSlot != null)
        {
            Item itemToBuy = selectedSlot.itemBuys;

            if (itemToBuy == null)
            {
                Debug.Log("Không có vật phẩm nào được chọn!");
                return;
            }

            if (itemToBuy.quantityBuys <= 0)
            {
                Debug.Log("Vật phẩm này đã hết hàng!");
                return;
            }

            if (inventory.coins < itemToBuy.priceItems)
            {
                Debug.Log("Không đủ tiền để mua " + itemToBuy.itemName);
                return;
            }

            // Trừ tiền và giảm số lượng
            inventory.RemoveCoins(itemToBuy.priceItems);
            itemToBuy.quantityBuys--;
            selectedSlot.UpdateSlot();

            // Thêm vật phẩm vào kho
            inventory.AddItem(itemToBuy, 1);

            Debug.Log("Đã mua " + itemToBuy.itemName);
            coinTextManager.UpdateCoinCount();
        }
        else
        {
            Debug.Log("Vui lòng chọn một vật phẩm để mua!");
        }
    }

    private void OnNewDay(object sender, int newDayCount)
    {
        // Đặt lại số lượng của tất cả vật phẩm về giá trị ban đầu
        foreach (var item in shopItems)
        {
            item.quantityBuys = item.maxQuantity; // maxQuantity là số lượng tối đa có thể mua
        }

        // Cập nhật giao diện shop
        UpdateShopUI();
        Debug.Log("Số lượng vật phẩm đã được đặt lại vì qua ngày mới!");
    }
}
