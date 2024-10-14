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

    internal void GenerateResources(int currentLevel, bool isGenerateResources)
    {
        ClearResources();

        int[,] caveMap = caveManager.GetMap();

        if (!isGenerateResources)
        {
            resourceDicts.Clear();

            while (resourceDicts.Count < maxResourcesCount)
            {
                int x = Random.Range(1, caveMap.GetLength(0));
                int y = Random.Range(1, caveMap.GetLength(1));

                if (caveMap[x, y] == 0 && caveMap[x, y] != 3)
                {
                    Vector3 newPos = new Vector3(x + 0.5f, y + 0.5f, 0);

                    if (!IsResourcesTooClose(newPos))
                    {
                        int chance = Random.Range(0, 100);
                        int cumulativeChance = 0;

                        for (int i = 0; i < resourcesData.Count; i++)
                        {
                            if (resourcesData[i].IsLevelAllow(currentLevel))
                            {
                                cumulativeChance += resourcesData[i].resoureChance;

                                if (chance < cumulativeChance)
                                {
                                    var resource = Instantiate(resourcesData[i].resourcePrefab, newPos, Quaternion.identity, this.transform);
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
                Instantiate(resourceDicts[positionResource].resourcePrefab, positionResource, Quaternion.identity, this.transform);
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
        GenerateResources(0, true); //any level
    }
}