using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class LevelCaveManager : MonoBehaviour
{
    [SerializeField] private CaveManager caveManager;
    [SerializeField] private ResourceManager resourceManager;

    private Dictionary<int, int[,]> caveMapDicts = new();
    private Dictionary<int, Dictionary<Vector2, ResourceData>> resourceDataDicts = new();
    private int currentLevel = 1;

    private void Start()
    {
        GenerateLevel(currentLevel);
    }

    private void GenerateLevel(int level)
    {
        Debug.Log("Current: " + currentLevel);
        if (!caveMapDicts.ContainsKey(level) || !resourceDataDicts.ContainsKey(level))
        {
            caveManager.GenerateMap();
            caveMapDicts[level] = caveManager.GetMap();

            resourceManager.GenerateResources(level, false);
            resourceDataDicts[level] = resourceManager.GetResources();
        }
        else
        {
            caveManager.LoadMap(caveMapDicts[level]);
            resourceManager.LoadResources(resourceDataDicts[level]);
        }
    }

    public void ChangeLevel(int newLevel)
    {
        if (newLevel != currentLevel)
        {
            currentLevel = newLevel;
            GenerateLevel(currentLevel);
        }
    }

    public void PlusLevel() => GenerateLevel(++currentLevel);

    public void MinusLevel() => GenerateLevel(--currentLevel);
}