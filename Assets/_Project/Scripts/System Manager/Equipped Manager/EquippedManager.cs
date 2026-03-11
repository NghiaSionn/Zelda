using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedManager : MonoBehaviour
{
    [Header("UI Equip Slots")]
    public EquippedSlot[] equippedSlots;
    private Animator[] buttonAnimator;

    private int currentSelectedIndex = -1;

    private void Start()
    {
        buttonAnimator = new Animator[equippedSlots.Length];

        for (int i = 0; i < equippedSlots.Length; i++)
        {
            if (equippedSlots[i] != null)
            {
                buttonAnimator[i] = equippedSlots[i].GetComponent<Animator>(); 
            }
        }
    }

    private void Update()
    {
        DetectEquipInput();
    }

    private void DetectEquipInput()
    {
        for (int i = 1; i <= equippedSlots.Length; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                SelectEquipSlot(i - 1);
                return;
            }
        }
    }

    public void SelectEquipSlot(int index)
    {
        if (index < 0 || index >= equippedSlots.Length) return;

        // Nếu slot đã được chọn -> Bỏ chọn
        if (currentSelectedIndex == index)
        {
            if (buttonAnimator[index] != null)
            {
                buttonAnimator[index].SetTrigger("Normal"); 
            }
            currentSelectedIndex = -1; 
            return;
        }

        // Nếu slot chưa được chọn -> Chọn slot mới
        currentSelectedIndex = index;
        Debug.Log($"Selected Equip Slot: {index}");

        if (buttonAnimator != null)
        {
            for (int i = 0; i < equippedSlots.Length; i++)
            {
                if (buttonAnimator[i] != null)
                {
                    if (i == index)
                    {
                        buttonAnimator[i].SetTrigger("Selected"); 
                    }
                    else
                    {
                        buttonAnimator[i].SetTrigger("Normal"); 
                    }
                }
                else
                {
                    Debug.LogWarning($"Animator is NULL at index {i}");
                }
            }
        }
    }

    public Inventory playerInventory;

    public void EquipItem(int slotIndex, Item newItem, string itemCount)
    {
        if (slotIndex < 0 || slotIndex >= equippedSlots.Length) return;

        equippedSlots[slotIndex].SetItem(newItem, itemCount);
    }

    public void EquipItemFromInventory(InventorySlot invSlot, int equipSlotIndex)
    {
        if (invSlot == null || invSlot.thisItem == null) return;
        if (equipSlotIndex < 0 || equipSlotIndex >= equippedSlots.Length) return;

        // Fallback: Nếu playerInventory chưa được gán, thử lấy từ InventoryManager của slot
        if (playerInventory == null && invSlot.thisManager != null)
        {
            playerInventory = invSlot.thisManager.playerInventory;
        }

        // Nếu vẫn null thì không thể xử lý
        if (playerInventory == null) 
        {
             Debug.LogWarning("PlayerInventory is null in EquippedManager!");
             return;
        }

        Item itemToEquip = invSlot.thisItem; // Item đang được kéo vào
        Item currentEquippedItem = equippedSlots[equipSlotIndex].equippedItem; // Item đang có trong slot (nếu có)
        
        // Nếu đã có item trong slot equipment, trả nó về kho
        if (currentEquippedItem != null)
        {            
            playerInventory.AddItem(currentEquippedItem, currentEquippedItem.quantity);
        }

        // Đặt item mới vào Equipment Slot
        equippedSlots[equipSlotIndex].SetItem(itemToEquip, itemToEquip.quantity.ToString());

        // Xóa item khỏi Inventory
        playerInventory.RemoveItem(itemToEquip);
        
        // Cập nhật UI Inventory
        InventoryManager invManager = invSlot.thisManager;
        if (invManager != null)
        {
            invManager.UpdateInventoryUI();
        }
    }

    public void UnEquipItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedSlots.Length) return;

        Item itemToUnEquip = equippedSlots[slotIndex].equippedItem;
        if (itemToUnEquip != null)
        {
             // Thêm lại vào Inventory
             playerInventory.AddItem(itemToUnEquip, itemToUnEquip.quantity);
             
             // Xóa khỏi slot Equipment
             equippedSlots[slotIndex].SetItem(null, ""); // SetItem cần handle null logic để ẩn ảnh
             equippedSlots[slotIndex].UpdateEquippedSlot(); // Helper để cập nhật UI nếu cần
             
             // Update UI Inventory
             FindObjectOfType<InventoryManager>().UpdateInventoryUI();
        }
    }

    public void SwapEquipSlots(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= equippedSlots.Length) return;
        if (indexB < 0 || indexB >= equippedSlots.Length) return;

        Item itemA = equippedSlots[indexA].equippedItem;
        string countA = equippedSlots[indexA].numberItem.text;

        Item itemB = equippedSlots[indexB].equippedItem;
        string countB = equippedSlots[indexB].numberItem.text;

        // Swap visual and data
        if (itemA != null) equippedSlots[indexB].SetItem(itemA, countA);
        else equippedSlots[indexB].SetItem(null, "");

        if (itemB != null) equippedSlots[indexA].SetItem(itemB, countB);
        else equippedSlots[indexA].SetItem(null, "");
    }

}
