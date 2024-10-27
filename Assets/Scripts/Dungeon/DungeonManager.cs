using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public RoomManager roomManager;
    public EnemyManager enemyManager;

    void Awake()
    {
        roomManager = FindFirstObjectByType<RoomManager>();
        enemyManager = FindFirstObjectByType<EnemyManager>();
    }

    void Start()
    {
        roomManager.GenerateDungeon();
        enemyManager.GenerateEnemies();
    }
}
