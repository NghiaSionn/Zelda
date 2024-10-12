using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceManager : MonoBehaviour
{
    [Header("Resource settings")]
    [SerializeField] private List<ResourceData> resourcesData;

    [SerializeField] private CaveManager caveManager;

    private void Start()
    {
        GenerateResources();
    }

    private void GenerateResources()
    {
        ClearResources();

        int[,] caveMap = caveManager.GetMap();

        for (int x = 0; x < caveMap.GetLength(0); x++)
        {
            for (int y = 0; y < caveMap.GetLength(1); y++)
            {
                if (caveMap[x, y] == 0)
                {
                    Vector3 newPos = new Vector3(x + 0.5f, y + 0.5f, 0);

                    int chance = Random.Range(0, 100);
                    int cumulativeChance = 0;

                    for (int i = 0; i < resourcesData.Count; i++)
                    {
                        cumulativeChance += resourcesData[i].chance;

                        if (chance < cumulativeChance)
                        {
                            Instantiate(resourcesData[i].prefab, newPos, Quaternion.identity, this.transform);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void ClearResources()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
