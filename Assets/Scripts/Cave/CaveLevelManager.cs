using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveLevelManager : MonoBehaviour
{
    [SerializeField] private CaveManager caveManager;
    [SerializeField] private OreManager oreManager;

    private static CaveLevelManager instance;
    private Dictionary<int, int[,]> caveMapDicts = new();
    internal Dictionary<int, Dictionary<Vector2, OreData>> oreDataDicts = new();
    internal int currentLevel = 1;
    internal static event Action OnLevelChanged;

    public static CaveLevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CaveLevelManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<CaveLevelManager>();
                    singletonObject.name = typeof(CaveLevelManager).Name + " (Singleton)";
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
            ChangeLevel(currentLevel + 1);
        }
        if (Input.GetKeyDown(KeyCode.X) && currentLevel > 1)
        {
            ChangeLevel(currentLevel - 1);
        }
    }

    private void GenerateLevel(int level)
    {
        Debug.Log("Current: " + currentLevel);
        if (!caveMapDicts.ContainsKey(level) || !oreDataDicts.ContainsKey(level))
        {
            caveManager.GenerateMap();
            caveMapDicts[level] = caveManager.GetMap();

            oreManager.GenerateOre(level, false);
            oreDataDicts[level] = oreManager.GetOres();
        }
        else
        {
            caveManager.LoadMap(caveMapDicts[level]);
            oreManager.LoadOres(oreDataDicts[level]);
        }

        OnLevelChanged?.Invoke();
    }

    public void ChangeLevel(int newLevel)
    {
        if (newLevel <= 0)
        {
            Debug.Log("Invalid level number. Must be greater than 0.");
            SceneManager.LoadScene("Map1");
            return;
        }

        if (newLevel != currentLevel)
        {
            currentLevel = newLevel;
            GenerateLevel(newLevel);
        }
    }
}