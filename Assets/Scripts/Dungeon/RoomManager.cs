using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    [Header("Room settings")]
    public int roomWidth = 10;
    public int roomHeight = 10;
    public int numberOfRooms = 5;
    public int corridorLength = 4;
    public int corridorWidth = 1;

    [Header("Room type chances")]
    [Range(0, 100)] public int treasureRoomChance = 20;


    [Header("References")]
    public Transform player;
    public VectorValue startingPosition;
    public TilemapVisualizer tilemapVisualizer;

    internal List<Room> rooms = new();
    internal Vector2Int startRoomPosition = Vector2Int.zero;
    internal HashSet<Vector2Int> floorPositions = new();
    private HashSet<Vector2Int> potentialDoorPositions = new();
    private Vector2Int[] directions = {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down
    };

    public void GenerateRooms()
    {
        ClearRooms();
        CreateRooms();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);

        SetPlayer();
    }

    public void ClearRooms()
    {
        rooms.Clear();
        floorPositions.Clear();
        tilemapVisualizer.Clear();
    }

    private void CreateRooms()
    {
        var startRoom = new Room(startRoomPosition, roomWidth, roomHeight, RoomType.Start);
        rooms.Add(startRoom);
        CreateRoom(startRoomPosition);

        Queue<Room> roomQueue = new Queue<Room>();
        roomQueue.Enqueue(rooms[0]);

        int roomsCreated = 1;

        while(roomQueue.Count > 0 && roomsCreated < numberOfRooms)
        {
            Room currentRoom = roomQueue.Dequeue();

            for(int i = 0; i < directions.Length; i++)
            {
                int randomIndex = Random.Range(i, directions.Length);
                (directions[i], directions[randomIndex]) = (directions[randomIndex], directions[i]);
            }

            foreach (Vector2Int direction in directions)
            {
                if (!currentRoom.CanConnectInDirection(direction) || roomsCreated >= numberOfRooms)
                    continue;

                Vector2Int newRoomPosition = currentRoom.position + direction * (roomWidth + corridorLength);

                if (IsRoomOverlap(newRoomPosition))
                    continue;

                RoomType newRoomType = DetermineRoomType(roomsCreated);

                var newRoom = new Room(newRoomPosition, roomWidth, roomHeight, newRoomType);
                rooms.Add(newRoom);
                CreateRoom(newRoomPosition);

                CreateCorridor(currentRoom.position, newRoomPosition, direction);

                currentRoom.connectedDoors.Add(direction);
                newRoom.connectedDoors.Add(-direction);
                currentRoom.neighbors[direction] = newRoom;
                newRoom.neighbors[-direction] = currentRoom;

                roomQueue.Enqueue(newRoom);
                roomsCreated++;

                if (newRoomType == RoomType.Treasure)
                    break;
            }
        }
    }

    private void CreateRoom(Vector2Int roomPosition)
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

    private bool IsRoomOverlap(Vector2Int newRoomPosition)
    {
        foreach (var room in rooms)
        {
            Rect existingRect = new Rect(
                room.position.x - 1,
                room.position.y - 1,
                roomWidth + 2,
                roomHeight + 2
            );

            Rect newRect = new Rect(
                newRoomPosition.x - 1,
                newRoomPosition.y - 1,
                roomWidth + 2,
                roomHeight + 2
            );

            if (existingRect.Overlaps(newRect))
                return true;
        }
        return false;
    }

    private RoomType DetermineRoomType(int currentRoomCount)
    {
        if (currentRoomCount == 0)
            return RoomType.Start;

        if (currentRoomCount == numberOfRooms - 1)
            return RoomType.Boss;

        int roll = Random.Range(0, 100);
        if (roll < treasureRoomChance && !rooms.Any(r => r.type == RoomType.Treasure))
            return RoomType.Treasure;

        return RoomType.Normal;
    }

    private void CreateCorridor(Vector2Int fromRoom, Vector2Int toRoom, Vector2Int direction)
    {
        Vector2Int current = fromRoom + new Vector2Int(roomWidth / 2, roomHeight / 2);
        Vector2Int target = toRoom + new Vector2Int(roomWidth / 2, roomHeight / 2);

        Vector2Int doorStart = current + direction * (roomWidth / 2);
        Vector2Int doorEnd = target - direction * (roomWidth / 2);
        potentialDoorPositions.Add(doorStart);
        potentialDoorPositions.Add(doorEnd);

        while (current != target)
        {
            current += direction;
            floorPositions.Add(current);

            if (direction.x != 0)
            {
                for (int i = 1; i <= corridorWidth / 2; i++)
                {
                    floorPositions.Add(current + Vector2Int.up * i);
                }
                for (int i = 1; i <= (corridorWidth - 1) / 2; i++)
                {
                    floorPositions.Add(current + Vector2Int.down * i);
                }
            }

            if (direction.y != 0)
            {
                for (int i = 1; i <= corridorWidth / 2; i++)
                {
                    floorPositions.Add(current + Vector2Int.right * i);
                }
                for (int i = 1; i <= (corridorWidth - 1) / 2; i++)
                {
                    floorPositions.Add(current + Vector2Int.left * i);
                }
            }
        }
    }

    private void SetPlayer()
    {
        startingPosition.initialValue = new Vector2(
            startRoomPosition.x + roomWidth / 2 + 0.5f,
            startRoomPosition.y + roomHeight / 2 + 0.5f
        );
        player.position = startingPosition.initialValue;
    }
}