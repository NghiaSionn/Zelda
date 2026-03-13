using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Quản lý UI")]
    public Inventory playerInventory;
    [SerializeField] private GameObject blankInventorySlot;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject useButton;
    [SerializeField] private UnityEngine.UI.Image itemDetailImage; // Hiển thị ảnh item trong panel mô tả
    [SerializeField] private UnityEngine.UI.Scrollbar inventoryScrollbar; // Khóa size scrollbar = 0.2f

    private InventorySlot selectedSlot;
    public Item currentItem;
    public int maxSize = 32;

    private void Start()
    {
        if (playerInventory != null)
        {
            UpdateCoinUI();
        }

        // Ẩn panel mô tả mặc định khi chưa hover
        HideDescription();
    }
    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnCoinsChanged += UpdateCoinUI;
        }
    }

    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnCoinsChanged -= UpdateCoinUI;
        }
    }

    private void UpdateCoinUI()
    {
        if (descriptionText != null && playerInventory != null)
        {
            descriptionText.text = $"Coins: {playerInventory.coins}";
        }

        if (inventoryPanel == null) return;
        
        InventorySlot[] slots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        foreach (var slot in slots)
        {
            if (slot.thisItem != null && slot.thisItem.itemType == Item.ItemType.Coin)
            {
                slot.itemNumberText.text = playerInventory.coins.ToString();
            }
        }
    }


    public void SetSelectedSlot(GameObject slotObject)
    {
        if (selectedSlot != null && selectedSlot.selectedPanel)
        {
            selectedSlot.selectedPanel.SetActive(false);
        }

        selectedSlot = slotObject.GetComponent<InventorySlot>();

        if (selectedSlot != null)
        {
            Debug.Log($"Selected slot: {selectedSlot.name}, Item: {selectedSlot.thisItem.itemName}");
            if (selectedSlot.selectedPanel)
            {
                selectedSlot.selectedPanel.SetActive(true);
            }

            SetUpDescriptionAndButton(selectedSlot.thisItem.itemDescription,
                              selectedSlot.thisItem.itemName, selectedSlot.thisItem.usable, selectedSlot.thisItem);
        }
        else
        {
            SetTextAndButton("", "", false, 0);
        }
    }



    public void SetTextAndButton(string description, string name, bool buttonActive, int quanity)
    {
        if (descriptionText != null)
        {
            descriptionText.text = description;
        }
        
        if (nameText != null)
        {
            nameText.text = name;
        }

        if (useButton != null)
        {
            if (quanity < 1)
            {
                useButton.SetActive(false);
            }
            else
            {
                useButton.SetActive(buttonActive);
            }
        }

        if (descriptionText != null && playerInventory != null)
        {
            descriptionText.text = $"Coins: {playerInventory.coins}";
        }
        
        if (nameText != null)
        {
            nameText.text = "";
        }
        
        if (useButton != null)
        {
            useButton.SetActive(false);
        }

    }

    void MakeInventorySlots()
    {
        if (playerInventory == null || inventoryPanel == null || blankInventorySlot == null)
        {
            return;
        }
        
        if (playerInventory)
        {
            foreach (Transform child in inventoryPanel.transform)
            {
                Destroy(child.gameObject);
            }

            List<InventorySlot> slots = new List<InventorySlot>();

            for (int i = 0; i < maxSize; i++)
            {
                // Instantiate trực tiếp vào parent với localSpace=false để Canvas Scaler không làm sai kích thước
                GameObject temp = Instantiate(blankInventorySlot, inventoryPanel.transform, false);
                temp.GetComponent<RectTransform>().localScale = Vector3.one;
                InventorySlot newSlot = temp.GetComponent<InventorySlot>();
                newSlot.index = i;
                slots.Add(newSlot);

                // Đặt slot rỗng nếu chưa có item
                newSlot.Setup(null, this);
            }

            int index = 0;
            foreach (Item item in playerInventory.items)
            {
                if (index >= maxSize) break;

                // Bỏ qua item được đánh dấu ẩn khỏi inventory (vd: Coin đã có UI riêng)
                // KHÔNG tăng index để slot đó được item tiếp theo điền vào
                if (item.hideFromInventory)
                {
                    continue;
                }

                InventorySlot newSlot = slots[index];
                if (newSlot)
                {
                    newSlot.Setup(item, this);

                    if (item.itemType == Item.ItemType.Coin)
                    {
                        newSlot.itemNumberText.text = playerInventory.coins.ToString();
                    }
                }
                index++;
            }
        }

        // Khoá size scrollbar cố định = 0.2f sau khi build xong
        if (inventoryScrollbar != null)
        {
            inventoryScrollbar.size = 0.2f;
        }
    }



    void Awake()
    {

        // Reset runtime values và đọc quantity khởi đầu từ Coin item trong Inspector
        if (playerInventory != null)
        {
            playerInventory.Initialize();
        }

        MakeInventorySlots();
        SetTextAndButton("", "", false, 0);
        
        if (playerInventory != null)
        {
            playerInventory.OnItemAdded += UpdateInventoryUI;
        }
    }

    public void UpdateInventoryUI()
    {
        if (inventoryPanel == null) return;
        
        foreach (Transform child in inventoryPanel.transform)
        {
            if (child == null || child.gameObject == null)
            {
                Debug.LogWarning("Slot bị null trước khi Destroy.");
                continue;
            }

            Destroy(child.gameObject);
        }

        MakeInventorySlots();

        SetTextAndButton("", "", false, playerInventory.coins);
    }



    public void SetUpDescriptionAndButton(string newDescriptionString, string newNameTextString, bool isButtonUsable, Item newItem)
    {
        currentItem = newItem;
        
        if (descriptionText != null)
        {
            descriptionText.text = newDescriptionString;
        }
        
        if (nameText != null)
        {
            nameText.text = newNameTextString;
        }
        
        if (useButton != null)
        {
            useButton.SetActive(isButtonUsable);
        }

    }

    public void SetUpDescriptionAndButton2(string newDescriptionString, string newNameTextString, Item newItem)
    {
        currentItem = newItem;

        // Hiện các phần tử mô tả khi hover vào slot
        if (descriptionText != null) { descriptionText.gameObject.SetActive(true); descriptionText.text = newDescriptionString; }
        if (nameText != null)        { nameText.gameObject.SetActive(true);        nameText.text = newNameTextString; }
        if (itemDetailImage != null && newItem != null)
        {
            itemDetailImage.gameObject.SetActive(true);
            itemDetailImage.sprite = newItem.itemSprite;
            itemDetailImage.preserveAspect = true;
            itemDetailImage.enabled = true;
        }
    }

    /// <summary>Tắt đi các phần tử mô tả khi không còn hover vào slot nào.</summary>
    public void HideDescription()
    {
        if (descriptionText != null)  descriptionText.gameObject.SetActive(false);
        if (nameText != null)         nameText.gameObject.SetActive(false);
        if (itemDetailImage != null)  itemDetailImage.gameObject.SetActive(false);
    }

    public void UseButtonPressed()
    {
        if (selectedSlot == null || selectedSlot.thisItem == null) return;

        Item itemToUse = selectedSlot.thisItem;


        if (itemToUse.quantity <= 0) return;


        switch (itemToUse.itemUseType)
        {
            case Item.ItemUseType.Healing:
                if (FindObjectOfType<PlayerMovement>().IsHealthFull())
                {
                    Debug.Log("Health đã đầy, không thể sử dụng vật phẩm này.");
                    return;
                }
                HealPlayer(itemToUse.healAmount);
                break;
            case Item.ItemUseType.Mana:
                RestoreMana(itemToUse.manaAmount);
                break;
            case Item.ItemUseType.Buff:
                ApplyBuff(itemToUse);
                break;
            default:
                Debug.Log("Vật phẩm này không thể sử dụng.");
                return;
        }


        itemToUse.quantity--;

        if (itemToUse.quantity > 0)
        {
            selectedSlot.itemNumberText.text = itemToUse.quantity.ToString();
        }
        else
        {
            Destroy(selectedSlot.gameObject);
            playerInventory.RemoveItem(itemToUse);
        }

        // Nếu hết số lượng, tắt mô tả và nút sử dụng
        if (itemToUse.quantity < 1)
        {
            SetTextAndButton("", "", false, 0);
        }
    }

    private void HealPlayer(int healAmount)
    {
        var playerMovement = FindObjectOfType<PlayerMovement>();

        if (playerMovement == null) return;

        if (!playerMovement.IsHealthFull())
        {
            playerMovement.UpdateHealth(healAmount);

        }
    }

    private void RestoreMana(int manaAmount)
    {
        var playerMana = FindObjectOfType<StaminaWheel>();
        if (playerMana == null) return;

        playerMana.RestoreMana(manaAmount);
        Debug.Log($"Hồi {manaAmount} mana. Mana hiện tại: {playerMana.mana}/{playerMana.maxMana}");
    }

    private void ApplyBuff(Item item)
    {


    }
    public bool SwapItems(int slotIndexA, int slotIndexB)
    {
        if (playerInventory == null) return false;
        if (slotIndexA < 0 || slotIndexA >= playerInventory.items.Count) return false;
        if (slotIndexB < 0 || slotIndexB >= playerInventory.items.Count) return false;

        Item temp = playerInventory.items[slotIndexA];
        playerInventory.items[slotIndexA] = playerInventory.items[slotIndexB];
        playerInventory.items[slotIndexB] = temp;

        UpdateInventoryUI();
        return true;
    }
}