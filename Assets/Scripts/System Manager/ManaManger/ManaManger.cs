using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManaManger : MonoBehaviour
{
    [Header("Thông số")]
    public float mana;
    public float maxMana;

    [Header("UI")]
    public Slider manaSlider;

    [Header("Cài đặt thời gian ẩn")]
    public float hideDelay = 2f; 

    private Coroutine hideCoroutine;

    void Start()
    {
        mana = maxMana;
        manaSlider.maxValue = maxMana / 2f; 
        manaSlider.value = mana / 2f;
        manaSlider.gameObject.SetActive(false); 
    }

   
    public bool UseMana(int manaCost)
    {
        if (mana >= manaCost)
        {
            mana -= manaCost;
            UpdateManaUI();
            ShowManaUI(); 
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

 
    private void ShowManaUI()
    {
        manaSlider.gameObject.SetActive(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        
        hideCoroutine = StartCoroutine(HideManaUIAfterDelay());
    }

    
    private IEnumerator HideManaUIAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        manaSlider.gameObject.SetActive(false);
    }
}
