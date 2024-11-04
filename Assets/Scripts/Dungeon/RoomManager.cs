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
    public DoorManager doorManager;

    internal HashSet<Room> rooms = new();
    private Vector2Int startRoomPosition = Vector2Int.zero;
    private HashSet<Vector2Int> floorPositions = new();
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
        doorManager.GenerateDoors();
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
        CreateRoom(startRoom);
        SetPlayer(startRoom);

        Queue<Room> roomQueue = new();
        roomQueue.Enqueue(startRoom);

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
                if (!currentRoom.CanConnectInDirection(direction) || roomsCreated >= numberOfRooms) continue;

                Vector2Int newRoomPosition = currentRoom.position + direction
                                             * ((roomWidth > roomHeight ? roomWidth : roomHeight) + corridorLength);
                RoomType newRoomType = DetermineRoomType(roomsCreated);
                Room newRoom = new Room(newRoomPosition, roomWidth, roomHeight, newRoomType);

                CreateRoom(newRoom);
                CreateCorridor(currentRoom.position, newRoomPosition, direction);

                currentRoom.neighbors[direction] = newRoom;
                newRoom.neighbors[-direction] = currentRoom;

                roomQueue.Enqueue(newRoom);
                roomsCreated++;

                if (newRoomType == RoomType.Treasure) break;
            }
        }

        foreach (Room room in rooms)
        {
            if (Application.IsPlaying(this))
            {
                Instantiate(new GameObject($"{room.type}_{room.position}"), room.GetCenter(), Quaternion.identity, this.transform);
            }
        }
    }
    private void CreateRoom(Room room)
    {
        rooms.Add(room);

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                Vector2Int tilePos = new Vector2Int(x + room.position.x, y + room.position.y);
                room.floorPositions.Add(tilePos);
            }
        }

        floorPositions.UnionWith(room.floorPositions);
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

        while (current != target)
        {
            current += direction;
            floorPositions.Add(current);

            if (direction.x != 0)
            {
                for (int i = 1; i <= (corridorWidth - 1) / 2; i++)
                {
                    floorPositions.Add(current + Vector2Int.up * i);
                }
                for (int i = 1; i <= corridorWidth / 2; i++)
                {
                    floorPositions.Add(current + Vector2Int.down * i);
                }
            }

            if (direction.y != 0)
            {
                for (int i = 1; i <= (corridorWidth - 1) / 2; i++)
                {
                    floorPositions.Add(current + Vector2Int.right * i);
                }
                for (int i = 1; i <= corridorWidth / 2; i++)
                {
                    floorPositions.Add(current + Vector2Int.left * i);
                }
            }
        }
    }

    private void SetPlayer(Room room)
    {
        startingPosition.initialValue = room.GetCenter();
        player.position = startingPosition.initialValue;
    }
}