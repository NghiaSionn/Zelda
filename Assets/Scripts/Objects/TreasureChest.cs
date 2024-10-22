using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreasureChest : Interactable
{
    public Item contents;
    public Inventory playerInventory;
    public bool isOpen;
    public SignalSender raiseItem;
    public GameObject dialogBox;
    public TextMeshProUGUI diablogText;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange )
        {
            if(!isOpen)
            {
                OpenChest();
            }
            else
            {
                ChestIsOpen();
            }
        }
    }

    public void OpenChest()
    {
        dialogBox.SetActive(true);
        diablogText.text = contents.itemDescription;
        playerInventory.AddItem(contents);
        playerInventory.currentItem = contents;
        raiseItem.Raise();               
        isOpen = true;
        context.Raise();
        anim.SetBool("opened", true);
        isOpen = true;
    }

    public void ChestIsOpen()
    {
        if (!isOpen)
        { 
            dialogBox.SetActive(false);
            raiseItem.Raise();
            playerInRange = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger && !isOpen)
        {
            context.Raise();
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger && !isOpen)
        {
            context.Raise();
            playerInRange = false;
        }
    }
}
