using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class CaveLevelManager : MonoBehaviour
{
    [SerializeField] private CaveManager caveManager;
    [SerializeField] private OreManager oreManager;
    [SerializeField] private SceneTransitions sceneTransitions;

    private Dictionary<int, int[,]> caveMapDicts = new();
    internal Dictionary<int, Dictionary<Vector2, OreData>> oreDataDicts = new();
    private Dictionary<int, Vector3> playerPositionDicts = new();

    internal int currentLevel = 1;
    internal static event Action OnLevelChanged;

    private static CaveLevelManager instance;
    public static CaveLevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CaveLevelManager>();
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Cave")
        {
            caveManager = FindObjectOfType<CaveManager>();
            oreManager = FindObjectOfType<OreManager>();
            sceneTransitions = FindObjectOfType<SceneTransitions>();

            GenerateLevel(currentLevel);
        }
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ChangeLevel(currentLevel + 1);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeLevel(currentLevel - 1);
        }
    }

    private void GenerateLevel(int level)
    {
        Debug.Log("Current: " + currentLevel);
        if (!caveMapDicts.ContainsKey(level) && !oreDataDicts.ContainsKey(level) && !playerPositionDicts.ContainsKey(level))
        {
            caveManager.GenerateMap();
            caveMapDicts[level] = caveManager.GetMap();
            playerPositionDicts[level] = caveManager.GetPlayerPosition();

            oreManager.GenerateOre(level, false);
            oreDataDicts[level] = oreManager.GetOres();
        }
        else
        {
            caveManager.LoadMap(caveMapDicts[level]);
            caveManager.SetPlayerPosition(playerPositionDicts[level]);
            oreManager.LoadOres(oreDataDicts[level]);
        }

        OnLevelChanged?.Invoke();
    }

    public void ChangeLevel(int newLevel)
    {
        if (newLevel <= 0)
        {
            sceneTransitions.playerStorage.initialValue = sceneTransitions.playerPosition;
            sceneTransitions.StartCoroutine(sceneTransitions.FadeCo());
            return;
        }

        if (newLevel != currentLevel)
        {
            currentLevel = newLevel;
            GenerateLevel(newLevel);
        }
    }
}