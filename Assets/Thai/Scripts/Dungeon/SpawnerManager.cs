using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public RoomManager roomManager;

    [Header("Enemy Spawning")]
    public GameObject bossPrefab;
    public List<GameObject> enemyPrefabs;
    public int minEnemiesPerRoom = 1;
    public int maxEnemiesPerRoom = 5;
    public int maxEnemyCount = 5;
    [Range(0, 100)] public int enemiesSpawnChance = 20;
    private int totalEnemyCount = 0;


    [Header("Room Decorations")]
    public List<GameObject> allRoomDecorations;
    public List<GameObject> normalRoomDecorations;
    public List<GameObject> bossRoomDecorations;
    public List<GameObject> treasureRoomDecorations;
    public GameObject treasureChestPrefab;
    public int minDecorationsPerRoom = 2;
    public int maxDecorationsPerRoom = 5;
    [Range(0, 100)] public int decorationSpawnChance = 70;

    private Dictionary<Room, List<GameObject>> roomDecorations = new();

    public void Generate()
    {
        foreach (Room room in roomManager.rooms)
        {
            GenerateRoomContent(room);
        }
    }

    private void GenerateRoomContent(Room room)
    {
        SpawnDecorations(room, allRoomDecorations);

        switch (room.type)
        {
            case RoomType.Boss:
                CreateBoss(room);
                SpawnDecorations(room, bossRoomDecorations);
                break;

            case RoomType.Treasure:
                SpawnDecorations(room, treasureRoomDecorations);
                SpawnTreasureChest(room);
                break;

            case RoomType.Normal:
                if (Random.Range(0, 100) < enemiesSpawnChance)
                {
                    int enemyCount = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
                    CreateEnemies(enemyCount, room);
                }
                SpawnDecorations(room, normalRoomDecorations);
                break;

            case RoomType.Start:
                SpawnDecorations(room, normalRoomDecorations);
                break;
        }
    }

    private void CreateBoss(Room room)
    {
        var boss = Instantiate(bossPrefab, room.GetCenter(), Quaternion.identity, transform);
        room.enemies.Add(boss.GetComponent<EnemyDungeon>());
    }

    private void CreateEnemies(int enemyCount, Room room)
    {
        var roomFloorPositions = room.GetFloor();
        List<Vector2Int> availablePositions = new List<Vector2Int>(roomFloorPositions);

        Vector2Int centerPos = Vector2Int.RoundToInt(room.GetCenter());
        availablePositions.RemoveAll(pos => Vector2Int.Distance(pos, centerPos) < 2);

        foreach (var door in room.doors)
        {
            Vector2Int doorPos = Vector2Int.RoundToInt(door.transform.position);
            availablePositions.RemoveAll(pos => Vector2Int.Distance(pos, doorPos) < 2);
        }

        for (int i = 0; i < enemyCount && totalEnemyCount < maxEnemyCount; i++)
        {
            if (availablePositions.Count == 0) break;

            int randomPosIndex = Random.Range(0, availablePositions.Count);
            Vector2Int spawnPos = availablePositions[randomPosIndex];
            availablePositions.RemoveAt(randomPosIndex);

            Vector3 spawnPosition = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0);
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
            room.enemies.Add(enemy.GetComponent<EnemyDungeon>());
            totalEnemyCount++;

            availablePositions.RemoveAll(pos => Vector2Int.Distance(pos, spawnPos) < 2);
        }
    }

    private void SpawnDecorations(Room room, List<GameObject> decorationPool)
    {
        if (decorationPool == null || decorationPool.Count == 0) return;

        List<GameObject> decorationsForRoom = new List<GameObject>();
        roomDecorations[room] = decorationsForRoom;

        var floorPositions = room.GetFloor();
        List<Vector2Int> availablePositions = new List<Vector2Int>(floorPositions);

        Vector2Int centerPos = Vector2Int.RoundToInt(room.GetCenter());
        availablePositions.RemoveAll(pos => Vector2Int.Distance(pos, centerPos) < 2);

        foreach (var door in room.doors)
        {
            Vector2Int doorPos = Vector2Int.RoundToInt(door.transform.position);
            availablePositions.RemoveAll(pos => Vector2Int.Distance(pos, doorPos) < 2);
        }

        int decorationCount = Random.Range(minDecorationsPerRoom, maxDecorationsPerRoom + 1);

        for (int i = 0; i < decorationCount; i++)
        {
            if (availablePositions.Count == 0) break;
            if (Random.Range(0, 100) >= decorationSpawnChance) continue;

            int randomPosIndex = Random.Range(0, availablePositions.Count);
            Vector2Int spawnPos = availablePositions[randomPosIndex];
            availablePositions.RemoveAt(randomPosIndex);

            GameObject decorPrefab = decorationPool[Random.Range(0, decorationPool.Count)];
            Vector3 spawnPosition = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0);

            GameObject decoration = Instantiate(decorPrefab, spawnPosition, Quaternion.identity, transform);
            decorationsForRoom.Add(decoration);

            availablePositions.RemoveAll(pos => Vector2Int.Distance(pos, spawnPos) < 2);
        }
    }

    private void SpawnTreasureChest(Room room)
    {
        if (treasureChestPrefab == null) return;

        Vector3 centerPos = room.GetCenter();
        GameObject chest = Instantiate(treasureChestPrefab, centerPos, Quaternion.identity, transform);

        if (!roomDecorations.ContainsKey(room))
        {
            roomDecorations[room] = new List<GameObject>();
        }
        roomDecorations[room].Add(chest);
    }
}