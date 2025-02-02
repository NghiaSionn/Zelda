using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class CaveLevelManager : MonoBehaviour
{
    [SerializeField] private CaveManager caveManager;
    [SerializeField] private OreManager oreManager;
    [SerializeField] private CaveEnemyManager caveEnemyManager;
    [SerializeField] private SceneTransitions sceneTransitions;

    private Dictionary<int, int[,]> caveMapDicts = new();
    internal Dictionary<int, Dictionary<Vector2, OreData>> oreDataDicts = new();
    internal Dictionary<int, Dictionary<Vector2, GameObject>> enemyDicts = new();
    private Dictionary<int, (Vector3 stairUp, Vector3 stairDown)> stairPositionDicts = new();

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
            caveEnemyManager = FindObjectOfType<CaveEnemyManager>();
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
        Debug.Log("Current: " + level);
        if (!caveMapDicts.ContainsKey(level) && !oreDataDicts.ContainsKey(level)
            && !stairPositionDicts.ContainsKey(level) && !enemyDicts.ContainsKey(level))
        {
            caveManager.GenerateMap();
            oreManager.GenerateOres(level);
            caveEnemyManager.GenerateEnemies(level);

            caveMapDicts[level] = caveManager.GetMap();
            stairPositionDicts[level] = caveManager.GetStairsPosition();
            oreDataDicts[level] = oreManager.GetOres();
            enemyDicts[level] = caveEnemyManager.GetEnemies();
        }
        else
        {
            caveManager.LoadMap(caveMapDicts[level]);
            caveManager.SetStairsPosition(stairPositionDicts[level]);
            oreManager.LoadOres(oreDataDicts[level]);
            caveEnemyManager.LoadEnemies(enemyDicts[level]);
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
            GenerateLevel(newLevel);

            if(newLevel < currentLevel)
                caveManager.SetPlayerPosition(stairPositionDicts[newLevel].stairDown - new Vector3(0, 0.5f, 0));
            else
                caveManager.SetPlayerPosition(stairPositionDicts[newLevel].stairUp - new Vector3(0, 0.5f, 0));

            currentLevel = newLevel;
        }
    }
}