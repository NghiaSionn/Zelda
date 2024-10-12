using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class CaveManager : MonoBehaviour
{
    [Header("Cave settings")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int fillWallsPercent;

    [Header("Tile settings")]
    [SerializeField] private Tilemap floorsTilemap; //0
    [SerializeField] private Tilemap wallsTileMap; //1
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;

    private int[,] map;

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        map = new int[width, height];

        FillMap();

        DrawMap();
    }

    private void DrawMap()
    {
        floorsTilemap.ClearAllTiles();
        wallsTileMap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(map[x,y] == 0)
                {
                    floorsTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
                else
                {
                    wallsTileMap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }
    }

    private void FillMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(IsMapBorder(x, y))
                {
                    map[x,y] = 1;
                }
                else
                {
                    map[x,y] = Random.Range(0, 100) < fillWallsPercent ? 1 : 0;
                }
            }
        }
    }

    private bool IsMapBorder(int x, int y)
    => x == 0 || x == width - 1 || y == 0 || y == height - 1;
}