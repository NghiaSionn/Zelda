using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public RoomManager roomManager;
    public EnemyManager enemyManager;
    public DoorManager doorManager;

    void Awake()
    {
        roomManager = FindFirstObjectByType<RoomManager>();
        enemyManager = FindFirstObjectByType<EnemyManager>();
        doorManager = FindFirstObjectByType<DoorManager>();
    }

    void Start()
    {
        roomManager.GenerateRooms();
        enemyManager.GenerateEnemies();
        if(!roomManager.isRandomWalk) doorManager.GenerateDoors();
    }
}
