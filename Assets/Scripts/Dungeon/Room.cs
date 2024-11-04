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

[System.Serializable]
public class Room
{
    public int width;
    public int height;
    public RoomType type;
    public Vector2Int position;
    public HashSet<Vector2Int> floorPositions = new();
    public Dictionary<Vector2Int, Room> neighbors = new();
    public List<Door> doors = new List<Door>();
    public bool hasEnemies;
    public bool isCleared;
    public int enemyCount;
    public event Action OnRoomStateChanged;

    public Room(Vector2Int position, int width, int height, RoomType type = RoomType.Normal)
    {
        this.position = position;
        this.width = width;
        this.height = height;
        this.type = type;

        hasEnemies = type != RoomType.Start && type != RoomType.Treasure;
        isCleared = !hasEnemies;
        enemyCount = 0;
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

    public void SetEnemyCount(int count)
    {
        enemyCount = count;
        hasEnemies = count > 0;
        isCleared = count == 0;
        OnRoomStateChanged?.Invoke();
    }

    public void OnEnemyDefeated()
    {
        if (enemyCount > 0)
        {
            enemyCount--;
            Debug.Log(enemyCount);
            if (enemyCount == 0)
            {
                isCleared = true;
                OnRoomCleared();
                OnRoomStateChanged?.Invoke();
            }
        }
    }

    protected virtual void OnRoomCleared()
    {
        Debug.Log($"Room cleared at position {position}");
        foreach (var door in doors)
        {
            door.UpdateDoorState();
        }
    }
}