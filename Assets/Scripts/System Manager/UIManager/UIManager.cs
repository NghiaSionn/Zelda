using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject pausePanel;
    public GameObject inventoryPanel;
    public GameObject shopPanel;
    public GameObject profilePanel;
    public GameObject skillPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Để UIManager tồn tại giữa các scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CloseAllPanels();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                ClosePausePanel();
            }
            else
            {
                OpenPausePanel();
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!pausePanel.activeSelf)
            {
                ToggleInventoryPanel();
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (profilePanel.activeSelf)
            {
                Debug.Log("hồ sơ nhận vật");
                ToggleProfilePanel();
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (skillPanel.activeSelf)
            {
                Debug.Log("kỹ năng nhân vật");
                ToggleSkillPanel();
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
        Time.timeScale = 0; // Dừng thời gian khi pause
    }

    public void ClosePausePanel()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1; // Tiếp tục thời gian
    }

    public void ToggleInventoryPanel()
    {
        if (inventoryPanel.activeSelf)
        {
            CloseAllPanels();
        }
        else
        {
            CloseAllPanels();
            inventoryPanel.SetActive(true);
        }
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

}