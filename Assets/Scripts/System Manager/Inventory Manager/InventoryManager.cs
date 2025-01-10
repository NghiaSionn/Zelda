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

    public Item currentItem;

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
        nameText.text = ""; // Hoặc thông tin khác nếu cần
        useButton.SetActive(false);

    }

    void MakeInventorySlots()
    {
        if (playerInventory)
        {
            foreach (Item item in playerInventory.items)
            {
                GameObject temp = Instantiate(blankInventorySlot, inventoryPanel.transform.position, Quaternion.identity);

                temp.transform.SetParent(inventoryPanel.transform);
                temp.GetComponent<RectTransform>().localScale = Vector3.one;

                InventorySlot newSlot = temp.GetComponent<InventorySlot>();

                if (newSlot)
                {
                    newSlot.Setup(item, this);
                    newSlot.descriptionPanel = descriptionPanel;

                    // Cập nhật số lượng coin nếu item là Coin
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

    private void UpdateInventoryUI()
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

    public void UseButtonPressed()
    {
        if (playerInventory.items.Count == 0) return;

        Item itemToUse = GetUsableItem();
        if (itemToUse == null) return;

        itemToUse.Use();
        itemToUse.quantity--;

        // Cập nhật số lượng hiển thị trong UI
        InventorySlot[] slots = inventoryPanel.GetComponentsInChildren<InventorySlot>();
        foreach (var slot in slots)
        {
            if (slot.thisItem == itemToUse)
            {
                if (itemToUse.quantity > 0)
                {
                    slot.itemNumberText.text = itemToUse.quantity.ToString();
                }
                else
                {
                    Destroy(slot.gameObject);
                    playerInventory.RemoveItem(itemToUse);
                }
                break;
            }
        }

       
        if (itemToUse.quantity < 1)
        {
            SetTextAndButton("", "", false, 0);
        }
    }


    private Item GetUsableItem()
    {
        
        foreach (var item in playerInventory.items)
        {
            if (item.quantity > 0) 
            {
                
                switch (item.itemUseType)
                {
                    case Item.ItemUseType.Healing:
                        if(FindObjectOfType<PlayerMovement>().IsHealthFull())
                        {
                            return null;
                        }
                        HealPlayer(item.healAmount);
                        return item;
                    case Item.ItemUseType.Mana:
                        RestoreMana(item.manaAmount);
                        return item; 
                    case Item.ItemUseType.Buff:
                        return item; 
                    default:
                        continue;
                }
            }
        }

       
        return null;
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
}
