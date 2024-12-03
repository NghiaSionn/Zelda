using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
        selectedPanel.SetActive(false);
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

            thisManager.SetUpDescriptionAndButton(thisItem.itemDescription,
                thisItem.itemName, thisItem.usable, thisItem);

            // Cập nhật vị trí của description panel
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
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (thisItem && selectedPanel != null)
            {
                selectedPanel.SetActive(true);

                // Đặt vị trí của selectedPanel
                Vector3 mousePosition = Input.mousePosition;
                selectedPanelRect = selectedPanel.GetComponent<RectTransform>();
                if (selectedPanelRect != null)
                {
                    selectedPanelRect.position = mousePosition + new Vector3(50f, 50f, 0f);                   
                }
                
            }            
        }
    }


    public void ClickedOn()
    {
        if (thisItem)
        {
            thisManager.SetUpDescriptionAndButton(thisItem.itemDescription,
                thisItem.itemName, thisItem.usable, thisItem);
        }
    }
}
