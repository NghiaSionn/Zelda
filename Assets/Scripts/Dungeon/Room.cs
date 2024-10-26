using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    public Vector2Int position;
    public int width;
    public int height;

    public Room(Vector2Int position, int width, int height)
    {
        this.position = position;
        this.width = width;
        this.height = height;
    }
}

