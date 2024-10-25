using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonManager : MonoBehaviour
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

    private Vector2Int[] directions = {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down
    };

    private List<Vector2Int> roomPositionsList = new List<Vector2Int>();

    void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        Vector2Int currentRoomPosition = new();
        CreateRoom(currentRoomPosition);
        roomPositionsList.Add(currentRoomPosition);

        for (int i = 1; i < numberOfRooms; i++)
        {
            Vector2Int direction = directions[Random.Range(0, directions.Length)];
            Vector2Int newRoomPosition = currentRoomPosition +
            direction * new Vector2Int(roomWidth, roomHeight);


            if (!roomPositionsList.Contains(newRoomPosition))
            {
                CreateRoom(newRoomPosition);
                roomPositionsList.Add(newRoomPosition);
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
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                floorTilemap.SetTile(new Vector3Int(x + roomPosition.x, y + roomPosition.y, 0), floorTile);

                if (x == 0 || x == roomWidth - 1 || y == 0 || y == roomHeight - 1)
                {
                    wallTilemap.SetTile(new Vector3Int(x + roomPosition.x, y + roomPosition.y, 0), wallTile);
                }
            }
        }
    }
}