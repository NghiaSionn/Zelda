using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public RoomManager dungeonManager;
    public EnemyManager enemyManager;

    void Awake()
    {
        dungeonManager = FindFirstObjectByType<RoomManager>();
        enemyManager = FindFirstObjectByType<EnemyManager>();
    }

    void Start()
    {
        dungeonManager.GenerateDungeon();
        enemyManager.GenerateEnemies();
    }
}
