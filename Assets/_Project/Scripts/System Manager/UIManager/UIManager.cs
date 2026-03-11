using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject pausePanel;
    public GameObject inventoryPanel;
    public GameObject shopPanel;
    public GameObject profilePanel;
    public GameObject skillPanel;
    public GameObject mapPanel;
    public GameObject bookObject; // Cuốn sách sẽ hiện animation
    
    private Animator inventoryAnimator;
    private Animator bookAnimator;
    private bool isBookAnimating = false; // Ngăn spam input khi đang animation

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CloseAllPanels();
        
        // Đảm bảo inventory panel luôn bị ẩn khi start
        inventoryPanel.SetActive(false);
        
        // Ẩn cuốn sách khi start
        if (bookObject != null)
        {
            bookObject.SetActive(false);
        }
        
        inventoryAnimator = inventoryPanel.GetComponent<Animator>();
        
        if (bookObject != null)
        {
            bookAnimator = bookObject.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                // ESC lần 2 (pause đang mở) → Đóng pause
                ClosePausePanel();
            }
            else if ((bookObject != null && bookObject.activeSelf) || inventoryPanel.activeSelf)
            {
                // Sách/inventory đang mở → Đóng sách bằng animation trước, chưa mở pause
                if (!isBookAnimating)
                {
                    ToggleInventoryPanel();
                }
            }
            else
            {
                // Không có gì đang mở → Mở pause
                OpenPausePanel();
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!pausePanel.activeSelf && !isBookAnimating)
            {
                ToggleInventoryPanel();
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (!pausePanel.activeSelf)
            {
                ToggleProfilePanel();
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (!pausePanel.activeSelf)
            {
                ToggleSkillPanel();
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!pausePanel.activeSelf)
            {
                ToggleMapPanel();
            }
        }

        // Kiểm tra điều kiện playerInRange trước khi mở shop
        if (Input.GetKeyDown(KeyCode.E))
        {
            NPCSellItem shop = FindObjectOfType<NPCSellItem>(); // Tìm shop
            if (shop != null && shop.playerInRange && !pausePanel.activeSelf)
            {
                ToggleShopPanel();
            }
        }       
    }

    public void OpenPausePanel()
    {
        CloseAllPanels();
        pausePanel.SetActive(true);
    }

    public void ClosePausePanel()
    {
        pausePanel.SetActive(false);
    }

    public void ToggleInventoryPanel()
    {
        if (inventoryPanel.activeSelf)
        {
            // Đóng: Ẩn inventory panel trước, sau đó trigger close animation của book
            StartCoroutine(CloseInventorySequence());
        }
        else
        {
            // Mở: Trigger open animation của book, sau đó hiện inventory panel
            StartCoroutine(OpenInventorySequence());
        }
    }
    
    private IEnumerator OpenInventorySequence()
    {
        isBookAnimating = true;
        CloseAllPanels();
        
        // Hiện cuốn sách và trigger animation open
        if (bookObject != null && bookAnimator != null)
        {
            bookObject.SetActive(true);
            bookAnimator.SetTrigger("open");
            
            // Đợi animation open kết thúc (điều chỉnh thời gian theo animation thực tế)
            yield return new WaitForSeconds(0.8f);
        }
        
        // Sau khi animation open kết thúc, hiện inventory panel
        inventoryPanel.SetActive(true);
        isBookAnimating = false;
    }
    
    private IEnumerator CloseInventorySequence()
    {
        isBookAnimating = true;
        
        // Ẩn inventory panel trước
        inventoryPanel.SetActive(false);
        
        // Sau đó trigger close animation của book
        if (bookObject != null && bookAnimator != null)
        {
            bookAnimator.SetTrigger("close");
            
            // Đợi animation close kết thúc
            yield return new WaitForSeconds(0.8f);
            
            // Ẩn cuốn sách sau khi animation kết thúc
            bookObject.SetActive(false);
        }
        
        isBookAnimating = false;
    }

    IEnumerator DisablePanelDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    public void ToggleShopPanel()
    {
        if (shopPanel.activeSelf)
        {
            CloseAllPanels();
        }
        else
        {
            CloseAllPanels();
            shopPanel.SetActive(true);
        }
    }

    public void CloseAllPanels()
    {
        pausePanel.SetActive(false);
        inventoryPanel.SetActive(false);
        shopPanel.SetActive(false);
        skillPanel.SetActive(false);
        profilePanel.SetActive(false);
        mapPanel.SetActive(false);
    }

    public void ToggleProfilePanel()
    {
        if (profilePanel.activeSelf)
        {
            CloseAllPanels();
        }
        else
        {
            CloseAllPanels();
            profilePanel.SetActive(true);
        }
    }

    public void ToggleSkillPanel()
    {
        if (skillPanel.activeSelf)
        {
            CloseAllPanels();
        }
        else
        {
            CloseAllPanels();
            skillPanel.SetActive(true);
        }
    }

    public void ToggleMapPanel()
    {
        if (mapPanel.activeSelf)
        {
            CloseAllPanels();
        }
        else
        {
            CloseAllPanels();
            mapPanel.SetActive(true);
        }
    }

}