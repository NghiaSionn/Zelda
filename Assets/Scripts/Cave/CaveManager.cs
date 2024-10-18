using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class CaveManager : MonoBehaviour
{
    [Header("Cave settings")]
    [SerializeField][Range(1, 100)] internal int width;
    [SerializeField][Range(1, 100)] internal int height;
    [SerializeField][Range(0, 100)] private int fillWallsPercent = 45;
    [SerializeField] private int smoothIterations = 3;
    [SerializeField] private int minWallsToStayWall = 4; //2-3
    [SerializeField] private int minWallsToBecomeWall = 4; //4-5
    [SerializeField] private int pathWidth = 1;

    [Header("Tile settings")]
    [SerializeField] private Tilemap floorsTilemap; //0
    [SerializeField] private Tilemap wallsTileMap; //1
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;

    [Header("Base settings")]
    [SerializeField] private GameObject basePrefab;
    [SerializeField] private GameObject player;
    public VectorValue startingPosition;

    private int[,] caveMap;
    private AreaManager areaManager;
    private GameObject currentBase;
    private GameObject currentPlayer;

    internal void GenerateMap()
    {
        caveMap = new int[width, height];
        areaManager = GetComponent<AreaManager>();

        FillMap();

        areaManager.Initialize(caveMap, width, height);
        var areas = areaManager.GetAreas();
        ConnectAreasWithPaths(areas);

        DrawMap();
    }

    private void ConnectAreasWithPaths(List<List<Vector2Int>> areas)
    {
        for (int i = 0; i < areas.Count - 1; i++)
        {
            Vector2Int pointA = areas[i][Random.Range(0, areas[i].Count)];
            Vector2Int pointB = areas[i + 1][Random.Range(0, areas[i + 1].Count)];

            FillPath(pointA, pointB);
        }
    }

    private void FillPath(Vector2Int start, Vector2Int end)
    {
        Vector2Int currentPos = start;

        while (currentPos != end)
        {
            if (Random.value < 0.2f) // zigzag
            {
                if (Random.value < 0.5f)
                    currentPos.x += (Random.value < 0.5f) ? 1 : -1;
                else
                    currentPos.y += (Random.value < 0.5f) ? 1 : -1;
            }
            else
            {
                if (Random.value < 0.5f)
                {
                    if (currentPos.x < end.x)
                        currentPos.x++;
                    else if (currentPos.x > end.x)
                        currentPos.x--;
                }
                else
                {
                    if (currentPos.y < end.y)
                        currentPos.y++;
                    else if (currentPos.y > end.y)
                        currentPos.y--;
                }
            }

            caveMap[currentPos.x, currentPos.y] = 0;

            for (int offsetX = -pathWidth; offsetX <= pathWidth; offsetX++)
            {
                for (int offsetY = -pathWidth; offsetY <= pathWidth; offsetY++)
                {
                    int newX = currentPos.x + offsetX;
                    int newY = currentPos.y + offsetY;

                    if (IsInMapRange(newX, newY) && !IsMapBorder(newX, newY))
                    {
                        caveMap[newX, newY] = 0;
                    }
                }
            }
        }
    }
    private void DrawMap()
    {
        floorsTilemap.ClearAllTiles();
        wallsTileMap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caveMap[x, y] == 0)
                {
                    floorsTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
                else if (caveMap[x, y] == 1)
                {
                    wallsTileMap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }

        DrawBase();
    }

    private void DrawBase()
    {
        if (currentBase != null)
        {
            Destroy(currentBase);
            Destroy(currentPlayer);
        }

        Vector3Int center = new Vector3Int(width / 2, height / 2, 0);
        currentBase = Instantiate(basePrefab, center, Quaternion.identity);

        UpdateMapFromBase();
    }

    private void UpdateMapFromBase()
    {
        Tilemap baseTilemap = basePrefab.transform.Find("FloorBase").GetComponent<Tilemap>();
        BoundsInt bounds = baseTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int center = new Vector3Int(width / 2, height / 2, 0);
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                if (baseTilemap.HasTile(tilePosition))
                {
                    Vector2Int cavePosition = new Vector2Int(x + center.x, y + center.y);

                    if (IsInMapRange(cavePosition.x, cavePosition.y))
                    {
                        caveMap[cavePosition.x, cavePosition.y] = 3; //base

                        floorsTilemap.SetTile(new Vector3Int(cavePosition.x, cavePosition.y, 0), null);
                        wallsTileMap.SetTile(new Vector3Int(cavePosition.x, cavePosition.y, 0), null);
                    }
                }
            }
        }

        var currentPosition = Vector3Int.RoundToInt(bounds.center);
        startingPosition.initialValue = new Vector2(currentPosition.x + width / 2 + 0.5f, currentPosition.y + height / 2 - 0.5f);
        currentPlayer = Instantiate(player);
    }

    private void FillMap() //Perlin Noise
    {
        Vector2 noiseOffset = new Vector2(Random.Range(0, 1000f), Random.Range(0, 1000f));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsMapBorder(x, y))
                {
                    caveMap[x, y] = 1;
                }
                else
                {
                    float perlinValue = Mathf.PerlinNoise((x + noiseOffset.x) * 0.1f, (y + noiseOffset.y) * 0.1f);
                    caveMap[x, y] = perlinValue < fillWallsPercent / 100f ? 1 : 0;
                }
            }
        }

        SmoothMap();
    }

    private void SmoothMap() //Cellular Automata
    {
        for (int i = 0; i < smoothIterations; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (IsMapBorder(x, y))
                    {
                        caveMap[x, y] = 1;
                    }
                    else
                    {
                        int neighborWalls = GetSurroundWallCount(x, y);

                        if (caveMap[x, y] == 1)
                        {
                            caveMap[x, y] = (neighborWalls >= minWallsToStayWall) ? 1 : 0;
                        }
                        else
                        {
                            caveMap[x, y] = (neighborWalls > minWallsToBecomeWall) ? 1 : 0;
                        }
                    }
                }
            }
        }
    }

    private int GetSurroundWallCount(int x, int y)
    {
        int wallCount = 0;

        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if (neighbourX != x || neighbourY != y)
                    {
                        wallCount += caveMap[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    private bool IsInMapRange(int x, int y)
    => x >= 0 && x < width && y >= 0 && y < height;

    private bool IsMapBorder(int x, int y)
    => x == 0 || x == width - 1 || y == 0 || y == height - 1;

    internal int[,] GetMap() => caveMap;

    internal void LoadMap(int[,] savedMap)
    {
        caveMap = savedMap;
        DrawMap();
    }
}