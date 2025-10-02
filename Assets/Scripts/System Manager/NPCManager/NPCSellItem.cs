using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSellItem : Interactable
{
    [Header("Shop")]
    public GameObject playerInventory;
    public bool isOpen;

    private void Awake()
    {
        playerInventory.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

   
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

    private void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.CompareTag("Player") && playerInRange)
        {
            context.Raise();
            playerInRange = false;
            CloseShop(); 
        }
    }

}
