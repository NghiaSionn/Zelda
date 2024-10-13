using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceManager : MonoBehaviour
{
    [Header("Resource settings")]
    [SerializeField] private List<ResourceData> resourcesData;
    [SerializeField] private int minDistanceBetweenResources;
    [SerializeField] private int maxResourcesCount;
    private Dictionary<Vector2, ResourceData> resourceDicts = new();

    [SerializeField] private CaveManager caveManager;

    internal void GenerateResources(bool isExistLevel)
    {
        ClearResources();

        int[,] caveMap = caveManager.GetMap();

        if (!isExistLevel)
        {
            resourceDicts.Clear();

            for (int x = 0; x < caveMap.GetLength(0); x++)
            {
                for (int y = 0; y < caveMap.GetLength(1); y++)
                {
                    if (caveMap[x, y] == 0)
                    {
                        Vector3 newPos = new Vector3(x + 0.5f, y + 0.5f, 0);
                        Vector3 center = new Vector3(-caveMap.GetLength(0) / 2, -caveMap.GetLength(1) / 2, 0);

                        if (!IsResourcesTooClose(newPos))
                        {
                            int chance = Random.Range(0, 100);
                            int cumulativeChance = 0;

                            for (int i = 0; i < resourcesData.Count; i++)
                            {
                                cumulativeChance += resourcesData[i].chance;

                                if (chance < cumulativeChance && resourceDicts.Count < maxResourcesCount)
                                {
                                    var resource = Instantiate(resourcesData[i].prefab, newPos + center, Quaternion.identity, this.transform);
                                    resourceDicts[resource.transform.position] = resourcesData[i];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var positionResource in resourceDicts.Keys)
            {
                Instantiate(resourceDicts[positionResource].prefab, positionResource, Quaternion.identity, this.transform);
            }

            resourceDicts.Clear();
        }
    }

    private bool IsResourcesTooClose(Vector3 newPos)
    {
        foreach (var oldPos in resourceDicts.Keys)
        {
            if (Vector2.Distance(oldPos, newPos) < minDistanceBetweenResources)
            {
                return true;
            }
        }
        return false;
    }

    private void ClearResources()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

    internal Dictionary<Vector2, ResourceData> GetResources() => new(resourceDicts);

    internal void LoadResources(Dictionary<Vector2, ResourceData> savedResources)
    {
        resourceDicts = new(savedResources);
        GenerateResources(true);
    }
}