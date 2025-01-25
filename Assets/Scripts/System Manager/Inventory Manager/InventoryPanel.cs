using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    public GameObject inventoryPanel;

    private bool isOpen;

    private static InventoryPanel _instance;

    public static InventoryPanel Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError(" InventoryPanel instance is null!");
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        inventoryPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!isOpen)
            {
                OpenPanel();
            }
            else
            {
                ClosePanel();
            }

        }
    }

    private void ClosePanel()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
    }

    private void OpenPanel()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.UpdateInventoryUI();
        }
    }
}
