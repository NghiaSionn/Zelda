using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class ShopSlots : MonoBehaviour, IPointerClickHandler
{
    [Header("Vật phẩm để bán")]
    public Item itemBuys;

    [Header("UI")]
    public Image itemImage;
    public TextMeshProUGUI numberHeld;

    [Header("UI Mô tả vật phẩm")]
    public GameObject desPanel;
    public Image itemImageDes;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPriceText;

    private ShopSlots previousSelectedSlot; 
    private ShopManager shopManager;
    public int slotIndex;

    void Start()
    {
        desPanel.SetActive(false);
        UpdateSlot();
        shopManager = FindObjectOfType<ShopManager>();
    }

    public void UpdateSlot()
    {
        if (itemBuys != null)
        {
            itemImage.sprite = itemBuys.itemSprite;
            itemImageDes.sprite = itemBuys.itemSprite;

            numberHeld.text = itemBuys.quantityBuys.ToString();
            itemName.text = itemBuys.itemName;
            itemDescription.text = itemBuys.itemDescription;
            itemPriceText.text = $"Price: {itemBuys.priceItems} Coins";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (shopManager != null)
        {
            shopManager.SelectItem(this);
        }
    }

    public void ShowDescription()
    {
        if (itemBuys != null)
        {
            desPanel.SetActive(true);
            itemImageDes.sprite = itemBuys.itemSprite;
            itemName.text = itemBuys.itemName;
            itemDescription.text = itemBuys.itemDescription;
            itemPriceText.text = $"Price: {itemBuys.priceItems} Coins";
        }
    }

    public void HideDescription()
    {
        desPanel.SetActive(false);
    }
}