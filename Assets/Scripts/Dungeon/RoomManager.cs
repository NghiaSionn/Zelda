using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    [Header("Room settings")]
    public int roomWidth = 10;
    public int roomHeight = 10;
    public int numberOfRooms = 5;
    public int spacingRoom = 2;
    public int corridorWidth = 2;


    [Header("Random Room Settings")]
    public bool isRandomWalk = false;
    public int walkLength = 100;
    public int walkIterations = 10;


    [Header("Other settings")]
    public Transform player;
    public VectorValue startingPosition;
    public TilemapVisualizer tilemapVisualizer;

    private Vector2Int[] directions = {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down
    };

    internal List<Room> rooms = new();
    internal Vector2Int startRoomPosition = new();
    internal HashSet<Vector2Int> floorPositions = new();

    public void GenerateRooms()
    {
        ClearDungeon();
        CreateRooms();
        SetPlayer();
    }

    public void ClearDungeon()
    {
        rooms.Clear();
        floorPositions.Clear();
        tilemapVisualizer.Clear();
    }

    private void CreateRooms()
    {
        Vector2Int currentRoomPosition = startRoomPosition;
        CreateRoom(currentRoomPosition);
        rooms.Add(new Room(currentRoomPosition, roomWidth, roomHeight));

        for (int i = 1; i < numberOfRooms; i++)
        {
            Vector2Int direction = directions[Random.Range(0, directions.Length)];
            Vector2Int newRoomPosition = currentRoomPosition + direction * new Vector2Int(roomWidth + spacingRoom, roomHeight + spacingRoom);

            if (!IsRoomOverlap(newRoomPosition))
            {
                CreateRoom(newRoomPosition);
                CreateCorridor(currentRoomPosition, newRoomPosition);
                rooms.Add(new Room(newRoomPosition, roomWidth, roomHeight));
                currentRoomPosition = newRoomPosition;
            }
            else
            {
                i--;
            }
        }

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private void CreateRoom(Vector2Int roomPosition)
    {
        if (isRandomWalk)
        {
            CreateRandomWalkRoom(roomPosition);
        }
        else
        {
            CreateRectangularRoom(roomPosition);
        }
    }

    private void CreateRectangularRoom(Vector2Int roomPosition)
    {
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                Vector2Int tilePos = new Vector2Int(x + roomPosition.x, y + roomPosition.y);
                floorPositions.Add(tilePos);
            }
        }
    }

    private void CreateRandomWalkRoom(Vector2Int startPosition)
    {
        floorPositions.Add(startPosition);
        Vector2Int currentPosition = startPosition;

        for (int i = 0; i < walkIterations; i++)
        {
            HashSet<Vector2Int> path = RandomWalk(currentPosition);
            foreach (var position in path)
            {
                floorPositions.Add(position);
            }
        }
    }

    private void CreateCorridor(Vector2Int fromRoom, Vector2Int toRoom)
    {
        Vector2Int start = fromRoom + new Vector2Int(roomWidth / 2, roomHeight / 2);
        Vector2Int end = toRoom + new Vector2Int(roomWidth / 2, roomHeight / 2);

        int xStart = Mathf.Min(start.x, end.x);
        int xEnd = Mathf.Max(start.x, end.x);

        for (int x = xStart; x <= xEnd; x++)
        {
            for (int w = 0; w < corridorWidth; w++)
            {
                Vector2Int tilePos = new Vector2Int(x, start.y - corridorWidth / 2 + w);
                floorPositions.Add(tilePos);
            }
        }

        int yStart = Mathf.Min(start.y, end.y);
        int yEnd = Mathf.Max(start.y, end.y);

        for (int y = yStart; y <= yEnd; y++)
        {
            for (int w = 0; w < corridorWidth; w++)
            {
                Vector2Int tilePos = new Vector2Int(end.x - corridorWidth / 2 + w, y);
                floorPositions.Add(tilePos);
            }
        }
    }

    private void SetPlayer()
    {
        startingPosition.initialValue = new Vector2(startRoomPosition.x + roomWidth / 2 + 0.5f, startRoomPosition.y + roomHeight / 2 + 0.5f);
        player.position = startingPosition.initialValue;

        int radius = 2;
        Vector2Int playerPosition = new Vector2Int((int)player.position.x, (int)player.position.y);

        for (int x = playerPosition.x - radius; x <= playerPosition.x + radius; x++)
        {
            for (int y = playerPosition.y - radius; y <= playerPosition.y + radius; y++)
            {
                Vector2Int tilePos = new Vector2Int(x, y);
                floorPositions.Add(tilePos);
            }
        }
    }

    private HashSet<Vector2Int> RandomWalk(Vector2Int startPosition)
    {
        HashSet<Vector2Int> path = new();
        path.Add(startPosition);

        Vector2Int currentPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            Vector2Int direction = directions[Random.Range(0, directions.Length)];
            currentPosition += direction;

            path.Add(currentPosition);
        }

        return path;
    }

    private bool IsRoomOverlap(Vector2Int newRoomPosition)
    {
        foreach (var room in rooms)
        {
            if (room.position == newRoomPosition)
            {
                return true;
            }
        }
        return false;
    }
}