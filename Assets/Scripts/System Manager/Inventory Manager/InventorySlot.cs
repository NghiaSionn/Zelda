using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (thisItem != null)
        {
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (thisItem != null)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void RemoveItem()
    {
        thisItem = null;
        itemImage.enabled = false;
        itemNumberText.enabled = false;
    }
}
