using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    [SerializeField] public TextMeshProUGUI itemNumberText;
    [SerializeField] private Image itemImage;

    [Header("Thông số vật phẩm")]
    public Item thisItem;
    public InventoryManager thisManager;

    public GameObject descriptionPanel;
    public GameObject selectedPanel;

    private RectTransform descriptionPanelRect;
    private RectTransform selectedPanelRect;

    public bool isPointerInside = false;

    private Transform parentAfterDrag;

    void Start() 
    {
        //itemImage.enabled = false;
        //itemNumberText.enabled = false;
    }

    public void Setup(Item newItem, InventoryManager newManager)
    {
        thisItem = newItem;
        thisManager = newManager;

        if (thisItem)
        {
            itemImage.enabled = true;
            itemImage.sprite = thisItem.itemSprite;

            itemNumberText.enabled = true;
            itemNumberText.text = thisItem.quantity.ToString();
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

    // Bắt đầu kéo
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (thisItem != null)
        {
            parentAfterDrag = transform.parent; // Lưu vị trí cha trước khi kéo
            transform.SetParent(transform.root); // Đưa lên cấp cao nhất để không bị giới hạn
            transform.SetAsLastSibling(); // Đảm bảo hiển thị trên top
            GetComponent<CanvasGroup>().blocksRaycasts = false; // Cho phép raycast đi qua
        }
    }

    // Trong quá trình kéo
    public void OnDrag(PointerEventData eventData)
    {
        if (thisItem != null)
        {
            transform.position = Input.mousePosition;
        }
    }

    // Khi thả vật phẩm
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag); // Đưa về vị trí ban đầu nếu không kéo vào ô hợp lệ
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
