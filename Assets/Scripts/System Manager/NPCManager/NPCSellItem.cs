using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSellItem : Interactable
{
    [Header("Shop")]
    public GameObject playerInventory;
    public bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        playerInventory.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            if (!isOpen)
            {
                OpenShop();
            }
            else
            {
                CloseShop();
            }
        }
    }

    public void OpenShop()
    {
       isOpen = true;
       playerInRange = true;
       playerInventory.SetActive(true);

    }

    public void CloseShop()
    {
        isOpen = false;
        playerInventory.SetActive(false);
    }

  
}
