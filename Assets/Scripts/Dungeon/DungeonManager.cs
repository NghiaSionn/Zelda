using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public RoomManager roomManager;
    public SpawnerManager spawnerManager;

    void Awake()
    {
        roomManager = FindFirstObjectByType<RoomManager>();
        spawnerManager = FindFirstObjectByType<SpawnerManager>();
    }

    void Start()
    {
        roomManager.GenerateRooms();
        spawnerManager.Generate();
    }
}
