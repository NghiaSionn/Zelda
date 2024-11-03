using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public RoomManager roomManager;

    [Header("Boss settings")]
    public GameObject bossPrefab;


    [Header("Enemy Spawning")]
    public List<GameObject> enemyPrefabs;
    public int minEnemyCountInRoom = 1;
    public int maxEnemyCountInRoom = 5;
    public int maxEnemyCountInDungeon = 5;
    [Range(0, 100)] public int chanceCreateEnemies = 20;
    private int totalEnemyCount = 0;

    public void GenerateEnemies()
    {
        foreach (Room room in roomManager.rooms)
        {
            DetermineRoomEnemies(room);
        }
    }

    private void DetermineRoomEnemies(Room room)
    {
        switch (room.type)
        {
            case RoomType.Treasure:
                room.SetEnemyCount(0);
                break;

            case RoomType.Boss:
                CreateBoss();
                room.SetEnemyCount(1);
                break;

            case RoomType.Normal:
                if (Random.Range(0, 100) < chanceCreateEnemies)
                {
                    int enemyCount = Random.Range(minEnemyCountInRoom, maxEnemyCountInRoom + 1);
                    CreateEnemies(enemyCount, room);
                    room.SetEnemyCount(enemyCount);
                }
                else
                {
                    room.SetEnemyCount(0);
                }
                break;
        }
    }

    private void CreateBoss()
    {
        var bossRoom = roomManager.rooms.Last();
        Instantiate(bossPrefab, bossRoom.GetCenter(), Quaternion.identity, this.transform);
    }

    private void CreateEnemies(int enemyCount, Room room)
    {
        var roomFloorPositions = room.GetFloor();

        for (int i = 0; i < enemyCount; i++)
        {
            if (totalEnemyCount < maxEnemyCountInDungeon)
            {
                Vector2 randomFloorPosition = roomFloorPositions[Random.Range(0, roomFloorPositions.Count)];
                Vector3 spawnPosition = new Vector3(randomFloorPosition.x + 0.5f, randomFloorPosition.y + 0.5f, 0);
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, this.transform);
                totalEnemyCount++;
            }
        }

    }
}