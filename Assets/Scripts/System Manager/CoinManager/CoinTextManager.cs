using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinTextManager : MonoBehaviour
{
    public Inventory playerInventory;
    public TextMeshProUGUI coinDisplay;

    void Start()
    {
        UpdateCoinCount(); 
    }

    public void UpdateCoinCount()
    {
        coinDisplay.text = playerInventory.coins.ToString();
    }
}
