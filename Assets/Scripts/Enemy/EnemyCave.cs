using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCave : Enemy
{
    private EnemyAI enemyAI;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();

        List<Vector2Int> walkablePositions = ConvertMapToWalkablePositions(CaveLevelManager.Instance.caveMapDicts[CaveLevelManager.Instance.currentLevel]);
        enemyAI.InitializeWanderSpots(walkablePositions);
    }

    private List<Vector2Int> ConvertMapToWalkablePositions(int[,] map)
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
