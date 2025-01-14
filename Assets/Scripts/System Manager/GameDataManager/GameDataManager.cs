using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    private const string SAVE_FILE_NAME = "GameData.json";

    public Inventory inventory;
    public GameTimeData gameTimeData;
    public FloatValue playerHealth;
    public FloatValue playerExp;
    public SignalSender playerHealthSignal;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        if (inventory == null || gameTimeData == null)
        {
            Debug.LogError("Inventory hoặc GameTimeData chưa được gán!");
            return;
        }

        List<ItemData> itemDataList = new List<ItemData>();
        foreach (Item item in inventory.items)
        {
            ItemData itemData = new ItemData
            {
                itemName = item.itemName,
                spriteName = item.itemSprite != null ? item.itemSprite.name : string.Empty, // Lưu tên ảnh
                itemDescription = item.itemDescription,
                quantity = item.quantity,
                usable = item.usable,
                unique = item.unique,
                isKey = item.isKey,
                itemType = item.itemType,
                itemUseType = item.itemUseType,
                healAmount = item.healAmount,
                manaAmount = item.manaAmount,
                priceItems = item.priceItems,
                quantityBuys = item.quantityBuys,
                maxQuantity = item.maxQuantity
            };
            itemDataList.Add(itemData);
        }

        GameData gameData = new GameData
        {
            sceneName = SceneManager.GetActiveScene().name,
            playerHealth = Mathf.Round(playerHealth.RuntimeValue),
            playerExp = Mathf.Round(playerExp.RuntimeValue),
            numberOfKeys = inventory.numberOfKeys,
            coins = inventory.coins,
            meats = inventory.meats,
            logs = inventory.logs,
            fish = inventory.fish,
            dayCount = gameTimeData.dayCount,
            currentTimeString = gameTimeData.currentTimeString,
            isRaining = gameTimeData.isRaining,
            rainEndTimeString = gameTimeData.rainEndTimeString,
            items = itemDataList
        };


        SaveLoadUtility.SaveToJSON(gameData, SAVE_FILE_NAME);
    }

    public void LoadGame()
    {
        GameData loadedData = SaveLoadUtility.LoadFromJSON<GameData>(SAVE_FILE_NAME);
        if (loadedData == null) return;

        // Kiểm tra Scene
        if (SceneManager.GetActiveScene().name != loadedData.sceneName)
        {
            StartCoroutine(LoadSceneAndRestoreData(loadedData));
        }
        else
        {
            RestoreGameData(loadedData);
        }
    }

    private IEnumerator LoadSceneAndRestoreData(GameData loadedData)
    {
        // Chuyển Scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadedData.sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        // Tìm GameDataManager mới trong Scene vừa tải
        GameDataManager newGameDataManager = FindObjectOfType<GameDataManager>();
        if (newGameDataManager != null)
        {
            newGameDataManager.RestoreGameData(loadedData);
        }
    }

    public void RestoreGameData(GameData loadedData)
    {
        inventory.numberOfKeys = loadedData.numberOfKeys;
        inventory.coins = loadedData.coins;
        inventory.meats = loadedData.meats;
        inventory.logs = loadedData.logs;
        inventory.fish = loadedData.fish;

        inventory.items.Clear();
        foreach (ItemData itemData in loadedData.items)
        {
            Item newItem = ScriptableObject.CreateInstance<Item>();
            newItem.itemName = itemData.itemName;

            if (!string.IsNullOrEmpty(itemData.spriteName))
            {
                newItem.itemSprite = Resources.Load<Sprite>($"{itemData.spriteName}");
            }

            newItem.itemDescription = itemData.itemDescription;
            newItem.quantity = itemData.quantity;
            newItem.usable = itemData.usable;
            newItem.unique = itemData.unique;
            newItem.isKey = itemData.isKey;
            newItem.itemType = itemData.itemType;
            newItem.itemUseType = itemData.itemUseType;
            newItem.healAmount = itemData.healAmount;
            newItem.manaAmount = itemData.manaAmount;
            newItem.priceItems = itemData.priceItems;
            newItem.quantityBuys = itemData.quantityBuys;
            newItem.maxQuantity = itemData.maxQuantity;

            inventory.items.Add(newItem);
        }

        gameTimeData.isRaining = loadedData.isRaining;
        gameTimeData.rainEndTimeString = loadedData.rainEndTimeString;
        gameTimeData.dayCount = loadedData.dayCount;
        gameTimeData.currentTimeString = loadedData.currentTimeString;

        playerHealth.SetValue(loadedData.playerHealth);
        playerExp.SetValue(loadedData.playerExp);

        // Đồng bộ với WorldTime
        WorldTime worldTime = FindObjectOfType<WorldTime>();
        if (worldTime != null)
        {
            worldTime.SyncWithGameTimeData(gameTimeData); // Thêm phương thức này trong WorldTime.
        }

        // Đồng bộ với PlayerMovement
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            player.UpdateHealth(loadedData.playerHealth - playerHealth.RuntimeValue);
            player.UpdateExpBar();
        }

        // Kích hoạt tín hiệu cập nhật máu
        playerHealthSignal?.Raise();

        Debug.Log("Game state restored.");
    }
}
