using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI")]
    [SerializeField] public TextMeshProUGUI itemNumberText;
    [SerializeField] public Image itemImage;

    [Header("Thông số vật phẩm")]
    public Item thisItem;
    public InventoryManager thisManager;

    public GameObject descriptionPanel;
    public GameObject selectedPanel;

    private RectTransform descriptionPanelRect;
    private RectTransform selectedPanelRect;

    public bool isPointerInside = false;
    private Transform parentAfterDrag;

    public int index;

    void Start()
    {

    }

    public void Setup(Item newItem, InventoryManager newManager)
    {
        thisItem = newItem;
        thisManager = newManager;

        if (thisItem != null)
        {
            itemImage.enabled = true;
            itemImage.sprite = thisItem.itemSprite;

            itemNumberText.enabled = true;
            itemNumberText.text = thisItem.quantity.ToString();
        }
        else
        {
            itemImage.enabled = false;
            itemNumberText.enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (thisItem && descriptionPanel && !isPointerInside)
        {
            isPointerInside = true;
            descriptionPanel.SetActive(true);
            thisManager.SetUpDescriptionAndButton2(thisItem.itemDescription, thisItem.itemName, thisItem);

            Vector3 mousePosition = Input.mousePosition;
            descriptionPanelRect = descriptionPanel.GetComponent<RectTransform>();
            descriptionPanelRect.position = mousePosition + new Vector3(400f, 0f, 0f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionPanel && isPointerInside)
        {
            isPointerInside = false;
            descriptionPanel.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Slots: {gameObject.name} clicked.");
        if (thisItem != null)
        {
            thisManager.SetSelectedSlot(gameObject);
        }
    }

    public void ClickedOn()
    {
        if (thisItem)
        {
            thisManager.SetUpDescriptionAndButton(thisItem.itemDescription, thisItem.itemName, thisItem.usable, thisItem);
            thisManager.SetSelectedSlot(gameObject);
        }
    }

    private GameObject dragGhost;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (thisItem != null)
        {
            // Create ghost icon
            dragGhost = new GameObject("DragGhost");
            dragGhost.transform.SetParent(transform.root); // Parent to canvas
            dragGhost.transform.SetAsLastSibling();

            Image ghostImage = dragGhost.AddComponent<Image>();
            ghostImage.sprite = itemImage.sprite;
            ghostImage.color = new Color(1, 1, 1, 0.8f);
            byte originalAlpha = 255;
            // set transparency 
            
            ghostImage.raycastTarget = false; // Important: Let raycast pass through to target

            RectTransform rect = dragGhost.GetComponent<RectTransform>();
            rect.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            rect.position = transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (thisItem != null && dragGhost != null)
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
        if (eventData.pointerDrag != null)
        {
            // 1. Kéo từ Inventory Slot khác sang (Swap)
            InventorySlot otherSlot = eventData.pointerDrag.GetComponent<InventorySlot>();
            if (otherSlot != null && otherSlot != this && otherSlot.thisManager == thisManager)
            {
                // Perform swap
                if (thisManager.SwapItems(otherSlot.index, this.index))
                {
                    // Fix duplication: Destroy the old dragged object because SwapItems rebuilds the UI
                    Destroy(eventData.pointerDrag);
                }
                return;
            }

            // 2. Kéo từ EquippedSlot về Inventory
            EquippedSlot equippedSlot = eventData.pointerDrag.GetComponent<EquippedSlot>();
            if (equippedSlot != null)
            {
                EquippedManager equippedManager = FindObjectOfType<EquippedManager>();
                if (equippedManager != null)
                {
                    // Tìm index của slot equipment
                    int equipIndex = System.Array.IndexOf(equippedManager.equippedSlots, equippedSlot);
                    if (equipIndex != -1)
                    {
                        equippedManager.UnEquipItem(equipIndex);
                    }
                }
            }
        }
    }

    public void RemoveItem()
    {
        thisItem = null;
        itemImage.enabled = false;
        itemNumberText.enabled = false;
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
