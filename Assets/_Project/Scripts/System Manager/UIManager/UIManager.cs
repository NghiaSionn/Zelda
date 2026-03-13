using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject inventoryPanel;
    public GameObject shopPanel;
    public GameObject profilePanel;
    public GameObject skillPanel;
    public GameObject mapPanel;

    [Header("Book Animation")]
    public GameObject bookObject;           // GameObject có Animator của cuốn sách
    public float flipDuration = 0.5f;       // Thời gian animation book_fip1
    public float openCloseDuration = 0.8f;  // Thời gian animation open/close

    private Animator bookAnimator;
    private bool isBookAnimating = false;

    // Panel đang hiện (null = không có panel nào mở)
    private GameObject currentActivePanel = null;

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

        if (bookObject != null)
        {
            bookObject.SetActive(false);
            bookAnimator = bookObject.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (isBookAnimating) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
            {
                ClosePausePanel();
            }
            else if (bookObject != null && bookObject.activeSelf)
            {
                // Sách đang mở → đóng sách trước
                StartCoroutine(CloseBookSequence());
            }
            else
            {
                OpenPausePanel();
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.I) && !pausePanel.activeSelf)
            RequestPanel(inventoryPanel);

        if (Input.GetKeyDown(KeyCode.U) && !pausePanel.activeSelf)
            RequestPanel(profilePanel);

        if (Input.GetKeyDown(KeyCode.O) && !pausePanel.activeSelf)
            RequestPanel(skillPanel);

        if (Input.GetKeyDown(KeyCode.M) && !pausePanel.activeSelf)
            RequestPanel(mapPanel);

        if (Input.GetKeyDown(KeyCode.E) && !pausePanel.activeSelf)
        {
            NPCSellItem shop = FindObjectOfType<NPCSellItem>();
            if (shop != null && shop.playerInRange)
                RequestPanel(shopPanel);
        }
    }

    /// <summary>
    /// Điểm vào duy nhất để mở panel. Xử lý 3 trường hợp:
    /// - Sách chưa mở → mở sách (open anim) rồi hiện panel
    /// - Sách đang mở panel khác → flip trang (book_fip1 anim) rồi đổi panel
    /// - Bấm lại panel đang mở → đóng sách (close anim)
    /// </summary>
    private void RequestPanel(GameObject targetPanel)
    {
        if (isBookAnimating) return;

        if (!bookObject.activeSelf)
        {
            // Sách chưa mở → mở lần đầu
            StartCoroutine(OpenBookSequence(targetPanel));
        }
        else if (currentActivePanel == targetPanel)
        {
            // Bấm lại cùng panel → đóng sách
            StartCoroutine(CloseBookSequence());
        }
        else
        {
            // Đang ở panel khác → flip sang panel mới
            StartCoroutine(FlipToPanel(targetPanel));
        }
    }

    // ─── Mở sách lần đầu ───────────────────────────────────────────────────
    private IEnumerator OpenBookSequence(GameObject targetPanel)
    {
        isBookAnimating = true;
        HideAllPanelContent();

        bookObject.SetActive(true);
        if (bookAnimator != null) bookAnimator.Play("open_book", -1, 0f);
        AudioManager.PlaySound("BOOK_OPEN", transform.position);

        yield return new WaitForSeconds(openCloseDuration);

        ShowPanel(targetPanel);
        isBookAnimating = false;
    }

    // ─── Đóng sách ─────────────────────────────────────────────────────────
    private IEnumerator CloseBookSequence()
    {
        isBookAnimating = true;
        HideAllPanelContent();

        if (bookAnimator != null) bookAnimator.Play("close_book", -1, 0f);
        AudioManager.PlaySound("BOOK_CLOSE", transform.position);

        yield return new WaitForSeconds(openCloseDuration);

        bookObject.SetActive(false);
        currentActivePanel = null;
        isBookAnimating = false;
    }

    // ─── Flip sang panel khác (đã mở sách) ─────────────────────────────────
    private IEnumerator FlipToPanel(GameObject targetPanel)
    {
        isBookAnimating = true;
        HideAllPanelContent();

        // Phát animation lật trang trực tiếp bằng Play (không cần nối transition)
        if (bookAnimator != null) bookAnimator.Play("book_flip1", -1, 0f);
        AudioManager.PlaySound("BOOK_FLIP", transform.position);

        yield return new WaitForSeconds(flipDuration);

        ShowPanel(targetPanel);
        isBookAnimating = false;
    }

    // ─── Helpers ────────────────────────────────────────────────────────────
    private void HideAllPanelContent()
    {
        inventoryPanel.SetActive(false);
        shopPanel.SetActive(false);
        skillPanel.SetActive(false);
        profilePanel.SetActive(false);
        mapPanel.SetActive(false);
    }

    private void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
        currentActivePanel = panel;
    }

    // ─── Pause (không dùng book) ────────────────────────────────────────────
    public void OpenPausePanel()
    {
        CloseAllPanels();
        pausePanel.SetActive(true);
    }

    public void ClosePausePanel()
    {
        pausePanel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        pausePanel.SetActive(false);
        HideAllPanelContent();
        if (bookObject != null) bookObject.SetActive(false);
        currentActivePanel = null;
    }

    // ─── Public toggle methods (dùng từ nút bấm UI nếu cần) ────────────────
    public void ToggleInventoryPanel() => RequestPanel(inventoryPanel);
    public void ToggleProfilePanel()   => RequestPanel(profilePanel);
    public void ToggleSkillPanel()     => RequestPanel(skillPanel);
    public void ToggleMapPanel()       => RequestPanel(mapPanel);
    public void ToggleShopPanel()      => RequestPanel(shopPanel);
}
