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

        if (draggedSlot?.thisItem == null) return; // Kiểm tra null an toàn

        // Gán item vào slot và đồng bộ số lượng từ InventorySlot
        SetItem(draggedSlot.thisItem, draggedSlot.itemNumberText.text);
    }

    public void SetItem(Item newItem, string itemCount)
    {
        equippedItem = newItem;

        // Hiển thị hình ảnh vật phẩm
        slotImage.sprite = equippedItem.itemSprite;
        slotImage.color = Color.white;
        slotImage.enabled = true;

        // Đảm bảo đồng bộ số lượng với InventorySlot
        numberItem.enabled = true;
        numberItem.text = itemCount;
    }
}
