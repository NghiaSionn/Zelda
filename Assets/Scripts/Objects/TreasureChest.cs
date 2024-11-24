
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreasureChest : Interactable
{
    [Header("ID Rương")]
    public string chestID;

    [Header("Vật phẩm")]
    public Item contents;

    [Header("Túi đồ người chơi")]
    public Inventory playerInventory;
    public bool isOpen;


    [Header("Data")]
    public SignalSender raiseItem;


    [Header("UI")]
    public GameObject dialogBox;
    public TextMeshProUGUI diablogText;


    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // Kiểm tra trạng thái
        if (PlayerPrefs.GetInt(chestID, 0) == 1) 
        {
            anim.Play("Open_Idle");
            isOpen = true;
        }
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
        isOpen = true;
        Debug.Log("Opening Chest"); 
        dialogBox.SetActive(true);
        diablogText.text = contents.itemDescription;

        playerInventory.AddItem(contents);
        playerInventory.currentItem = contents;

        raiseItem.Raise();               
        
        context.Raise();
        anim.SetBool("opened", true);

        SoundManager.Instance.PlaySound3D("openchest", transform.position);

        PlayerPrefs.SetInt(chestID, 1);

    }

    public void ChestIsOpen()
    {
       
            
            Debug.Log("Rương đã mở");
            dialogBox.SetActive(false);
            raiseItem.Raise();
            playerInRange = false;                 
        
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

    public void ResetChest()
    {
        PlayerPrefs.SetInt(chestID, 0); 
        anim.Play("Close"); 
        isOpen = false;

    }
}
