using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    private int[,] map;
    private int width;
    private int height;

    internal void Initialize(int[,] map, int width, int height)
    {
        this.map = map;
        this.width = width;
        this.height = height;
    }

    public List<List<Vector2Int>> GetAreas()
    {
        bool[,] visited = new bool[width, height];

        List<List<Vector2Int>> areas = new List<List<Vector2Int>>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0 && !visited[x, y])
                {
                    List<Vector2Int> area = new List<Vector2Int>();
                    DFSArea(x, y, visited, area);
                    areas.Add(area);
                }
            }
        }

        return areas;
    }

    private void DFSArea(int x, int y, bool[,] visited, List<Vector2Int> area) //Depth-First Search
    {
        if (!IsInMapRange(x, y) || visited[x, y] || map[x, y] == 1)
            return;

        visited[x, y] = true;
        area.Add(new Vector2Int(x, y));

        DFSArea(x + 1, y, visited, area);
        DFSArea(x - 1, y, visited, area);
        DFSArea(x, y + 1, visited, area);
        DFSArea(x, y - 1, visited, area);
    }

    private bool IsInMapRange(int x, int y)
    => x >= 0 && x < width && y >= 0 && y < height;
}
