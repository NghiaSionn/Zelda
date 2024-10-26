using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public RoomManager roomManager;

    [Header("Boss settings")]
    public GameObject bossPrefab;

    public void GenerateEnemies()
    {
        CreateBoss();
    }

    private void CreateBoss()
    {
        var bossRoom = roomManager.rooms[roomManager.rooms.Count - 1];
        Vector3 bossSpawnPosition = new Vector3(bossRoom.position.x + roomManager.roomWidth / 2 + 0.5f,
                                                 bossRoom.position.y + roomManager.roomHeight / 2 + 0.5f, 0);
        Instantiate(bossPrefab, bossSpawnPosition, Quaternion.identity, this.transform);
    }
}
