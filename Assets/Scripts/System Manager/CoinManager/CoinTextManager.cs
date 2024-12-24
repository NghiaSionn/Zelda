using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinTextManager : MonoBehaviour
{
    public Inventory playerInventory;
    public TextMeshProUGUI coinDisplay;

    void Awake()
    {
        UpdateCoinCount();
        GameObject coinUI = GameObject.Find("Coin UI Text");
        coinDisplay = coinUI.GetComponent<TextMeshProUGUI>();
    }

    public void UpdateCoinCount()
    {
        coinDisplay.text = playerInventory.coins.ToString();
    }
}
