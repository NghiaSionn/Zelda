using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootUIManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI[] textItems; 
    public Image[] imageItems;


    

    [Header("Thời gian hiển thị UI")]
    public float uiDisplayDuration = 2f;

    private void Awake()
    {

        HideLootUI();
    }

    public void DisplayLoot(List<LootBagData.LootItem> lootItems)
    {
        HideLootUI();
        int maxItemsToDisplay = Mathf.Min(lootItems.Count, textItems.Length);

        // Hiển thị thông tin cho từng vật phẩm
        for (int i = 0; i < maxItemsToDisplay; i++)
        {
            LootBagData.LootItem lootItem = lootItems[i];
            Item item = lootItem.item;

            // Chỉ cần hiển thị số lượng vật phẩm đã được tính sẵn từ LootBag
            int quantity = lootItem.quantity;

            // Hiển thị thông tin vật phẩm và số lượng
            textItems[i].text = $"+ {quantity} {item.itemName}";
            imageItems[i].sprite = item.itemSprite;

            textItems[i].gameObject.SetActive(true);
            imageItems[i].gameObject.SetActive(true);
        }

        // Ẩn UI sau một thời gian
        StartCoroutine(HideLootUIAfterDelay());
    }


    public void HideLootUI()
    {
        // Ẩn tất cả các Text và Image
        foreach (var text in textItems)
        {
            text.gameObject.SetActive(false);
        }
        foreach (var image in imageItems)
        {
            image.gameObject.SetActive(false);
        }
    }

    private IEnumerator HideLootUIAfterDelay()
    {
        yield return new WaitForSeconds(uiDisplayDuration);
        HideLootUI();
    }
}