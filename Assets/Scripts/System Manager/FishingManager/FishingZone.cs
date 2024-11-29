﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Rendering;
using UnityEngine;

public class FishingZone : Interactable
{
    public Animator playerAnimator;
    public Inventory playerInventory;

    [Header("Chức năng minigame khi câu cá")]
    public GameObject fishingMiniGame;
    public FishingMiniGame fishMinigame;

    [Header("Loot khi câu cá")]
    public LootBagData fishingLootData; 
    public LootUIManager lootUIManager;

    public PlayerMovement playerMovement;

    public bool isFishing;
    private bool canFish = false;
    // Start is called before the first frame update
    void Start()
    {
        fishingMiniGame.SetActive(false);
        lootUIManager = FindObjectOfType<LootUIManager>();


        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>(); 
        }

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Fishing()
    {
        fishMinigame.ResetGame();    
        isFishing = true;
        StartCoroutine(IsFishing());
        Debug.Log("Fishing: " + playerAnimator.GetBool("fishing"));
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E) && playerInRange && !isFishing)
            {
                Debug.Log("Bắt đầu câu cá");
                Fishing();
            }
            else if (isFishing)
            {
                Debug.Log("Đang câu cá, không thể bắt đầu mới.");
            }
        }
    }

    IEnumerator IsFishing()
    {

        playerMovement.enabled = false;

        isFishing = true;
        playerAnimator.SetBool("fishing", true);
        fishingMiniGame.SetActive(true);

        yield return new WaitUntil(() => fishMinigame.IsComplete);

        fishingMiniGame.SetActive(false);

        if (fishMinigame.IsWin)
        {
            GenerateLoot();
        }

        EndFishing();


    }

    private void GenerateLoot()
    {
        int lootCount = Random.Range(fishingLootData.lootCountRange.x, fishingLootData.lootCountRange.y + 1);

        // Tạo danh sách tạm thời và shuffle
        List<LootBagData.LootItem> tempItems = new List<LootBagData.LootItem>(fishingLootData.lootItems);
        tempItems.Shuffle();

        List<LootBagData.LootItem> selectedLoot = tempItems.GetRange(0, lootCount);

        // Hiển thị UI loot
        lootUIManager.DisplayLoot(selectedLoot);

        foreach (var lootItem in selectedLoot)
        {
            Item item = lootItem.item;
            int quantity = Random.Range(lootItem.quantityRange.x, lootItem.quantityRange.y + 1);

            playerInventory.AddItem(item,quantity);

            switch (item.itemType)
            {
                case Item.ItemType.Fish:
                     playerInventory.fish += quantity;
                      break;
            }

                Debug.Log($"Nhặt được {item.itemName} + {quantity}.");
            
        }
    }

    private void EndFishing()
    {
        playerMovement.enabled = true;
        isFishing = false;
        playerAnimator.SetBool("fishing", false);
    }

    

}
