using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EnemyDungeon : Enemy
{
    private Room currentRoom;
    private EnemyAI enemyAI;

    void Start()
    {
        base.Awake();
        enemyAI = GetComponent<EnemyAI>();
        FindCurrentRoom();
    }
    private void FindCurrentRoom()
    {
        var roomManager = FindObjectOfType<RoomManager>();
        if (roomManager == null) return;

        Vector2 position = transform.position;

        foreach (Room room in roomManager.rooms)
        {
            bool isInRoomX = position.x >= room.position.x &&
                            position.x < (room.position.x + room.width);
            bool isInRoomY = position.y >= room.position.y &&
                            position.y < (room.position.y + room.height);

            if (isInRoomX && isInRoomY)
            {
                currentRoom = room;
                enemyAI.InitializeWanderSpots(room.GetFloor());
                break;
            }
        }
    }
}