using System.Collections;
using System.Collections.Generic;
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
    public FishingLineController fishingLineController;

    public AudioSource fishingAudioSource;
    public bool isFishing;
    private bool canFish = false;

    void Awake()
    {
        fishingMiniGame.SetActive(false);
        lootUIManager = FindObjectOfType<LootUIManager>();
        fishingLineController = FindAnyObjectByType<FishingLineController>();

        if (playerMovement == null)
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && playerInRange && !isFishing)
        {
            Debug.Log("Bắt đầu câu cá");
            StartCoroutine(StartFishingWithDelay());
        }
    }

    IEnumerator StartFishingWithDelay()
    {
        isFishing = true;
        playerMovement.enabled = false;
        playerAnimator.SetTrigger("startfishing");

        yield return new WaitForSeconds(0.5f);
        // Lấy hướng di chuyển từ PlayerMovement
        Vector2 facingDirection = playerMovement.GetFacingDirection();

        // Kích hoạt dây câu
        fishingLineController.ActiveLine();

        // Vẽ dây câu theo hướng di chuyển của Player
        fishingLineController.DrawFishingLine(facingDirection.x, facingDirection.y);

        float waitTime = Random.Range(5f, 20f);
        yield return new WaitForSeconds(waitTime);

        fishingMiniGame.SetActive(true);
        fishMinigame.ResetGame();
        fishingAudioSource.Play();

        yield return new WaitUntil(() => fishMinigame.IsComplete);

        fishingAudioSource.Stop();
        fishingMiniGame.SetActive(false);

        if (fishMinigame.IsWin)
        {
            GenerateLoot();
        }

        StartCoroutine(EndFishing());
    }



    IEnumerator EndFishing()
    {
        playerMovement.enabled = true;
        isFishing = false;
        playerAnimator.SetTrigger("endfishing");

        // Ẩn dây câu
        fishingLineController.DisableLine();

        yield return null;
    }


    private void GenerateLoot()
    {
        int lootCount = Random.Range(fishingLootData.lootCountRange.x, fishingLootData.lootCountRange.y + 1);

        List<LootBagData.LootItem> tempItems = new List<LootBagData.LootItem>(fishingLootData.lootItems);
        tempItems.Shuffle();

        List<LootBagData.LootItem> selectedLoot = new List<LootBagData.LootItem>();

        for (int i = 0; i < lootCount && i < tempItems.Count; i++)
        {
            LootBagData.LootItem lootItem = tempItems[i];
            int quantity = Random.Range(lootItem.quantityRange.x, lootItem.quantityRange.y + 1);
            lootItem.quantity = quantity;
            selectedLoot.Add(lootItem);
            Debug.Log($"Nhặt được {quantity} {lootItem.item.itemName}.");
            playerInventory.AddItem(lootItem.item, quantity);
        }

        lootUIManager.DisplayLoot(selectedLoot);
    }

  
}