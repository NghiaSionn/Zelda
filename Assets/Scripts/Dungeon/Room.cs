using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Start,
    Normal,
    Boss,
    Treasure
}

[System.Serializable]
public class Room
{
    public Vector2Int position;
    public int width;
    public int height;
    public RoomType type;
    public List<Vector2Int> connectedDoors = new();
    public Dictionary<Vector2Int, Room> neighbors = new();

    public Room(Vector2Int position, int width, int height, RoomType type = RoomType.Normal)
    {
        this.position = position;
        this.width = width;
        this.height = height;
        this.type = type;
    }

    public bool CanConnectInDirection(Vector2Int direction)
    {
        return !connectedDoors.Contains(direction);
    }
}