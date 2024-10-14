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
    [SerializeField][Range(0, 100)] private int fillWallsPercent;
    [SerializeField] private int smoothIterations = 3;
    [SerializeField] private int maxAroundWall = 4;

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
    private GameObject currentBase;
    private GameObject currentPlayer;

    internal void GenerateMap()
    {
        caveMap = new int[width, height];

        FillMap();

        for (int i = 0; i < smoothIterations; i++)
        {
            SmoothMap();
        }

        DrawMap();
    }

    private void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundWallCount(x, y);
                caveMap[x, y] = (neighbourWallTiles <= maxAroundWall) ? 0 : 1;
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
        startingPosition.initialValue = new Vector2(17.5f, 15.5f);

        currentPlayer = Instantiate(player);
        currentBase = Instantiate(basePrefab, center, Quaternion.identity);
    }

    private void FillMap()
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
                    caveMap[x, y] = Random.Range(0, 100) < fillWallsPercent ? 1 : 0;
                }
            }
        }
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