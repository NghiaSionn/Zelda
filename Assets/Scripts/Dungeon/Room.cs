using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum RoomType
{
    Start,
    Normal,
    Boss,
    Treasure
}

[Serializable]
public class Room
{
    public int width;
    public int height;
    public RoomType type;
    public Vector2Int position;
    public HashSet<Vector2Int> floorPositions = new();
    public Dictionary<Vector2Int, Room> neighbors = new();
    public List<Door> doors = new();
    public List<EnemyDungeon> enemies = new();

    public Room(Vector2Int position, int width, int height, RoomType type = RoomType.Normal)
    {
        this.position = position;
        this.width = width;
        this.height = height;
        this.type = type;
    }

    public bool CanConnectInDirection(Vector2Int direction)
    {
        return !neighbors.ContainsKey(direction);
    }

    public Vector3 GetCenter()
    {
        int centerX = position.x + width / 2;
        int centerY = position.y + height / 2;
        return new Vector3(centerX, centerY);
    }

    public List<Vector2Int> GetFloor()
    {
        return floorPositions.Where(pos => pos.x >= position.x && pos.x < position.x + width
                                    && pos.y >= position.y && pos.y < position.y + height)
                             .ToList();
    }

    public bool ContainsPlayer(Vector3 playerPosition)
    {
        float playerX = playerPosition.x;
        float playerY = playerPosition.y;
        return playerX >= position.x && playerX < position.x + width
            && playerY >= position.y && playerY < position.y + height;
    }

    public void LockDoors()
    {
        foreach (var door in doors)
        {
            door.Lock();
        }
    }

    public void UnlockDoors()
    {
        if (isCleared())
        {
            foreach (var door in doors)
            {
                door.Unlock();
            }
        }
    }

    public bool isCleared()
    {
        if(enemies.Count == 0)
        {
            return true;
        }
        else {
            return false;
        }
    }
}