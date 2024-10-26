using System.Collections;
using System.Collections.Generic;
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
    [Range(0, 100)]public int chanceCreateEnemies = 20;
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
        var startRoomPosition = roomManager.startRoomPosition;

        foreach (var room in rooms)
        {
            int distanceFromStart = CalculateDistance(startRoomPosition, room.position);
            int enemyCount = Mathf.Clamp(minEnemyCountInRoom + distanceFromStart, minEnemyCountInRoom, maxEnemyCountInRoom);

            if(Random.Range(0, 100) > chanceCreateEnemies) continue;
            if(room.position == startRoomPosition || room.position == bossRoom.position) continue;

            for (int i = 0; i < enemyCount; i++)
            {
                if(totalEnemyCount <= maxEnemyCountInDungeon)
                {
                    Vector3 spawnPosition = new Vector3(room.position.x + Random.Range(0.5f, room.width - 0.5f),
                                                    room.position.y + Random.Range(0.5f, room.height - 0.5f), 0);
                    GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                    Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, this.transform);
                    totalEnemyCount++;
                }
            }
        }
    }

    private int CalculateDistance(Vector2Int start, Vector2Int roomPosition)
    {
        return Mathf.Abs(start.x - roomPosition.x) + Mathf.Abs(start.y - roomPosition.y);
    }
}
