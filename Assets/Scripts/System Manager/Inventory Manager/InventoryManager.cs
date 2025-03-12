using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Quản lý UI")]
    public Inventory playerInventory;
    [SerializeField] private GameObject blankInventorySlot;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject useButton;

    private InventorySlot selectedSlot;
    public Item currentItem;
    public int maxSize = 32;

    private void Start()
    {
        UpdateCoinUI();
    }
    private void OnEnable()
    {
        playerInventory.OnCoinsChanged += UpdateCoinUI;
    }

    private void OnDisable()
    {
        playerInventory.OnCoinsChanged -= UpdateCoinUI;
    }

    private void UpdateCoinUI()
    {
        descriptionText.text = $"Coins: {playerInventory.coins}";

       
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



    public void SetTextAndButton(string description, string name, bool buttonActive,int quanity)
    {
        descriptionText.text = description;
        nameText.text = name;

        
        if (quanity < 1)
        {
            useButton.SetActive(false); 
        }
        else
        {
            useButton.SetActive(buttonActive); 
        }

        descriptionText.text = $"Coins: {playerInventory.coins}";
        nameText.text = ""; 
        useButton.SetActive(false);

    }

    void MakeInventorySlots()
    {
        if (playerInventory)
        {
            foreach (Item item in playerInventory.items)
            {
                if (inventoryPanel.transform.childCount >= maxSize)
                    break;

                GameObject temp = Instantiate(blankInventorySlot, inventoryPanel.transform.position, Quaternion.identity);
                temp.transform.SetParent(inventoryPanel.transform);
                temp.GetComponent<RectTransform>().localScale = Vector3.one;
                InventorySlot newSlot = temp.GetComponent<InventorySlot>();

                if (newSlot)
                {
                    newSlot.Setup(item, this);
                    newSlot.descriptionPanel = descriptionPanel;

                    if (item.itemType == Item.ItemType.Coin)
                    {
                        newSlot.itemNumberText.text = playerInventory.coins.ToString();
                    }
                }
            }
        }
    }



    void Awake()
    {
        descriptionPanel.SetActive(false);
        MakeInventorySlots();
        SetTextAndButton("", "", false,0);
        playerInventory.OnItemAdded += UpdateInventoryUI;
    }

    public void UpdateInventoryUI()
    {
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
        descriptionText.text = newDescriptionString;
        nameText.text = newNameTextString;
        useButton.SetActive(isButtonUsable);
     
    }

    public void SetUpDescriptionAndButton2(string newDescriptionString, string newNameTextString, Item newItem)
    {
        currentItem = newItem;
        descriptionText.text = newDescriptionString;
        nameText.text = newNameTextString;
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



    //private Item GetUsableItem()
    //{      
    //    foreach (var item in playerInventory.items)
    //    {
    //        if (item.quantity > 0) 
    //        {              
    //            switch (item.itemUseType)
    //            {
    //                case Item.ItemUseType.Healing:
    //                    if(FindObjectOfType<PlayerMovement>().IsHealthFull())
    //                    {
    //                        return null;
    //                    }
    //                    HealPlayer(item.healAmount);
    //                    return item;
    //                case Item.ItemUseType.Mana:
    //                    RestoreMana(item.manaAmount);
    //                    return item; 
    //                case Item.ItemUseType.Buff:
    //                    return item; 
    //                default:
    //                    continue;
    //            }
    //        }
    //    }

       
    //    return null;
    //}


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
}
