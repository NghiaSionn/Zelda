using UnityEngine;
using System.Collections.Generic;

public class DoorManager : MonoBehaviour
{
    public RoomManager roomManager;

    [Header("Door Prefabs")]
    public GameObject doorHorizontalPrefab;
    public GameObject doorVerticalPrefab;
    public GameObject bossDoorHorizontalPrefab;
    public GameObject bossDoorVerticalPrefab;

    private Dictionary<Vector2Int, List<GameObject>> roomDoors = new();

    public void GenerateDoors()
    {
        ClearAllDoors();
        CreateDoorsForAllRooms();
    }

    private void ClearAllDoors()
    {
        foreach (var doorList in roomDoors.Values)
        {
            foreach (var door in doorList)
            {
                if (door != null)
                    Destroy(door);
            }
        }
        roomDoors.Clear();
    }

    private void CreateDoorsForAllRooms()
    {
        for (int i = 1; i < roomManager.rooms.Count; i++)
        {
            Room currentRoom = roomManager.rooms[i];
            bool isBossRoom = i == roomManager.rooms.Count - 1;

            Room previousRoom = roomManager.rooms[i - 1];
            CreateDoorsBetweenRooms(previousRoom, currentRoom, isBossRoom);
        }
    }

    private void CreateDoorsBetweenRooms(Room fromRoom, Room toRoom, bool isBossRoom)
    {
        Vector2 direction = new Vector2(toRoom.position.x - fromRoom.position.x,
                                      toRoom.position.y - fromRoom.position.y).normalized;

        float centerCorridor = roomManager.spacingRoom / 2f;
        Vector3 doorPosition;
        GameObject doorPrefabToUse;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            doorPrefabToUse = isBossRoom ? bossDoorHorizontalPrefab : doorHorizontalPrefab;
            doorPosition = direction.x > 0 ? new Vector3(toRoom.position.x - centerCorridor,
                                                         toRoom.position.y + toRoom.height / 2, 0)
                                           : new Vector3(toRoom.position.x + toRoom.width  + centerCorridor,
                                                         toRoom.position.y + toRoom.height / 2, 0);
        }
        else
        {
            doorPrefabToUse = isBossRoom ? bossDoorVerticalPrefab : doorVerticalPrefab;
            doorPosition = direction.y > 0 ? new Vector3(toRoom.position.x + toRoom.width / 2,
                                                         toRoom.position.y - centerCorridor, 0)
                                           : new Vector3(toRoom.position.x + toRoom.width / 2,
                                                         toRoom.position.y + toRoom.height + centerCorridor, 0);
        }
        GameObject door = Instantiate(doorPrefabToUse, doorPosition, Quaternion.identity, transform);
        Vector2Int roomKey = new Vector2Int(toRoom.position.x, toRoom.position.y);
        if (!roomDoors.ContainsKey(roomKey))
        {
            roomDoors[roomKey] = new List<GameObject>();
        }
        roomDoors[roomKey].Add(door);
    }
}