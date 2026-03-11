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
    private bool isDashing = false;
    private bool staminaDepleted = false;
    private bool hasDashed = false;
    private GameObject player;

    [Header("Cài đặt thời gian ẩn")]
    public float hideDelay = 2f;

    private Coroutine hideCoroutine;

    void Awake()
    {
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
        isDashing = player.GetComponent<PlayerMovement>().isDashing;

        if (isDashing && !staminaDepleted && !hasDashed)
        {
            if (stamina >= 10)
            {
                stamina -= 10;
                hasDashed = true;
            }
        }
        else if (!isDashing)
        {
            hasDashed = false;
        }

        if (isRunning && !staminaDepleted)
        {
            if (stamina > 0)
            {
                stamina -= 20 * Time.deltaTime;
            }
        }

        if (stamina <= 0)
        {
            stamina = 0;
            staminaDepleted = true;
            player.GetComponent<PlayerMovement>().canDash = false;
            player.GetComponent<PlayerMovement>().canRun = false;
        }

        if (stamina < maxStamina)
        {
            stamina += 15 * Time.deltaTime;
        }

        if (stamina >= maxStamina && staminaDepleted)
        {
            staminaDepleted = false;
            player.GetComponent<PlayerMovement>().canRun = true;
            player.GetComponent<PlayerMovement>().canDash = true;
        }

        staminaWheel.value = stamina / maxStamina;
        usageWheel.value = stamina / maxStamina + 0.05f;

        if (mana < maxMana)
        {
            mana += 1 * Time.deltaTime;
            UpdateManaUI();
        }

        if (isRunning || isDashing || mana < maxMana)
        {
            ShowUI();
        }

        if (stamina >= maxStamina && mana >= maxMana && hideCoroutine == null)
        {
            hideCoroutine = StartCoroutine(HideUIAfterDelay());
        }
    }

    public bool CanStartSkill(float manaCost)
    {
        if (mana >= manaCost)
        {
            return true;
        }
        else
        {
            Debug.Log("Không đủ mana để bắt đầu vận chiêu!");
            return false;
        }
    }

    public bool ConsumeMana(float manaAmount)
    {
        if (mana >= manaAmount)
        {
            mana -= manaAmount;
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

    public void RestoreMana(int manaAmount)
    {
        mana += manaAmount;
        mana = Mathf.Clamp(mana, 0f, maxMana);
        UpdateManaUI();
        ShowUI();
    }

    private void UpdateManaUI()
    {
        manaSlider.value = mana / 2f;
    }

    private void ShowUI()
    {
        staminaWheel.gameObject.SetActive(true);
        usageWheel.gameObject.SetActive(true);
        manaSlider.gameObject.SetActive(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    private IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        staminaWheel.gameObject.SetActive(false);
        usageWheel.gameObject.SetActive(false);
        manaSlider.gameObject.SetActive(false);
    }
}