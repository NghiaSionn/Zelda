using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaWheel : MonoBehaviour
{
    [Header("Thông số Stamina")]
    public float stamina;
    public float maxStamina;

    [Header("Thông số Mana")]
    public float mana;
    public float maxMana;

    [Header("UI")]
    public Slider staminaWheel;
    public Slider usageWheel;
    public Slider manaSlider;

    private bool isRunning = false;
    private bool staminaDepleted = false;
    private GameObject player;

    [Header("Cài đặt thời gian ẩn")]
    public float hideDelay = 2f;

    private Coroutine hideCoroutine;

    void Start()
    {
        // Thiết lập giá trị ban đầu
        stamina = maxStamina;
        mana = maxMana;

        staminaWheel.gameObject.SetActive(false);
        usageWheel.gameObject.SetActive(false);
        manaSlider.gameObject.SetActive(false);

        manaSlider.maxValue = maxMana / 2f;
        manaSlider.value = mana / 2f;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        
        isRunning = player.GetComponent<PlayerMovement>().isRunning;

        // Xử lý logic Stamina
        if (isRunning && !staminaDepleted)
        {
            if (stamina > 0)
            {
                stamina -= 20 * Time.deltaTime;
            }

            if (stamina <= 0)
            {
                stamina = 0;
                staminaDepleted = true; 
                player.GetComponent<PlayerMovement>().canRun = false; 
            }
        }
        else
        {
            if (stamina < maxStamina)
            {
                stamina += 15 * Time.deltaTime; 
            }

            if (stamina >= maxStamina && staminaDepleted)
            {
                staminaDepleted = false; 
                player.GetComponent<PlayerMovement>().canRun = true;
            }
        }

        // Cập nhật UI Stamina
        staminaWheel.value = stamina / maxStamina;
        usageWheel.value = stamina / maxStamina + 0.05f;

        // Xử lý logic Mana 
        if (mana < maxMana)
        {
            mana += 1 * Time.deltaTime; 
            UpdateManaUI();
        }

        // Hiển thị cả hai thanh khi sử dụng năng lượng
        if (isRunning || mana < maxMana)
        {
            ShowUI();
        }

        // Ẩn thanh nếu cả Stamina và Mana đầy
        if (stamina >= maxStamina && mana >= maxMana && hideCoroutine == null)
        {
            hideCoroutine = StartCoroutine(HideUIAfterDelay());
        }
    }

    public bool UseMana(int manaCost)
    {
        if (mana >= manaCost)
        {
            mana -= manaCost;
            UpdateManaUI();
            ShowUI();
            return true;
        }
        else
        {
            Debug.Log("Không đủ mana!");
            return false;
        }
    }

    private void UpdateManaUI()
    {
        manaSlider.value = mana / 2f;
    }

    private void ShowUI()
    {
        // Hiển thị cả hai thanh Stamina và Mana
        staminaWheel.gameObject.SetActive(true);
        usageWheel.gameObject.SetActive(true);
        manaSlider.gameObject.SetActive(true);

        // Nếu có coroutine ẩn, dừng lại để giữ UI hiển thị
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    private IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        // Ẩn cả hai thanh Stamina và Mana
        staminaWheel.gameObject.SetActive(false);
        usageWheel.gameObject.SetActive(false);
        manaSlider.gameObject.SetActive(false);
    }
}
