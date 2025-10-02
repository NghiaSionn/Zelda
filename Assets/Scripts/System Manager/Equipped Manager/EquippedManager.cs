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

    public void EquipItem(int slotIndex, Item newItem, string itemCount)
    {
        if (slotIndex < 0 || slotIndex >= equippedSlots.Length) return;

        equippedSlots[slotIndex].SetItem(newItem, itemCount);
    }

}
