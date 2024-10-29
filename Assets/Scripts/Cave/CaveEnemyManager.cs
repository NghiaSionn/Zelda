using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CaveEnemyManager : MonoBehaviour
{
    [SerializeField] private CaveManager caveManager;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject enemyPrefab;


    [Header("Enemy settings")]
    [SerializeField] private int minEnemyCount;
    [SerializeField] private int maxEnemyCount;
    [SerializeField] private float distanceFromPlayer = 5f;
    private Dictionary<Vector2, GameObject> enemyDicts = new();

    internal void GenerateEnemies(int level)
    {
        ClearEnemies();

        int[,] caveMap = caveManager.GetMap();

        enemyDicts.Clear();

        while (enemyDicts.Count < maxEnemyCount)
        {
            int x = Random.Range(1, caveMap.GetLength(0));
            int y = Random.Range(1, caveMap.GetLength(1));
            Vector3 randomPosition = new Vector3(x + 0.5f, y + 0.5f, 0);
            int groupSize = Random.Range(minEnemyCount, minEnemyCount + level + 1);

            if (caveMap[x, y] == 0 && IsFarEnoughFromPlayer(randomPosition))
            {
                for (int i = 0; i < groupSize; i++)
                {
                    int spawnX = x + Random.Range(-1, 2);
                    int spawnY = y + Random.Range(-1, 2);

                    if (caveMap[spawnX, spawnY] == 0)
                    {
                        Vector3 spawnPosition = new Vector3(spawnX + 0.5f, spawnY + 0.5f, 0);

                        if (!enemyDicts.ContainsKey(spawnPosition))
                        {
                            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
                            enemyDicts[spawnPosition] = newEnemy;
                        }
                    }
                }
            }
        }
    }

    private void ClearEnemies()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        enemyDicts.Clear();
    }

    private bool IsFarEnoughFromPlayer(Vector2 position)
    {
        float distance = Vector2.Distance(position, player.position);
        return distance >= distanceFromPlayer;
    }


    internal Dictionary<Vector2, GameObject> GetEnemies() => new(enemyDicts);

    internal void LoadEnemies(Dictionary<Vector2, GameObject> savedEnemies)
    {
        ClearEnemies();
        foreach (var enemy in savedEnemies)
        {
            Vector2 position = enemy.Key;
            // GameObject prefab = enemy.Value;
            GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity, transform);
            enemyDicts[position] = newEnemy;
        }
    }
}