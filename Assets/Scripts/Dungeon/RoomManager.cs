using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    [Header("Tilemap settings")]
    public Tilemap wallTilemap;
    public Tilemap floorTilemap;
    public TileBase wallTile;
    public TileBase floorTile;

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

    private Vector2Int[] directions = {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down
    };

    internal List<Room> rooms = new();
    internal Vector2Int startRoom = new();

    public void GenerateDungeon()
    {
        ClearDungeon();
        CreateRooms();
        SetPlayer();
        AddWallsAroundFloors();
    }

    public void ClearDungeon()
    {
        rooms.Clear();
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    private void CreateRooms()
    {
        Vector2Int currentRoomPosition = startRoom;
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
                floorTilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), floorTile);
            }
        }
    }

    private void CreateRandomWalkRoom(Vector2Int startPosition)
    {
        floorTilemap.SetTile(new Vector3Int(startPosition.x, startPosition.y, 0), floorTile);

        Vector2Int currentPosition = startPosition;

        for (int i = 0; i < walkIterations; i++)
        {
            HashSet<Vector2Int> path = RandomWalk(currentPosition);

            foreach (var position in path)
            {
                floorTilemap.SetTile(new Vector3Int(position.x, position.y, 0), floorTile);
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
                floorTilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), floorTile);
            }
        }

        int yStart = Mathf.Min(start.y, end.y);
        int yEnd = Mathf.Max(start.y, end.y);

        for (int y = yStart; y <= yEnd; y++)
        {
            for (int w = 0; w < corridorWidth; w++)
            {
                Vector2Int tilePos = new Vector2Int(end.x - corridorWidth / 2 + w, y);
                floorTilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), floorTile);
            }
        }
    }

    private void AddWallsAroundFloors()
    {
        BoundsInt bounds = floorTilemap.cellBounds;

        for (int x = bounds.xMin - 1; x <= bounds.xMax + 1; x++)
        {
            for (int y = bounds.yMin - 1; y <= bounds.yMax + 1; y++)
            {
                Vector3Int currentPos = new(x, y, 0);

                if (floorTilemap.GetTile(currentPos) == null)
                {
                    bool hasAdjacentFloor = false;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            Vector3Int checkPos = currentPos + new Vector3Int(dx, dy, 0);
                            if (floorTilemap.GetTile(checkPos) != null)
                            {
                                hasAdjacentFloor = true;
                                break;
                            }
                        }
                        if (hasAdjacentFloor) break;
                    }

                    if (hasAdjacentFloor)
                    {
                        wallTilemap.SetTile(currentPos, wallTile);
                    }
                }
            }
        }
    }

    private void SetPlayer()
    {
        startingPosition.initialValue = new Vector2(startRoom.x + roomWidth / 2 + 0.5f, startRoom.y + roomHeight / 2 + 0.5f);
        player.position = startingPosition.initialValue;

        int radius = 2;
        Vector2Int playerPosition = new Vector2Int((int)player.position.x, (int)player.position.y);

        DrawFloorTilesInRadius(playerPosition, radius);
    }

    public void DrawFloorTilesInRadius(Vector2Int position, int radius)
    {
        for (int x = position.x - radius; x <= position.x + radius; x++)
        {
            for (int y = position.y - radius; y <= position.y + radius; y++)
            {
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
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