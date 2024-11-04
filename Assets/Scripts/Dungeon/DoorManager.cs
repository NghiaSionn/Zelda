using UnityEngine;
using System.Collections.Generic;

public class DoorManager : MonoBehaviour
{
    [Header("Door Prefabs")]
    public GameObject horizontalDoorPrefab;
    public GameObject verticalDoorPrefab;

    public RoomManager roomManager;
    private Dictionary<Vector2, Door> createdDoors = new();

    public void GenerateDoors()
    {
        createdDoors.Clear();
        foreach (Room room in roomManager.rooms)
        {
            CreateDoorsForRoom(room);
        }
    }

    private void CreateDoorsForRoom(Room room)
    {
        foreach (var neighborPair in room.neighbors)
        {
            Vector2Int direction = neighborPair.Key;
            Room neighborRoom = neighborPair.Value;

            Vector3 doorPosition = CalculateCorridorDoorPosition(room, neighborRoom);
            if (direction.x != 0) doorPosition.y += 0.3f;

            if (!createdDoors.ContainsKey(doorPosition))
            {
                CreateDoor(room, neighborRoom, direction, doorPosition);
            }
            else
            {
                Door existingDoor = createdDoors[doorPosition];
                if (!room.doors.Contains(existingDoor))
                {
                    room.doors.Add(existingDoor);
                }
            }
        }
    }

    private Vector3 CalculateCorridorDoorPosition(Room fromRoom, Room toRoom)
    {
        Vector3 fromCenter = fromRoom.GetCenter();
        Vector3 toCenter = toRoom.GetCenter();

        return Vector3.Lerp(fromCenter, toCenter, 0.5f);
    }

    private void CreateDoor(Room fromRoom, Room toRoom, Vector2Int direction, Vector3 position)
    {
        GameObject prefabToUse = (direction.x != 0) ? horizontalDoorPrefab : verticalDoorPrefab;

        GameObject doorObj = Instantiate(prefabToUse, position, Quaternion.identity, transform);
        Door door = doorObj.GetComponent<Door>();

        fromRoom.doors.Add(door);
        toRoom.doors.Add(door);

        createdDoors.Add(position, door);
    }
}