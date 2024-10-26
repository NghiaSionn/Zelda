using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public RoomManager roomManager;

    [Header("Boss settings")]
    public GameObject bossPrefab;

    public void GenerateEnemy()
    {
        CreateBoss();
    }

    private void CreateBoss()
    {
        var roomPositionsList = roomManager.roomPositionsList;
        var roomWidth  = roomManager.roomWidth;
        var roomHeight = roomManager.roomHeight;
        var bossRoom = roomPositionsList[roomPositionsList.Count - 1];

        Instantiate(bossPrefab, new Vector3(bossRoom.x + roomWidth / 2 + 0.5f, bossRoom.y + roomHeight / 2 + 0.5f, 0),
                                  Quaternion.identity, this.transform);
    }
}
