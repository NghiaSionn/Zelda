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

    private void Start()
    {
        HideLootUI();
    }

    public void DisplayLoot(List<LootBagData.LootItem> lootItems)
    {
        HideLootUI();

        // Hiển thị tối đa 3 vật phẩm
        int maxItemsToDisplay = Mathf.Min(lootItems.Count, 3);
        

        for (int i = 0; i < maxItemsToDisplay; i++)
        {
            Item item = lootItems[i].item;
            int quantity = Random.Range(lootItems[i].quantityRange.x, lootItems[i].quantityRange.y + 1);

            // Hiển thị thông tin vật phẩm
            textItems[i].text = $"+ {item.itemName} x{quantity}";
            imageItems[i].sprite = item.itemSprite;

            // Kích hoạt UI
            textItems[i].gameObject.SetActive(true);
            imageItems[i].gameObject.SetActive(true);
        }

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