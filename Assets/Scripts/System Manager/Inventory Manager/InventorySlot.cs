using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    void Start()
    {
        
    }

    public void Setup(Item newItem, InventoryManager newManager)
    {
        thisItem = newItem;
        thisManager = newManager;

        if (thisItem)
        {
            itemImage.sprite = thisItem.itemSprite;
            itemNumberText.text = thisItem.quantity.ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (thisItem && descriptionPanel && !isPointerInside)
        {
            isPointerInside = true;
            descriptionPanel.SetActive(true);

            //thisManager.SetUpDescriptionAndButton(thisItem.itemDescription,
            //                   thisItem.itemName, thisItem.usable, thisItem);

            Vector3 mousePosition = Input.mousePosition;
            descriptionPanelRect = descriptionPanel.GetComponent<RectTransform>();
            descriptionPanelRect.position = mousePosition + new Vector3(150f, 150f, 0f);
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
            thisManager.SetUpDescriptionAndButton(thisItem.itemDescription,
                thisItem.itemName, thisItem.usable, thisItem);

            thisManager.SetSelectedSlot(gameObject);
        }
    }
}
