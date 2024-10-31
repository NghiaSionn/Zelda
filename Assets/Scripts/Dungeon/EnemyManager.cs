using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public RoomManager roomManager;

    [Header("Boss settings")]
    public GameObject bossPrefab;
    private Room bossRoom;


    [Header("Enemy Spawning")]
    public List<GameObject> enemyPrefabs;
    public int minEnemyCountInRoom = 1;
    public int maxEnemyCountInRoom = 5;
    public int maxEnemyCountInDungeon = 5;
    [Range(0, 100)] public int chanceCreateEnemies = 20;
    private int totalEnemyCount = 0;

    public void GenerateEnemies()
    {
        CreateBoss();
        CreateEnemies();
    }

    private void CreateBoss()
    {
        bossRoom = roomManager.rooms[roomManager.rooms.Count - 1];
        Vector3 bossSpawnPosition = new Vector3(bossRoom.position.x + roomManager.roomWidth / 2 + 0.5f,
                                                 bossRoom.position.y + roomManager.roomHeight / 2 + 0.5f, 0);
        Instantiate(bossPrefab, bossSpawnPosition, Quaternion.identity, this.transform);
    }

    private void CreateEnemies()
    {
        var rooms = roomManager.rooms;
        int bossRoomIndex = rooms.Count - 1;

        for (int roomIndex = bossRoomIndex; roomIndex >= 0; roomIndex--)
        {
            int enemyCount = Random.Range(minEnemyCountInRoom, maxEnemyCountInRoom);
            var room = rooms[roomIndex];
            var roomFloorPositions = roomManager.floorPositions
                                     .Where(pos => pos.x >= room.position.x && pos.x < room.position.x + room.width
                                            && pos.y >= room.position.y && pos.y < room.position.y + room.height)
                                     .ToList();

            if (Random.Range(0, 100) > chanceCreateEnemies) continue;
            if (room.position == roomManager.startRoomPosition || room.position == bossRoom.position) continue;

            for (int i = 0; i < enemyCount; i++)
            {
                if (totalEnemyCount < maxEnemyCountInDungeon)
                {
                    Vector3 spawnPosition = new Vector3(room.position.x + Random.Range(0.5f, room.width - 0.5f),
                                                        room.position.y + Random.Range(0.5f, room.height - 0.5f), 0);
                    GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                    GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, this.transform);

                    enemy.GetComponent<EnemyAI>().InitializeWanderSpots(roomFloorPositions);
                    totalEnemyCount++;
                }
            }
        }
    }
}