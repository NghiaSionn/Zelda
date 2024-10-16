using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OreManager : MonoBehaviour
{
    [Header("Resource settings")]
    [SerializeField] private List<OreData> oreData;
    [SerializeField] private int minDistanceBetweenOres;
    [SerializeField] private int maxOresCount;
    private Dictionary<Vector2, OreData> oreDicts = new();

    [SerializeField] private CaveManager caveManager;

    internal void GenerateOre(int currentLevel, bool isGenerateResources)
    {
        ClearOre();

        int[,] caveMap = caveManager.GetMap();

        if (!isGenerateResources)
        {
            oreDicts.Clear();

            while (oreDicts.Count < maxOresCount)
            {
                int x = Random.Range(1, caveMap.GetLength(0));
                int y = Random.Range(1, caveMap.GetLength(1));

                if (caveMap[x, y] == 0 && caveMap[x, y] != 3)
                {
                    Vector3 newPos = new Vector3(x + 0.5f, y + 0.5f, 0);

                    if (!IsOreTooClose(newPos))
                    {
                        int chance = Random.Range(0, 100);
                        int cumulativeChance = 0;

                        for (int i = 0; i < oreData.Count; i++)
                        {
                            if (oreData[i].IsLevelAllow(currentLevel))
                            {
                                cumulativeChance += oreData[i].oreChance;

                                if (chance < cumulativeChance)
                                {
                                    var resource = Instantiate(oreData[i].orePrefab, newPos, Quaternion.identity, this.transform);
                                    oreDicts[resource.transform.position] = oreData[i];
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
            foreach (var positionResource in oreDicts.Keys)
            {
                Instantiate(oreDicts[positionResource].orePrefab, positionResource, Quaternion.identity, this.transform);
            }

            oreDicts.Clear();
        }
    }

    private bool IsOreTooClose(Vector3 newPos)
    {
        foreach (var oldPos in oreDicts.Keys)
        {
            if (Vector2.Distance(oldPos, newPos) < minDistanceBetweenOres)
            {
                return true;
            }
        }
        return false;
    }

    private void ClearOre()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

    internal Dictionary<Vector2, OreData> GetOres() => new(oreDicts);

    internal void LoadOres(Dictionary<Vector2, OreData> savedOres)
    {
        oreDicts = new(savedOres);
        GenerateOre(0, true); //any level
    }
}