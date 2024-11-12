using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCave : Enemy
{
    private CaveManager caveManager;
    private EnemyAI enemyAI;

    void Start()
    {
        caveManager = FindObjectOfType<CaveManager>();
        enemyAI = GetComponent<EnemyAI>();

        List<Vector2Int> walkablePositions = ConvertMapToListPositions(caveManager.GetMap());
        enemyAI.InitializeWanderSpots(walkablePositions);
    }

    private List<Vector2Int> ConvertMapToListPositions(int[,] map)
    {
        List<Vector2Int> walkablePositions = new List<Vector2Int>();
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == 0)
                {
                    walkablePositions.Add(new Vector2Int(x, y));
                }
            }
        }
        return walkablePositions;
    }
}
