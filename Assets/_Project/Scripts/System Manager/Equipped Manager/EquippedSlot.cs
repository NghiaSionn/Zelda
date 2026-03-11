using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquippedSlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item equippedItem;
    public Image slotImage;
    public TextMeshProUGUI numberItem;

    private GameObject dragGhost;

    private void Start()
    {
        if (equippedItem == null)
        {
            slotImage.enabled = false;
            numberItem.enabled = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (equippedItem != null)
        {
            // Create ghost icon
            dragGhost = new GameObject("DragGhost");
            dragGhost.transform.SetParent(transform.root);
            dragGhost.transform.SetAsLastSibling();

            Image ghostImage = dragGhost.AddComponent<Image>();
            ghostImage.sprite = slotImage.sprite;
            ghostImage.color = new Color(1, 1, 1, 0.8f);
            ghostImage.raycastTarget = false;

            RectTransform rect = dragGhost.GetComponent<RectTransform>();
            rect.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            rect.position = transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (equippedItem != null && dragGhost != null)
        {
            dragGhost.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragGhost != null)
        {
            Destroy(dragGhost);
            dragGhost = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 1. Nhận từ InventorySlot (Logic cũ)
        InventorySlot draggedInvSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();
        EquippedManager equippedManager = FindObjectOfType<EquippedManager>();

        if (draggedInvSlot != null && draggedInvSlot.thisItem != null)
        {
            if (equippedManager != null)
            {
                 int myIndex = System.Array.IndexOf(equippedManager.equippedSlots, this);
                 if (myIndex != -1)
                 {
                     equippedManager.EquipItemFromInventory(draggedInvSlot, myIndex);
                 }
            }
            return;
        }

        // 2. Nhận từ EquippedSlot khác (Logic mới: Swap)
        EquippedSlot draggedEquipSlot = eventData.pointerDrag?.GetComponent<EquippedSlot>();
        if (draggedEquipSlot != null && draggedEquipSlot != this && equippedManager != null)
        {
            int myIndex = System.Array.IndexOf(equippedManager.equippedSlots, this);
            int otherIndex = System.Array.IndexOf(equippedManager.equippedSlots, draggedEquipSlot);

            if (myIndex != -1 && otherIndex != -1)
            {
                equippedManager.SwapEquipSlots(myIndex, otherIndex);
            }
        }
    }

    public void SetItem(Item newItem, string itemCount)
    {
        equippedItem = newItem;

        if (equippedItem != null)
        {
            slotImage.sprite = equippedItem.itemSprite;
            slotImage.color = Color.white;
            slotImage.enabled = true;

            numberItem.enabled = true;
            numberItem.text = itemCount;
        }
        else
        {
            slotImage.enabled = false;
            numberItem.enabled = false;
        }
    }
    
    public void UpdateEquippedSlot()
    {
         // Placeholder for future updates
    }

    private void OnDestroy()
    {
        if (dragGhost != null)
        {
            Destroy(dragGhost);
            dragGhost = null;
        }
    }
}