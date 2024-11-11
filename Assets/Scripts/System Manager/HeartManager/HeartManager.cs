using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHearts;
    public Sprite halfFullHeart;
    public Sprite emptyHeart;
    public FloatValue heartContainers;
    public FloatValue playerCurrentHealth;


    // Start is called before the first frame update
    void Start()
    {
        InitHearts();
    }

   
    public void InitHearts()
    {
        for (int i = 0; i < heartContainers.initiaValue; i++)
        {
            hearts[i].gameObject.SetActive(true);
            hearts[i].sprite = fullHearts;
        }
    }


    public void UpdateHearts()
    {
        float tempHealth = playerCurrentHealth.RuntimeValue / 2;
        for (int i = 0; i < heartContainers.initiaValue; i++)
        {
            if( i <= tempHealth - 1 )
            {
                //Full Heart
                hearts[i].sprite = fullHearts;
            }
            else if(i >= tempHealth )
            {
                //Emty Heart
                hearts[i].sprite = emptyHeart;
            }
            else
            {
                //Half Full Heart
                hearts[i].sprite = halfFullHeart;
            }
        }
    }
}
