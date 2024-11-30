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

    public void SetTextAndButton(string description, string name, bool buttonActive)
    {
        descriptionText.text = description;
        nameText.text = name;

        if (buttonActive)
        {
            useButton.SetActive(true);
        }
        else
        {
            useButton.SetActive(false);
        }
    }

    void MakeInventorySlots()
    {
        if (playerInventory)
        {
            for (int i = 0; i < playerInventory.items.Count; i++)
            {
                GameObject temp = Instantiate(blankInventorySlot,
                    inventoryPanel.transform.position, Quaternion.identity);

                temp.transform.SetParent(inventoryPanel.transform);
                temp.GetComponent<RectTransform>().localScale = Vector3.one; 
                InventorySlot newSlot = temp.GetComponent<InventorySlot>();

                if (newSlot)
                {
                    newSlot.Setup(playerInventory.items[i], this);
                    newSlot.descriptionPanel = descriptionPanel;
                }
            }
        }
    }


    void Start()
    {
        descriptionPanel.SetActive(false);
        MakeInventorySlots();
        SetTextAndButton("", "", false);
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
        if (currentItem)
        {
            currentItem.Use();
        }
    }
}
