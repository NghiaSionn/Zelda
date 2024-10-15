using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCaveManager : MonoBehaviour
{
    [SerializeField] private CaveManager caveManager;
    [SerializeField] private ResourceManager resourceManager;

    private static LevelCaveManager instance;

    private Dictionary<int, int[,]> caveMapDicts = new();
    internal Dictionary<int, Dictionary<Vector2, ResourceData>> resourceDataDicts = new();
    internal int currentLevel = 1;

    public static LevelCaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelCaveManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<LevelCaveManager>();
                    singletonObject.name = typeof(LevelCaveManager).Name + " (Singleton)";
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GenerateLevel(currentLevel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GenerateLevel(++currentLevel);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GenerateLevel(--currentLevel);
        }
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
            if (currentLevel != 0)
            {
                currentLevel = newLevel;
                GenerateLevel(currentLevel);
            }
            else
            {
                Debug.Log("GOOOOOOOOOO");
                SceneManager.LoadScene("Map1");
            }
        }
    }
}