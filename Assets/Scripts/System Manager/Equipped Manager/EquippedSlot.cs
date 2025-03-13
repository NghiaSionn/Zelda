using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquippedSlot : MonoBehaviour, IDropHandler
{
    public Item equippedItem;
    public Image slotImage;
    public TextMeshProUGUI numberItem;

    private void Start()
    {
        slotImage.enabled = false;
        numberItem.enabled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();

        if (draggedSlot?.thisItem == null) return;

        SetItem(draggedSlot.thisItem, draggedSlot.itemNumberText.text);
    }

    public void SetItem(Item newItem, string itemCount)
    {
        equippedItem = newItem;

        slotImage.sprite = equippedItem.itemSprite;
        slotImage.color = Color.white;
        slotImage.enabled = true;

        numberItem.enabled = true;
        numberItem.text = itemCount;
    }

    public void UpdateEquippedSlot()
    {
        if (equippedItem != null)
        {
            numberItem.text = equippedItem.quantity.ToString();
        }
    }
}